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
import { XuatXuManagementService } from "../Services/xuatxu-management.service";
import { XuatXuModel } from "../Model/xuatxu-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";
import { debug } from "console";

@Component({
  selector: "app-xuatxu-management-detail-dialog",
  templateUrl: "./xuatxu-management-detail-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class XuatXuManagementDetailDialogComponent
  implements OnInit, OnDestroy
{
  item: XuatXuModel = new XuatXuModel();
  // itemcha: LoaiMatHangModel = new LoaiMatHangModel();
  // itemkho: LoaiMatHangModel = new LoaiMatHangModel();
  isLoading;
  formGroup: FormGroup;
  // khoFilters: XuatXuModel[] = [];
  // loaiMHFilters: XuatXuModel[] = [];
  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<XuatXuManagementDetailDialogComponent>,
    private fb: FormBuilder,
    public xuatxuManagementService: XuatXuManagementService,
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
    this.item.IdXuatXu = this.data.item.IdXuatXu;
    this.loadForm();
    const sb = this.xuatxuManagementService.isLoading$.subscribe((res) => {
      this.isLoading = res;
    });
    this.subscriptions.push(sb);
    // const add = this.loaimathangManagementService
    //   .DM_Kho_List()
    //   .subscribe((res: ResultModel<XuatXuModel>) => {
    //     if (res && res.status === 1) {
    //       this.khoFilters = res.data;
    //       this.KhofilterForm = new FormControl(this.khoFilters[0].IdXuatXu);
    //       this.isInitData.next(true);
    //     }
    //   });
    // this.subscriptions.push(add);
    this.loadInitData();
  }

  loadInitData() {
    if (this.item.IdXuatXu !== 0) {
      const sbGet = this.xuatxuManagementService
        .GetXuatXuID(this.item.IdXuatXu)
        .pipe(
          tap((res: ResultObjModel<XuatXuModel>) => {
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
      tenxuatxu: [
        "",
        Validators.compose([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(50),
        ]),
      ],
      idxuatxu: [
        "",
        Validators.compose([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(50),
        ]),
      ],
    });
  }

  setValueToForm(model: XuatXuModel) {
    this.formGroup.controls.tenxuatxu.setValue(model.TenXuatXu);
    this.formGroup.controls.idxuatxu.setValue(model.IdXuatXu);
    // this.formGroup.controls.loaimathangcha.setValue(model.IdLMHParent);
    // this.formGroup.controls.mota.setValue(model.Mota);
    // this.formGroup.controls.douutien.setValue(model.DoUuTien);
    // this.item.IdKho = model.IdKho;
    // this.item.IdLMHParent = model.IdLMHParent;
    // this.item.IdLMH = model.IdLMH;
    //this.formGroup.controls.kho.setValue(model.IdKho);
  }

  // getTitle() {
  //   if (this.item.RowId === 0) {
  //     return this.translateService.instant(
  //       "ACCOUNTROLEMANAGEMENT.TAOTAIKHOANSUDUNG"
  //     );
  //   }
  //   return (
  //     this.translateService.instant("ACCOUNTROLEMANAGEMENT.SUATAIKHOAN") +
  //     " " +
  //     this.data.item.Username
  //   );
  // }

  private prepareData(): XuatXuModel {
    let model = new XuatXuModel();
    model.empty();
    model.TenXuatXu = this.formGroup.controls.tenxuatxu.value;
    model.IdXuatXu = this.item.IdXuatXu;
    //model.IdLMHParent = this.formGroup.controls.loaimathangcha.value;
    //model.IdLMHParent = this.item.IdLMHParent;
    //model.IdKho = this.formGroup.controls.kho.value;
    //model.IdKho = this.item.IdKho;
    //model.Mota = this.formGroup.controls.mota.value;
    //model.DoUuTien = this.formGroup.controls.douutien.value;
    //model.IdLMH = this.item.IdLMH;
    //model.HinhAnh = this.item.HinhAnh;
    //model.Mobile = this.formGroup.controls.sodienthoai.value;
    //model.Note = this.formGroup.controls.ghichu.value;
    // model.PartnerId = this.item.PartnerId;
    // model.Password = this.formGroup.controls.password.value;
    // model.Username = this.formGroup.controls.username.value;
    return model;
  }

  Luu() {
    const model = this.prepareData();
    this.item.IdXuatXu === 0 ? this.create(model) : this.update(model);
    // if (this.formGroup.valid) {
    //   if (
    //     this.formGroup.controls.password.value !==
    //     this.formGroup.controls.repassword.value
    //   ) {
    //     const message = this.translateService.instant(
    //       "ERROR.MATKHAUKHONGTRUNGKHOP"
    //     );
    //     this.layoutUtilsService.showActionNotification(
    //       message,
    //       MessageType.Read,
    //       999999999,
    //       true,
    //       false,
    //       3000,
    //       "top",
    //       0
    //     );
    //     return;
    //   }
    //   const model = this.prepareData();
    //   this.item.IdLMH === 0 ? this.create(model) : this.update(model);
    // } else {
    //   this.validateAllFormFields(this.formGroup);
    // }
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

  create(item: XuatXuModel) {
    this.isLoadingSubmit$.next(true);
    if (this.authService.currentUserValue.IsMasterAccount)
      this.xuatxuManagementService.DM_XuatXu_Insert(item).subscribe((res) => {
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

  update(item: XuatXuModel) {
    this.isLoadingSubmit$.next(true);
    this.xuatxuManagementService.UpdateXuatXu(item).subscribe((res) => {
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
    if (this.item.IdXuatXu === 0) {
      const empty = new XuatXuModel();
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
