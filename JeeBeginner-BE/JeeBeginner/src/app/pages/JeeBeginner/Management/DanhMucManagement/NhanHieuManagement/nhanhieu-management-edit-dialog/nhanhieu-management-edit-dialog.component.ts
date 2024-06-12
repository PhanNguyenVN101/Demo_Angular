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
import { NhanHieuManagementService } from "../Services/nhanhieu-management.service";
import { NhanHieuModel } from "../Model/nhanhieu-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";
import { debug } from "console";

@Component({
  selector: "app-nhanhieu-management-edit-dialog",
  templateUrl: "./nhanhieu-management-edit-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NhanHieuManagementEditDialogComponent
  implements OnInit, OnDestroy
{
  item: NhanHieuModel = new NhanHieuModel();
  // itemcha: LoaiMatHangModel = new LoaiMatHangModel();
  // itemkho: LoaiMatHangModel = new LoaiMatHangModel();
  isLoading;
  formGroup: FormGroup;
  itemsnh: NhanHieuModel[] = [];
  // khoFilters: NhanHieuModel[] = [];
  // loaiMHFilters: NhanHieuModel[] = [];
  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<NhanHieuManagementEditDialogComponent>,
    private fb: FormBuilder,
    public nhanhieuManagementService: NhanHieuManagementService,
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
    // this.itemkho.empty();
    // this.itemcha.empty();
    this.item.empty();
    // this.itemcha.IdLMHParent = this.data.item.IdLMHParent;
    // this.itemkho.IdKho = this.data.item.IdKho;
    this.item.IdNhanHieu = this.data.item.IdNhanHieu;
    this.loadForm();
    const sb = this.nhanhieuManagementService.isLoading$.subscribe((res) => {
      this.isLoading = res;
    });
    this.subscriptions.push(sb);

    this.loadInitData();
    this.nhanhieuManagementService.items$.subscribe((data) => {
      this.itemsnh = data;
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
  loadInitData() {
    if (this.item.IdNhanHieu !== 0) {
      const sbGet = this.nhanhieuManagementService
        .GetNhanHieuID(this.item.IdNhanHieu)
        .pipe(
          tap((res: ResultObjModel<NhanHieuModel>) => {
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

  loadForm() {
    this.formGroup = this.fb.group({
      tennhanhieu: [
        "",
        Validators.compose([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(50),
        ]),
      ],
    });
  }

  setValueToForm(model: NhanHieuModel) {
    if (this.item.IdNhanHieu !== 0) this.Visible = false;
    this.formGroup.controls.tennhanhieu.setValue(model.TenNhanHieu);
  }
  getTitle() {
    if (this.item.IdNhanHieu === 0) {
      return this.translateService.instant("COMMOM.THEMMOI");
    }
    return this.translateService.instant("COMMOM.CAPNHAT");
  }
  private prepareData(): NhanHieuModel {
    let model = new NhanHieuModel();
    model.empty();
    model.TenNhanHieu = this.formGroup.controls.tennhanhieu.value;
    model.IdNhanHieu = this.item.IdNhanHieu;

    return model;
  }
  onMouseEnter(event: any, color: string) {
    event.target.style.backgroundColor = color;
  }

  onMouseLeave(event: any, color: string) {
    event.target.style.backgroundColor = color;
  }
  Luu() {
    if (this.formGroup.valid) {
      const model = this.prepareData();
      const exists = this.itemsnh.some(
        (item) =>
          item.TenNhanHieu.toLowerCase() === model.TenNhanHieu.toLowerCase()
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
      this.item.IdNhanHieu === 0 ? this.create(model) : this.update(model);
    } else {
      this.validateAllFormFields(this.formGroup);
    }
  }
  LuuVaThemMoi() {
    if (this.formGroup.valid) {
      const model = this.prepareData();
      const exists = this.itemsnh.some(
        (item) =>
          item.TenNhanHieu.toLowerCase() === model.TenNhanHieu.toLowerCase()
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
      if (this.item.IdNhanHieu === 0) {
        this.createNew(model);
      } else {
        this.update(model);
      }
    } else {
      this.validateAllFormFields(this.formGroup);
    }
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
  createNew(item: NhanHieuModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.nhanhieuManagementService
        .DM_NhanHieu_Insert(item)
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
  create(item: NhanHieuModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.nhanhieuManagementService
        .DM_NhanHieu_Insert(item)
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

  update(item: NhanHieuModel) {
    this.isLoadingSubmit$.next(true);
    this.nhanhieuManagementService.UpdateNhanHieu(item).subscribe((res) => {
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
    if (this.item.IdNhanHieu === 0) {
      const empty = new NhanHieuModel();
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
