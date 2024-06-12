import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  HostListener,
  Inject,
  OnDestroy,
  OnInit,
} from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from "@angular/material/dialog";
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";
import { BehaviorSubject, of, ReplaySubject, Subscription } from "rxjs";
import { AuthService } from "src/app/modules/auth/_services/auth.service";
import {
  LayoutUtilsService,
  MessageType,
} from "../../../../_core/utils/layout-utils.service";
import {
  ResultModel,
  ResultObjModel,
} from "../../../../_core/models/_base.model";
import { DatePipe } from "@angular/common";
import { finalize, tap } from "rxjs/operators";
import { PaginatorState } from "src/app/_metronic/shared/crud-table";
import { LoaiTaiSanManagementService } from "../Services/loaitaisan-management.service";
import { LoaiTaiSanModel } from "../Model/loaitaisan-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";

@Component({
  selector: "app-loaitaisan-management-edit-dialog",
  templateUrl: "./loaitaisan-management-edit-dialog.component.html",
  styleUrls: ["./StylePhongTo.component.css"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoaiTaiSanManagementEditDialogComponent
  implements OnInit, OnDestroy
{
  item: LoaiTaiSanModel = new LoaiTaiSanModel();
  itemkho: LoaiTaiSanModel = new LoaiTaiSanModel();
  isLoading;
  formGroup: FormGroup;
  itemslts: LoaiTaiSanModel[] = [];
  khoFilters: LoaiTaiSanModel[] = [];
  loaiMHFilters: LoaiTaiSanModel[] = [];
  private subscriptions: Subscription[] = [];
  isExpanded = false;
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<LoaiTaiSanManagementEditDialogComponent>,
    private fb: FormBuilder,
    public loaitaisanManagementService: LoaiTaiSanManagementService,
    private changeDetect: ChangeDetectorRef,
    private layoutUtilsService: LayoutUtilsService,
    public general: GeneralService,
    public authService: AuthService,
    public datepipe: DatePipe,
    public dialog: MatDialog,
    private translateService: TranslateService
  ) {}

  ngOnDestroy(): void {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
  }

  ngOnInit(): void {
    this.item.empty();
    this.item.IdLoaiTS = this.data.item.IdLoaiTS;
    this.loadForm();
    const sb = this.loaitaisanManagementService.isLoading$.subscribe((res) => {
      this.isLoading = res;
    });
    this.subscriptions.push(sb);
    this.loadInitData();
    this.loaitaisanManagementService.items$.subscribe((data) => {
      this.itemslts = data;
    });
  }
  loadInitData() {
    if (this.item.IdLoaiTS !== 0) {
      const sbGet = this.loaitaisanManagementService
        .GetLoaiTaiSanID(this.item.IdLoaiTS)
        .pipe(
          tap((res: ResultObjModel<LoaiTaiSanModel>) => {
            if (res.status === 1) {
              this.item = res.data;
              this.setValueToForm(res.data);
            }
          })
        )
        .subscribe();
      this.subscriptions.push(sbGet);
    }
  }
  getTitle() {
    if (this.item.IdLoaiTS === 0) {
      return this.translateService.instant("COMMOM.THEMMOI");
    }
    return this.translateService.instant("COMMOM.CAPNHAT");
  }
  loadForm() {
    debugger;
    this.formGroup = this.fb.group({
      MaLoai: [
        "",
        Validators.compose([Validators.required, Validators.maxLength(50)]),
      ],
      TenLoai: ["", Validators.compose([Validators.required])],
      TrangThai: [false, Validators.compose([Validators.required])],
    });
  }
  Visible: boolean = true;
  handleKeyDown(event: KeyboardEvent) {
    // Kiểm tra nếu cả Ctrl và Enter đều được nhấn cùng lúc
    if (event.ctrlKey && event.key === "Enter") {
      // Gọi hàm lưu và đóng modal
      this.Luu();
      // Ngăn chặn hành vi mặc định của trình duyệt
      event.preventDefault();
    }
  }
  setValueToForm(model: LoaiTaiSanModel) {
    if (this.item.IdLoaiTS !== 0) this.Visible = false;
    this.formGroup.controls.MaLoai.setValue(model.MaLoai);
    this.formGroup.controls.TenLoai.setValue(model.TenLoai);
    this.formGroup.controls.TrangThai.setValue(model.TrangThai);
  }

  private prepareData(): LoaiTaiSanModel {
    let model = new LoaiTaiSanModel();
    model.empty();
    model.IdLoaiTS = this.item.IdLoaiTS;
    model.MaLoai = this.formGroup.controls.MaLoai.value;
    model.TenLoai = this.formGroup.controls.TenLoai.value;
    model.TrangThai =
      this.formGroup.controls.TrangThai.value !== ""
        ? this.formGroup.controls.TrangThai.value
        : false;
    return model;
  }

  Luu() {
    debugger;
    if (this.formGroup.valid) {
      const model = this.prepareData();
      const existsma = this.itemslts.some(
        (item) => item.MaLoai === model.MaLoai
      );
      if (existsma) {
        const message = this.translateService.instant("ERROR.MABITRUNG");
        this.layoutUtilsService.showActionNotification(
          message,
          MessageType.Read,
          999999999,
          true,
          false,
          3000,
          "top",
          0
        );
        return;
      }
      const exists = this.itemslts.some(
        (item) => item.TenLoai.toLowerCase() === model.TenLoai.toLowerCase()
      );
      if (exists) {
        const message = this.translateService.instant("ERROR.TENBITRUNG");
        this.layoutUtilsService.showActionNotification(
          message,
          MessageType.Read,
          999999999,
          true,
          false,
          3000,
          "top",
          0
        );
        return;
      }
      this.item.IdLoaiTS === 0 ? this.create(model) : this.update(model);
    } else {
      this.validateAllFormFields(this.formGroup);
    }
  }
  LuuVaThemMoi() {
    if (this.formGroup.valid) {
      const model = this.prepareData();
      const existsma = this.itemslts.some(
        (item) => item.MaLoai === model.MaLoai
      );
      if (existsma) {
        const message = this.translateService.instant("ERROR.MABITRUNG");
        this.layoutUtilsService.showActionNotification(
          message,
          MessageType.Read,
          999999999,
          true,
          false,
          3000,
          "top",
          0
        );
        return;
      }
      const exists = this.itemslts.some(
        (item) => item.TenLoai.toLowerCase() === model.TenLoai.toLowerCase()
      );
      if (exists) {
        const message = this.translateService.instant("ERROR.TENBITRUNG");
        this.layoutUtilsService.showActionNotification(
          message,
          MessageType.Read,
          999999999,
          true,
          false,
          3000,
          "top",
          0
        );
        return;
      }
      if (this.item.IdLoaiTS === 0) {
        this.createNew(model);
      } else {
        this.update(model);
      }
    } else {
      this.validateAllFormFields(this.formGroup);
    }
  }
  onMouseEnter(event: any, color: string) {
    event.target.style.backgroundColor = color;
  }

  onMouseLeave(event: any, color: string) {
    event.target.style.backgroundColor = color;
  }
  createNew(item: LoaiTaiSanModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.loaitaisanManagementService
        .DM_LoaiTaiSan_Insert(item)
        .subscribe((res) => {
          this.isLoadingSubmit$.next(false);
          if (res && res.status === 1) {
            this.formGroup.reset();
            Object.keys(this.formGroup.controls).forEach((key) => {
              this.formGroup.get(key).setErrors(null);
            });
            this.setValueToForm(this.item);
            this.layoutUtilsService.showActionNotification(
              "Thêm mới thành công",
              MessageType.Create,
              3000,
              true,
              false
            );
          } else {
            this.layoutUtilsService.showActionNotification(
              res.error.message,
              MessageType.Read,
              999999999,
              true,
              false,
              3000,
              "top",
              0
            );
          }
        });
  }
  toggleExpand() {
    this.isExpanded = !this.isExpanded;
  }
  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach((field) => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }

  create(item: LoaiTaiSanModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.loaitaisanManagementService
        .DM_LoaiTaiSan_Insert(item)
        .subscribe((res) => {
          this.isLoadingSubmit$.next(false);
          if (res && res.status === 1) {
            this.dialogRef.close(res);
          } else {
            this.layoutUtilsService.showActionNotification(
              res.error.message,
              MessageType.Read,
              999999999,
              true,
              false,
              3000,
              "top",
              0
            );
          }
        });
  }

  update(item: LoaiTaiSanModel) {
    debugger;
    this.isLoadingSubmit$.next(true);
    this.loaitaisanManagementService.UpdateLoaiTaiSan(item).subscribe((res) => {
      this.isLoadingSubmit$.next(false);
      if (res && res.status === 1) {
        this.dialogRef.close(res);
      } else {
        this.layoutUtilsService.showActionNotification(
          res.error.message,
          MessageType.Read,
          999999999,
          true,
          false,
          3000,
          "top",
          0
        );
      }
    });
  }

  goBack() {
    if (this.checkDataBeforeClose()) {
      this.dialogRef.close();
    } else {
      const _title = this.translateService.instant("CHECKPOPUP.TITLE");
      const _description = this.translateService.instant(
        "CHECKPOPUP.DESCRIPTION"
      );
      const _waitDesciption = this.translateService.instant(
        "CHECKPOPUP.WAITDESCRIPTION"
      );
      const popup = this.layoutUtilsService.deleteElement(
        _title,
        _description,
        _waitDesciption
      );
      popup.afterClosed().subscribe((res) => {
        res ? this.dialogRef.close() : undefined;
      });
    }
  }

  checkDataBeforeClose(): boolean {
    const model = this.prepareData();
    if (this.item.IdLoaiTS === 0) {
      const empty = new LoaiTaiSanModel();
      empty.empty();
      return this.general.isEqual(empty, model);
    }
    return this.general.isEqual(model, this.item);
  }

  @HostListener("window:beforeunload", ["$event"])
  beforeunloadHandler(e) {
    if (!this.checkDataBeforeClose()) {
      e.preventDefault(); //for Firefox
      return (e.returnValue = ""); //for Chorme
    }
  }
}
