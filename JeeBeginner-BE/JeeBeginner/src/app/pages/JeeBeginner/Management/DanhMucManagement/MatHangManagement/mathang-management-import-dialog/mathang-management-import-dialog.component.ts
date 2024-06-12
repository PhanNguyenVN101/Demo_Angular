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
import { MatHangManagementService } from "../Services/mathang-management.service";
import { MatHangModel } from "../Model/mathang-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";

@Component({
  selector: "app-mathang-management-import-dialog",
  templateUrl: "./mathang-management-import-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MatHangManagementImportDialogComponent
  implements OnInit, OnDestroy
{
  item: MatHangModel = new MatHangModel();
  itemkho: MatHangModel = new MatHangModel();
  isLoading;
  isExpanded = false;
  selectedFile: File | null = null;
  formGroup: FormGroup;
  khoFilters: MatHangModel[] = [];
  selectedFileName: string | null = null;
  loaiMHFilters: MatHangModel[] = [];
  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<MatHangManagementImportDialogComponent>,
    private fb: FormBuilder,
    public mathangManagementService: MatHangManagementService,
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
    this.item.IdMH = this.data.item.IdMH;
    const sb = this.mathangManagementService.isLoading$.subscribe((res) => {
      this.isLoading = res;
    });
    this.subscriptions.push(sb);
  }

  reset() {
    debugger;
    const values = new MatHangModel();
    values.IdMH = this.formGroup.get("IdMH").value;
    values.MaHang = this.formGroup.get("MaHang").value;
    values.TenMatHang = this.formGroup.get("TenMatHang").value;
    values.TenOnSite = this.formGroup.get("TenOnSite").value;
    values.GiaBan = this.formGroup.get("GiaBan").value;
    values.IdLMH = this.formGroup.get("IdLMH").value;
    values.IdDVT = this.formGroup.get("IdDVT").value;
    values.Mota = this.formGroup.get("Mota").value;

    this.mathangManagementService.setitemstemp(values);

    this.dialogRef.close();
  }

  toggleExpand() {
    this.isExpanded = !this.isExpanded;
  }

  create(item: MatHangModel, actionType: string) {
    this.isLoadingSubmit$.next(true);
    if (this.authService.currentUserValue.IsMasterAccount)
      this.mathangManagementService.DM_MatHang_Insert(item).subscribe((res) => {
        this.isLoadingSubmit$.next(false);
        if (res && res.status === 1) {
          if (actionType === "saveAndContinue") {
            this.dialogRef.close(res);

            const item = new MatHangModel();
            item.empty();
            let saveMessageTranslateParam = "";
            saveMessageTranslateParam += "Thêm thành công";
            const saveMessage = this.translate.instant(
              saveMessageTranslateParam
            );
            const messageType = MessageType.Create;
            const dialogRef = this.dialog.open(
              MatHangManagementImportDialogComponent,
              {
                data: { item: item },
                width: "900px",
                disableClose: true,
              }
            );
            dialogRef.afterClosed().subscribe((res) => {
              if (!res) {
                this.mathangManagementService.fetch();
              } else {
                this.layoutUtilsService.showActionNotification(
                  saveMessage,
                  messageType,
                  4000,
                  true,
                  false
                );
                this.mathangManagementService.fetch();
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
      this.mathangManagementService.setitemstemp(new MatHangModel());
      res ? this.dialogRef.close() : undefined;
    });
  }

  @HostListener("window:beforeunload", ["$event"])
  beforeunloadHandler(e) {
    e.preventDefault(); //for Firefox
    return (e.returnValue = "");
  }

  downloadTemplate(): void {
    this.mathangManagementService.exportToExcel().subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = "mat-hang.xlsx";
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
      this.mathangManagementService
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
