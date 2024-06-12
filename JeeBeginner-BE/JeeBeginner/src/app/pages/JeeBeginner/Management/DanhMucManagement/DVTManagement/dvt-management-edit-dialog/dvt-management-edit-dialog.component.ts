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
import { DVTManagementService } from "../Services/dvt-management.service";
import { DVTModel } from "../Model/dvt-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";
import { debug } from "console";

@Component({
  selector: "app-dvt-management-edit-dialog",
  templateUrl: "./dvt-management-edit-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DVTManagementEditDialogComponent implements OnInit, OnDestroy {
  item: DVTModel = new DVTModel();
  isLoading;
  formGroup: FormGroup;
  private subscriptions: Subscription[] = [];
  itemsdvt: DVTModel[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<DVTManagementEditDialogComponent>,
    private fb: FormBuilder,
    public dvtManagementService: DVTManagementService,
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
    this.item.IdDVT = this.data.item.IdDVT;
    this.loadForm();
    const sb = this.dvtManagementService.isLoading$.subscribe((res) => {
      this.isLoading = res;
    });
    this.subscriptions.push(sb);
    this.loadInitData();
    this.dvtManagementService.items$.subscribe((data) => {
      this.itemsdvt = data;
    });
  }

  loadInitData() {
    if (this.item.IdDVT !== 0) {
      const sbGet = this.dvtManagementService
        .GetDVTID(this.item.IdDVT)
        .pipe(
          tap((res: ResultObjModel<DVTModel>) => {
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
      tendvt: [
        "",
        Validators.compose([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(50),
        ]),
      ],
    });
  }

  setValueToForm(model: DVTModel) {
    if (this.item.IdDVT !== 0) this.Visible = false;
    this.formGroup.controls.tendvt.setValue(model.TenDVT);
  }

  private prepareData(): DVTModel {
    let model = new DVTModel();
    model.empty();
    model.TenDVT = this.formGroup.controls.tendvt.value;
    model.IdDVT = this.item.IdDVT;

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
      const exists = this.itemsdvt.some(
        (item) => item.TenDVT.toLowerCase() === model.TenDVT.toLowerCase()
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
      this.item.IdDVT === 0 ? this.create(model) : this.update(model);
    } else {
      this.validateAllFormFields(this.formGroup);
    }
  }
  LuuVaThemMoi() {
    if (this.formGroup.valid) {
      const model = this.prepareData();
      const exists = this.itemsdvt.some(
        (item) => item.TenDVT.toLowerCase() === model.TenDVT.toLowerCase()
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
      this.item.IdDVT === 0 ? this.createNew(model) : this.update(model);
    } else {
      this.validateAllFormFields(this.formGroup);
    }
  }
  resetForm() {
    this.formGroup.reset();

    Object.keys(this.formGroup.controls).forEach((key) => {
      this.formGroup.get(key).setErrors(null);
    });
    this.setValueToForm(this.item);
  }

  resetData() {
    this.resetForm();
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
  getTitle() {
    if (this.item.IdDVT === 0) {
      return this.translateService.instant("COMMOM.THEMMOI");
    }
    return this.translateService.instant("COMMOM.CAPNHAT");
  }
  createNew(item: DVTModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.dvtManagementService.DM_DVT_Insert(item).subscribe((res) => {
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
  create(item: DVTModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.dvtManagementService.DM_DVT_Insert(item).subscribe((res) => {
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

  update(item: DVTModel) {
    this.isLoadingSubmit$.next(true);
    this.dvtManagementService.UpdateDVT(item).subscribe((res) => {
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
    if (this.item.IdDVT === 0) {
      const empty = new DVTModel();
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
