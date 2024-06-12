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
  selector: "app-lydotanggiamtaisan-management-import-dialog",
  templateUrl: "./lydotanggiamtaisan-management-import-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LyDoTangGiamTaiSanManagementImportDialogComponent
  implements OnInit, OnDestroy
{
  item: LyDoTangGiamTaiSanModel = new LyDoTangGiamTaiSanModel();
  itemkho: LyDoTangGiamTaiSanModel = new LyDoTangGiamTaiSanModel();
  isLoading;
  isExpanded = false;
  selectedFile: File | null = null;
  formGroup: FormGroup;
  khoFilters: LyDoTangGiamTaiSanModel[] = [];
  selectedFileName: string | null = null;
  loaiMHFilters: LyDoTangGiamTaiSanModel[] = [];
  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<LyDoTangGiamTaiSanManagementImportDialogComponent>,
    private fb: FormBuilder,
    public lydotanggiamtaisanManagementService: LyDoTangGiamTaiSanManagementService,
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
    this.item.IdRow = this.data.item.IdRow;
    const sb = this.lydotanggiamtaisanManagementService.isLoading$.subscribe(
      (res) => {
        this.isLoading = res;
      }
    );
    this.subscriptions.push(sb);
  }

  reset() {
    debugger;
    const values = new LyDoTangGiamTaiSanModel();
    values.IdRow = this.formGroup.get("IdRow").value;
    values.LoaiTangGiam = this.formGroup.get("LoaiTangGiam").value;
    values.TenTangGiam = this.formGroup.get("TenTangGiam").value;
    values.MaTangGiam = this.formGroup.get("MaTangGiam").value;
    values.TrangThai = this.formGroup.get("TrangThai").value;

    this.lydotanggiamtaisanManagementService.setitemstemp(values);

    this.dialogRef.close();
  }

  toggleExpand() {
    this.isExpanded = !this.isExpanded;
  }

  create(item: LyDoTangGiamTaiSanModel, actionType: string) {
    this.isLoadingSubmit$.next(true);
    if (this.authService.currentUserValue.IsMasterAccount)
      this.lydotanggiamtaisanManagementService
        .DM_LyDoTangGiamTaiSan_Insert(item)
        .subscribe((res) => {
          this.isLoadingSubmit$.next(false);
          if (res && res.status === 1) {
            if (actionType === "saveAndContinue") {
              this.dialogRef.close(res);

              const item = new LyDoTangGiamTaiSanModel();
              item.empty();
              let saveMessageTranslateParam = "";
              saveMessageTranslateParam += "Thêm thành công";
              const saveMessage = this.translate.instant(
                saveMessageTranslateParam
              );
              const messageType = MessageType.Create;
              const dialogRef = this.dialog.open(
                LyDoTangGiamTaiSanManagementImportDialogComponent,
                {
                  data: { item: item },
                  width: "900px",
                  disableClose: true,
                }
              );
              dialogRef.afterClosed().subscribe((res) => {
                if (!res) {
                  this.lydotanggiamtaisanManagementService.fetch();
                } else {
                  this.layoutUtilsService.showActionNotification(
                    saveMessage,
                    messageType,
                    4000,
                    true,
                    false
                  );
                  this.lydotanggiamtaisanManagementService.fetch();
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
      this.lydotanggiamtaisanManagementService.setitemstemp(
        new LyDoTangGiamTaiSanModel()
      );
      res ? this.dialogRef.close() : undefined;
    });
  }

  @HostListener("window:beforeunload", ["$event"])
  beforeunloadHandler(e) {
    e.preventDefault(); //for Firefox
    return (e.returnValue = "");
  }

  downloadTemplate(): void {
    this.lydotanggiamtaisanManagementService
      .exportToExcel()
      .subscribe((blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = "ly-do-tang-giam-tai-san.xlsx";
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
      this.lydotanggiamtaisanManagementService
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
