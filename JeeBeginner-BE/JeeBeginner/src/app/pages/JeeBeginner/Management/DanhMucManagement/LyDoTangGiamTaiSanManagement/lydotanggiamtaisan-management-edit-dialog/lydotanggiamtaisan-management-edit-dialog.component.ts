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
import { LyDoTangGiamTaiSanManagementService } from "../Services/lydotanggiamtaisan-management.service";
import { LyDoTangGiamTaiSanModel } from "../Model/lydotanggiamtaisan-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";

@Component({
  selector: "app-lydotanggiamtaisan-management-edit-dialog",
  templateUrl: "./lydotanggiamtaisan-management-edit-dialog.component.html",
  styleUrls: ["./StylePhongTo.component.css"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LyDoTangGiamTaiSanManagementEditDialogComponent
  implements OnInit, OnDestroy
{
  item: LyDoTangGiamTaiSanModel = new LyDoTangGiamTaiSanModel();
  itemsldtgts: LyDoTangGiamTaiSanModel[] = [];
  itemkho: LyDoTangGiamTaiSanModel = new LyDoTangGiamTaiSanModel();
  isLoading;
  formGroup: FormGroup;
  khoFilters: LyDoTangGiamTaiSanModel[] = [];
  loaiMHFilters: LyDoTangGiamTaiSanModel[] = [];
  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isExpanded = false;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<LyDoTangGiamTaiSanManagementEditDialogComponent>,
    private fb: FormBuilder,
    public lydotanggiamtaisanManagementService: LyDoTangGiamTaiSanManagementService,
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
    debugger;
    this.item.empty();
    this.item.IdRow = this.data.item.IdRow;
    this.loadForm();
    console.log(this.formGroup.value);
    const sb = this.lydotanggiamtaisanManagementService.isLoading$.subscribe(
      (res) => {
        this.isLoading = res;
      }
    );
    this.subscriptions.push(sb);
    this.loadInitData();
    this.lydotanggiamtaisanManagementService.items$.subscribe((data) => {
      this.itemsldtgts = data;
    });
  }
  loadInitData() {
    if (this.item.IdRow !== 0) {
      const sbGet = this.lydotanggiamtaisanManagementService
        .GetLyDoTangGiamTaiSanID(this.item.IdRow)
        .pipe(
          tap((res: ResultObjModel<LyDoTangGiamTaiSanModel>) => {
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
  toggleExpand() {
    this.isExpanded = !this.isExpanded;
  }
  loadForm() {
    this.formGroup = this.fb.group({
      LoaiTangGiam: [
        "",
        Validators.compose([
          Validators.required,
          Validators.pattern(/^-?(0|[0-9]\d*)?$/),
          Validators.maxLength(50),
        ]),
      ],
      MaTangGiam: [
        "",
        Validators.compose([Validators.required, Validators.maxLength(50)]),
      ],
      TenTangGiam: ["", Validators.compose([Validators.required])],
      TrangThai: [false, Validators.compose([Validators.required])],
    });
  }
  onMouseEnter(event: any, color: string) {
    event.target.style.backgroundColor = color;
  }

  onMouseLeave(event: any, color: string) {
    event.target.style.backgroundColor = color;
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
  setValueToForm(model: LyDoTangGiamTaiSanModel) {
    if (this.item.IdRow !== 0) this.Visible = false;
    this.formGroup.controls.LoaiTangGiam.setValue(model.LoaiTangGiam);
    this.formGroup.controls.MaTangGiam.setValue(model.MaTangGiam);
    this.formGroup.controls.TenTangGiam.setValue(model.TenTangGiam);
    this.formGroup.controls.TrangThai.setValue(model.TrangThai);
  }
  getTitle() {
    if (this.item.IdRow === 0) {
      return this.translateService.instant("COMMOM.THEMMOI");
    }
    return this.translateService.instant("COMMOM.CAPNHAT");
  }
  private prepareData(): LyDoTangGiamTaiSanModel {
    let model = new LyDoTangGiamTaiSanModel();
    model.empty();
    model.IdRow = this.item.IdRow;
    model.LoaiTangGiam = this.formGroup.controls.LoaiTangGiam.value;
    model.MaTangGiam = this.formGroup.controls.MaTangGiam.value;
    model.TenTangGiam = this.formGroup.controls.TenTangGiam.value;
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
      const existsma = this.itemsldtgts.some(
        (item) => item.MaTangGiam === model.MaTangGiam
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
      const exists = this.itemsldtgts.some(
        (item) =>
          item.TenTangGiam.toLowerCase() === model.TenTangGiam.toLowerCase()
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
      this.item.IdRow === 0 ? this.create(model) : this.update(model);
    } else {
      this.validateAllFormFields(this.formGroup);
    }
  }
  LuuVaThemMoi() {
    if (this.formGroup.valid) {
      const model = this.prepareData();
      const existsma = this.itemsldtgts.some(
        (item) => item.MaTangGiam === model.MaTangGiam
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
      const exists = this.itemsldtgts.some(
        (item) =>
          item.TenTangGiam.toLowerCase() === model.TenTangGiam.toLowerCase()
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
      if (this.item.IdRow === 0) {
        this.createNew(model);
      } else {
        this.update(model);
      }
    } else {
      this.validateAllFormFields(this.formGroup);
    }
  }
  createNew(item: LyDoTangGiamTaiSanModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.lydotanggiamtaisanManagementService
        .DM_LyDoTangGiamTaiSan_Insert(item)
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

  create(item: LyDoTangGiamTaiSanModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.lydotanggiamtaisanManagementService
        .DM_LyDoTangGiamTaiSan_Insert(item)
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

  update(item: LyDoTangGiamTaiSanModel) {
    debugger;
    this.isLoadingSubmit$.next(true);
    this.lydotanggiamtaisanManagementService
      .UpdateLyDoTangGiamTaiSan(item)
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
    if (this.item.IdRow === 0) {
      const empty = new LyDoTangGiamTaiSanModel();
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
