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
  selector: "app-loaitaisan-management-import-dialog",
  templateUrl: "./loaitaisan-management-import-dialog.component.html",
  //styleUrls: ["./StylePhongTo.component.css"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoaiTaiSanManagementImportDialogComponent
  implements OnInit, OnDestroy
{
  item: LoaiTaiSanModel = new LoaiTaiSanModel();
  itemkho: LoaiTaiSanModel = new LoaiTaiSanModel();
  isLoading;
  isExpanded = false;
  selectedFile: File | null = null;
  formGroup: FormGroup;
  khoFilters: LoaiTaiSanModel[] = [];
  selectedFileName: string | null = null;
  loaiMHFilters: LoaiTaiSanModel[] = [];
  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<LoaiTaiSanManagementImportDialogComponent>,
    private fb: FormBuilder,
    public loaitaisanManagementService: LoaiTaiSanManagementService,
    private changeDetect: ChangeDetectorRef,
    private layoutUtilsService: LayoutUtilsService,
    public general: GeneralService,
    public authService: AuthService,
    private translate: TranslateService,
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
    const sb = this.loaitaisanManagementService.isLoading$.subscribe((res) => {
      this.isLoading = res;
    });
    this.subscriptions.push(sb);
    //this.loadInitData();
  }
  // loadInitData() {
  //   if (this.item.IdLoaiTS !== 0) {
  //     const sbGet = this.loaitaisanManagementService
  //       .GetLoaiTaiSanID(this.item.IdLoaiTS)
  //       .pipe(
  //         tap((res: ResultObjModel<LoaiTaiSanModel>) => {
  //           if (res.status === 1) {
  //             this.item = res.data;
  //             this.setValueToForm(res.data);
  //           }
  //         })
  //       )
  //       .subscribe();
  //     this.subscriptions.push(sbGet);
  //   }
  // }

  reset() {
    debugger;
    const values = new LoaiTaiSanModel();
    values.MaLoai = this.formGroup.get("MaLoai").value;
    values.TenLoai = this.formGroup.get("TenLoai").value;
    values.TrangThai = this.formGroup.get("TrangThai").value;

    this.loaitaisanManagementService.setitemstemp(values);

    this.dialogRef.close();
  }
  // loadForm() {
  //   debugger;
  //   if (this.loaitaisanManagementService.itemstemp$) {
  //     const itemstemp = this.loaitaisanManagementService.itemstemp$;

  //     this.formGroup = this.fb.group({
  //       MaLoai: [
  //         itemstemp.MaLoai,
  //         Validators.compose([
  //           Validators.required,
  //           Validators.minLength(3),
  //           Validators.maxLength(50),
  //         ]),
  //       ],
  //       TenLoai: [
  //         itemstemp.TenLoai,
  //         Validators.compose([
  //           Validators.required,
  //           Validators.minLength(3),
  //           Validators.maxLength(50),
  //         ]),
  //       ],
  //       SoDT: [
  //         itemstemp.TrangThai,
  //         Validators.compose([
  //           Validators.required,
  //           Validators.pattern(/^-?(0|[0-9]\d*)?$/),
  //           Validators.maxLength(50),
  //         ]),
  //       ],

  //     });
  //   } else {
  //     this.formGroup = this.fb.group({
  //       TenDonVi: [
  //         "",
  //         Validators.compose([
  //           Validators.required,
  //           Validators.minLength(3),
  //           Validators.maxLength(50),
  //         ]),
  //       ],
  //       DiaChi: [
  //         "",
  //         Validators.compose([
  //           Validators.required,
  //           Validators.minLength(3),
  //           Validators.maxLength(50),
  //         ]),
  //       ],
  //       SoDT: [
  //         "",
  //         Validators.compose([
  //           Validators.required,
  //           Validators.pattern(/^-?(0|[0-9]\d*)?$/),
  //           Validators.maxLength(50),
  //         ]),
  //       ],
  //       NguoiLienHe: [
  //         "",
  //         Validators.compose([
  //           Validators.required,
  //           Validators.minLength(3),
  //           Validators.maxLength(50),
  //         ]),
  //       ],
  //       GhiChu: ["", Validators.compose([Validators.maxLength(500)])],
  //     });
  //   }
  // }

  // loadForm() {
  //   this.formGroup = this.fb.group({
  //     TenDonVi: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(50)])],
  //     DiaChi: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(50)])],
  //     SoDT: ['', Validators.compose([Validators.required, Validators.pattern(/^-?(0|[0-9]\d*)?$/), Validators.maxLength(50)])],
  //     NguoiLienHe: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(50)])],
  //     GhiChu: ['', Validators.compose([Validators.maxLength(500)])],
  //   });
  // }

  // setValueToForm(model: LoaiTaiSanModel) {
  //   this.formGroup.controls.TenDonVi.setValue(model.TenDonVi);
  //   this.formGroup.controls.DiaChi.setValue(model.DiaChi);
  //   this.formGroup.controls.SoDT.setValue(model.SoDT);
  //   this.formGroup.controls.NguoiLienHe.setValue(model.NguoiLienHe);
  //   this.formGroup.controls.GhiChu.setValue(model.GhiChu);
  //   //this.formGroup.controls.nhanhieucha.setValue(model.IdLMHParent);
  //   //this.formGroup.controls.kho.setValue(model.IdKho);
  // }

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

  // private prepareData(): DoiTacBaoHiemModel {
  //   let model = new DoiTacBaoHiemModel();
  //   model.empty();
  //   model.Id_DV = this.item.Id_DV;
  //   model.TenDonVi = this.formGroup.controls.TenDonVi.value;
  //   model.DiaChi = this.formGroup.controls.DiaChi.value;
  //   model.SoDT = this.formGroup.controls.SoDT.value;
  //   model.NguoiLienHe = this.formGroup.controls.NguoiLienHe.value;
  //   model.GhiChu = this.formGroup.controls.GhiChu.value;
  //   return model;
  // }
  toggleExpand() {
    this.isExpanded = !this.isExpanded;
  }

  // savecontinute() {
  //   if (this.formGroup.valid) {
  //     const model = this.prepareData();
  //     this.item.Id_DV === 0
  //       ? this.create(model, "saveAndContinue")
  //       : this.update(model);
  //   } else {
  //     this.validateAllFormFields(this.formGroup);
  //   }
  // }
  // Luu() {
  //   if (this.formGroup.valid) {
  //     const model = this.prepareData();
  //     this.item.Id_DV === 0 ? this.create(model, "save") : this.update(model);
  //   } else {
  //     this.validateAllFormFields(this.formGroup);
  //   }
  // }
  // savecontinute(){
  //   debugger
  //   const model = this.prepareData();
  //   this.item.Id_DV === 0 ? this.create(model,'saveAndContinue') : this.update(model);
  // }
  // Luu() {
  //   debugger
  //   const model = this.prepareData();
  //   this.item.Id_DV === 0 ? this.create(model,'save') : this.update(model);
  //   // this.nhanhieuManagementService.setTempTennhanhieu('');
  //   // if (this.formGroup.valid) {
  //   //   if (
  //   //     this.formGroup.controls.password.value !==
  //   //     this.formGroup.controls.repassword.value
  //   //   ) {
  //   //     const message = this.translateService.instant(
  //   //       "ERROR.MATKHAUKHONGTRUNGKHOP"
  //   //     );
  //   //     this.layoutUtilsService.showActionNotification(
  //   //       message,
  //   //       MessageType.Read,
  //   //       999999999,
  //   //       true,
  //   //       false,
  //   //       3000,
  //   //       "top",
  //   //       0
  //   //     );
  //   //     return;
  //   //   }
  //   //   const model = this.prepareData();
  //   //   this.item.IdLMH === 0 ? this.create(model) : this.update(model);
  //   // } else {
  //   //   this.validateAllFormFields(this.formGroup);
  //   // }
  // }

  create(item: LoaiTaiSanModel, actionType: string) {
    this.isLoadingSubmit$.next(true);
    if (this.authService.currentUserValue.IsMasterAccount)
      this.loaitaisanManagementService
        .DM_LoaiTaiSan_Insert(item)
        .subscribe((res) => {
          this.isLoadingSubmit$.next(false);
          if (res && res.status === 1) {
            if (actionType === "saveAndContinue") {
              this.dialogRef.close(res);

              const item = new LoaiTaiSanModel();
              item.empty();
              let saveMessageTranslateParam = "";
              saveMessageTranslateParam += "Thêm thành công";
              const saveMessage = this.translate.instant(
                saveMessageTranslateParam
              );
              const messageType = MessageType.Create;
              const dialogRef = this.dialog.open(
                LoaiTaiSanManagementImportDialogComponent,
                {
                  data: { item: item },
                  width: "900px",
                  disableClose: true,
                }
              );
              dialogRef.afterClosed().subscribe((res) => {
                if (!res) {
                  this.loaitaisanManagementService.fetch();
                } else {
                  this.layoutUtilsService.showActionNotification(
                    saveMessage,
                    messageType,
                    4000,
                    true,
                    false
                  );
                  this.loaitaisanManagementService.fetch();
                }
              });
            } else {
              this.dialogRef.close(res);
            }
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
  // Luu() {
  //   debugger
  //   const model = this.prepareData();
  //   this.item.Id_DV === 0 ? this.create(model) : this.update(model);
  //   this.doitacbaohiemManagementService.setitemstemp(new DoiTacBaoHiemModel());
  //   // if (this.formGroup.valid) {
  //   //   if (
  //   //     this.formGroup.controls.password.value !==
  //   //     this.formGroup.controls.repassword.value
  //   //   ) {
  //   //     const message = this.translateService.instant(
  //   //       "ERROR.MATKHAUKHONGTRUNGKHOP"
  //   //     );
  //   //     this.layoutUtilsService.showActionNotification(
  //   //       message,
  //   //       MessageType.Read,
  //   //       999999999,
  //   //       true,
  //   //       false,
  //   //       3000,
  //   //       "top",
  //   //       0
  //   //     );
  //   //     return;
  //   //   }
  //   //   const model = this.prepareData();
  //   //   this.item.IdLMH === 0 ? this.create(model) : this.update(model);
  //   // } else {
  //   //   this.validateAllFormFields(this.formGroup);
  //   // }
  // }

  // validateAllFormFields(formGroup: FormGroup) {
  //   Object.keys(formGroup.controls).forEach((field) => {
  //     const control = formGroup.get(field);
  //     if (control instanceof FormControl) {
  //       control.markAsTouched({ onlySelf: true });
  //     } else if (control instanceof FormGroup) {
  //       this.validateAllFormFields(control);
  //     }
  //   });
  // }

  // create(item: DoiTacBaoHiemModel) {
  //   this.isLoadingSubmit$.next(true);
  //   if (this.authService.currentUserValue.IsMasterAccount)
  //     this.doitacbaohiemManagementService
  //       .DM_DoiTacBaoHiem_Insert(item)
  //       .subscribe((res) => {
  //         this.isLoadingSubmit$.next(false);
  //         if (res && res.status === 1) {
  //           this.dialogRef.close(res);
  //         } else {
  //           this.layoutUtilsService.showActionNotification(
  //             res.error.message,
  //             MessageType.Read,
  //             999999999,
  //             true,
  //             false,
  //             3000,
  //             "top",
  //             0
  //           );
  //         }
  //       });
  // }

  // update(item: DoiTacBaoHiemModel) {
  //   debugger;
  //   this.isLoadingSubmit$.next(true);
  //   this.doitacbaohiemManagementService
  //     .UpdateDoiTacBaoHiem(item)
  //     .subscribe((res) => {
  //       this.isLoadingSubmit$.next(false);
  //       if (res && res.status === 1) {
  //         this.dialogRef.close(res);
  //       } else {
  //         this.layoutUtilsService.showActionNotification(
  //           res.error.message,
  //           MessageType.Read,
  //           999999999,
  //           true,
  //           false,
  //           3000,
  //           "top",
  //           0
  //         );
  //       }
  //     });
  // }

  goBack() {
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
      this.loaitaisanManagementService.setitemstemp(new LoaiTaiSanModel());
      res ? this.dialogRef.close() : undefined;
    });
  }

  // checkDataBeforeClose(): boolean {
  //   const model = this.prepareData();
  //   if (this.item.IdLoaiTS === 0) {
  //     const empty = new LoaiTaiSanModel();
  //     empty.empty();
  //     return this.general.isEqual(empty, model);
  //   }
  //   return this.general.isEqual(model, this.item);
  // }

  @HostListener("window:beforeunload", ["$event"])
  beforeunloadHandler(e) {
    e.preventDefault(); //for Firefox
    return (e.returnValue = "");
  }

  downloadTemplate(): void {
    this.loaitaisanManagementService.exportToExcel().subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = "loai-tai-san.xlsx";
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    });
  }

  import(): void {
    debugger;
    this.isLoadingSubmit$.next(true);
    if (
      this.authService.currentUserValue.IsMasterAccount ||
      !this.authService.currentUserValue.IsMasterAccount
    )
      this.loaitaisanManagementService
        .importData(this.selectedFile)
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

  onFileChange(event: any): void {
    //const file = event.target.files[0];
    this.selectedFile = event.target.files[0] as File;
  }

  fileInput() {}
}
