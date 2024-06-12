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
  selector: "app-dvt-management-detail-dialog",
  templateUrl: "./dvt-management-detail-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DVTManagementDetailDialogComponent implements OnInit, OnDestroy {
  item: DVTModel = new DVTModel();

  isLoading;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<DVTManagementDetailDialogComponent>,
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
    this.item.empty();

    this.item.IdDVT = this.data.item.IdDVT;
    this.loadForm();
    const sb = this.dvtManagementService.isLoading$.subscribe((res) => {
      this.isLoading = res;
    });
    this.subscriptions.push(sb);

    this.loadInitData();
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
      iddvt: [
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
    this.formGroup.controls.tendvt.setValue(model.TenDVT);
    this.formGroup.controls.iddvt.setValue(model.IdDVT);
  }

  private prepareData(): DVTModel {
    let model = new DVTModel();
    model.empty();
    model.TenDVT = this.formGroup.controls.tendvt.value;
    model.IdDVT = this.item.IdDVT;

    return model;
  }

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
      res ? this.dialogRef.close() : undefined;
    });
  }

  @HostListener("window:beforeunload", ["$event"])
  beforeunloadHandler(e) {
    e.preventDefault(); //for Firefox
    return (e.returnValue = ""); //for Chorme
  }
}
