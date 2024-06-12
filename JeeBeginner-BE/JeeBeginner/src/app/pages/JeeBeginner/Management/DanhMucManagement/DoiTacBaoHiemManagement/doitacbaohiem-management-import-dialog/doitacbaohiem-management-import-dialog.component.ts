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
import { DoiTacBaoHiemManagementService } from "../Services/doitacbaohiem-management.service";
import { DoiTacBaoHiemModel } from "../Model/doitacbaohiem-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";

@Component({
  selector: "app-doitacbaohiem-management-import-dialog",
  templateUrl: "./doitacbaohiem-management-import-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DoiTacBaoHiemManagementImportDialogComponent
  implements OnInit, OnDestroy
{
  item: DoiTacBaoHiemModel = new DoiTacBaoHiemModel();
  itemkho: DoiTacBaoHiemModel = new DoiTacBaoHiemModel();
  isLoading;
  isExpanded = false;
  selectedFile: File | null = null;
  formGroup: FormGroup;
  khoFilters: DoiTacBaoHiemModel[] = [];
  selectedFileName: string | null = null;
  loaiMHFilters: DoiTacBaoHiemModel[] = [];
  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<DoiTacBaoHiemManagementImportDialogComponent>,
    private fb: FormBuilder,
    public doitacbaohiemManagementService: DoiTacBaoHiemManagementService,
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
    this.item.Id_DV = this.data.item.Id_DV;
    this.loadForm();
    const sb = this.doitacbaohiemManagementService.isLoading$.subscribe(
      (res) => {
        this.isLoading = res;
      }
    );
    this.subscriptions.push(sb);
    this.loadInitData();
  }
  loadInitData() {
    if (this.item.Id_DV !== 0) {
      const sbGet = this.doitacbaohiemManagementService
        .GetDoiTacBaoHiemID(this.item.Id_DV)
        .pipe(
          tap((res: ResultObjModel<DoiTacBaoHiemModel>) => {
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
    debugger;
    if (this.doitacbaohiemManagementService.itemstemp$) {
      const itemstemp = this.doitacbaohiemManagementService.itemstemp$;

      this.formGroup = this.fb.group({
        TenDonVi: [
          itemstemp.TenDonVi,
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(50),
          ]),
        ],
        DiaChi: [
          itemstemp.DiaChi,
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(50),
          ]),
        ],
        SoDT: [
          itemstemp.SoDT,
          Validators.compose([
            Validators.required,
            Validators.pattern(/^-?(0|[0-9]\d*)?$/),
            Validators.maxLength(50),
          ]),
        ],
        NguoiLienHe: [
          itemstemp.NguoiLienHe,
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(50),
          ]),
        ],
        GhiChu: [
          itemstemp.GhiChu,
          Validators.compose([Validators.maxLength(500)]),
        ],
      });
    } else {
      this.formGroup = this.fb.group({
        TenDonVi: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(50),
          ]),
        ],
        DiaChi: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(50),
          ]),
        ],
        SoDT: [
          "",
          Validators.compose([
            Validators.required,
            Validators.pattern(/^-?(0|[0-9]\d*)?$/),
            Validators.maxLength(50),
          ]),
        ],
        NguoiLienHe: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(50),
          ]),
        ],
        GhiChu: ["", Validators.compose([Validators.maxLength(500)])],
        ChonFileDuLieu: ["", Validators.compose([Validators.required])],
      });
    }
  }

  setValueToForm(model: DoiTacBaoHiemModel) {
    this.formGroup.controls.TenDonVi.setValue(model.TenDonVi);
    this.formGroup.controls.DiaChi.setValue(model.DiaChi);
    this.formGroup.controls.SoDT.setValue(model.SoDT);
    this.formGroup.controls.NguoiLienHe.setValue(model.NguoiLienHe);
    this.formGroup.controls.GhiChu.setValue(model.GhiChu);
    //this.formGroup.controls.nhanhieucha.setValue(model.IdLMHParent);
    //this.formGroup.controls.kho.setValue(model.IdKho);
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
      this.doitacbaohiemManagementService.setitemstemp(
        new DoiTacBaoHiemModel()
      );
      res ? this.dialogRef.close() : undefined;
    });
  }

  @HostListener("window:beforeunload", ["$event"])
  beforeunloadHandler(e) {
    e.preventDefault(); //for Firefox
    return (e.returnValue = ""); //for Chorme
  }

  downloadTemplate(): void {
    this.doitacbaohiemManagementService.exportToExcel().subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = "doi-tac-bao-hiem-mau.xlsx";
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
      this.doitacbaohiemManagementService
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
