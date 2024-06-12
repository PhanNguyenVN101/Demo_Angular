import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  HostListener,
  Inject,
  OnDestroy,
  OnInit,
  EventEmitter,
  Output,
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
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
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
import { LoaiMatHangManagementService } from "../Services/loaimathang-management.service";
import { LoaiMatHangModel } from "../Model/loaimathang-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";
import { debug } from "console";

export function conditionalValidator(
  predicate: () => boolean,
  validator: ValidatorFn
): ValidatorFn {
  return (formControl: AbstractControl): ValidationErrors | null => {
    if (!formControl.parent) {
      return null;
    }
    return predicate() ? validator(formControl) : null;
  };
}
@Component({
  selector: "app-loaimathang-management-fill-dialog",
  templateUrl: "./loaimathang-management-fill-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoaiMatHangManagementFillDialogComponent
  implements OnInit, OnDestroy
{
  item: LoaiMatHangModel = new LoaiMatHangModel();
  // itemcha: LoaiMatHangModel = new LoaiMatHangModel();
  // itemkho: LoaiMatHangModel = new LoaiMatHangModel();
  isLoading;
  formGroup: FormGroup;
  khoFilters: LoaiMatHangModel[] = [];
  loaiMHFilters: LoaiMatHangModel[] = [];
  private subscriptions: Subscription[] = [];
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<LoaiMatHangManagementFillDialogComponent>,
    private fb: FormBuilder,
    public loaimathangManagementService: LoaiMatHangManagementService,
    private changeDetect: ChangeDetectorRef,
    private layoutUtilsService: LayoutUtilsService,
    public general: GeneralService,
    public authService: AuthService,
    public datepipe: DatePipe,
    public dialog: MatDialog,
    private translateService: TranslateService,
    private changeDetector: ChangeDetectorRef
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
    this.item.IdLMH = this.data.item.IdLMH;
    this.loadForm();
    const sb = this.loaimathangManagementService.isLoading$.subscribe((res) => {
      this.isLoading = res;
    });
    this.subscriptions.push(sb);
    this.loadInitDataUpdate();

    const add = this.loaimathangManagementService
      .DM_Kho_List()
      .subscribe((res: ResultModel<LoaiMatHangModel>) => {
        if (res && res.status === 1) {
          this.khoFilters = res.data;
          this.KhofilterForm = new FormControl(this.khoFilters[0].IdKho);
          this.isInitData.next(true);
        }
      });
    this.subscriptions.push(add);
    this.loadInitData();

    const addLMHC = this.loaimathangManagementService
      .LoaiMatHangCha_List()
      .subscribe((res: ResultModel<LoaiMatHangModel>) => {
        if (res && res.status === 1) {
          this.loaiMHFilters = res.data;
          this.loaiMHfilterForm = new FormControl(
            this.loaiMHFilters[0].IdLMHParent
          );
          this.isInitData.next(true);
        }
      });
    this.subscriptions.push(addLMHC);
    this.loadInitDataLoaiMHCHA();
    this.checkboxes.valueChanges.subscribe((checkboxValues) => {
      this.toggleFields(checkboxValues);
    });

    this.toggleFields(this.checkboxes.value);
  }
  loadInitDataUpdate() {
    debugger;
    if (this.item.IdLMH !== 0) {
      const sbGet = this.loaimathangManagementService
        .GetLoaiMHID(this.item.IdLMH)
        .pipe(
          tap((res: ResultObjModel<LoaiMatHangModel>) => {
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
  loadInitData() {
    if (this.item.IdKho !== 0) {
      const sbGet = this.loaimathangManagementService
        .GetKhoID(this.item.IdKho)
        .pipe(
          tap((res: ResultObjModel<LoaiMatHangModel>) => {
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

  loadInitDataLoaiMHCHA() {
    if (this.item.IdLMHParent !== 0) {
      const sbGet = this.loaimathangManagementService
        .GetKhoID(this.item.IdLMHParent)
        .pipe(
          tap((res: ResultObjModel<LoaiMatHangModel>) => {
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
  onFileSelected(event: any) {
    const selectedFile = event.target.files[0];
    if (selectedFile) {
      const url = window.webkitURL.createObjectURL(selectedFile);
      console.log(url);
      this.item.HinhAnh = url;
    }
  }

  onSelectionChangeKho(event: any) {
    const selectedValue = event.target.value;
    this.item.IdKho = selectedValue;
  }
  onSelectionChangeCha(event: any) {
    const selectedValue = event.target.value;
    this.item.IdLMHParent = selectedValue;
  }

  loadForm() {
    this.formGroup = this.fb.group({
      tenloaimathang: [
        "",
        Validators.compose([
          Validators.maxLength(500),
          conditionalValidator(
            () => this.checkboxes.at(0).value,
            Validators.required
          ),
        ]),
      ],
      loaimathangcha: [
        this.item.IdLMHParent == null ? "0" : this.item.IdLMHParent.toString(),
        Validators.compose([
          conditionalValidator(
            () => this.checkboxes.at(1).value,
            Validators.required
          ),
        ]),
      ],
      douutien: [
        0,
        Validators.compose([
          Validators.pattern(/^-?(0|[0-9]\d*)?$/),
          Validators.maxLength(50),
          conditionalValidator(
            () => this.checkboxes.at(2).value,
            Validators.required
          ),
        ]),
      ],
      mota: [
        "",
        Validators.compose([
          Validators.maxLength(500),
          conditionalValidator(
            () => this.checkboxes.at(3).value,
            Validators.required
          ),
        ]),
      ],
      checkboxes: this.fb.array([false, false, false, false]),
    });
  }

  setValueToForm(model: LoaiMatHangModel) {
    this.formGroup.controls.tenloaimathang.setValue(model.TenLMH);
    this.formGroup.controls.loaimathangcha.setValue(model.IdLMHParent);
    this.formGroup.controls.mota.setValue(model.Mota);
    this.formGroup.controls.douutien.setValue(model.DoUuTien);
  }
  get checkboxes(): FormArray {
    return this.formGroup.get("checkboxes") as FormArray;
  }

  toggleFields(checkboxValues: boolean[]) {
    const controlNames = [
      "tenloaimathang",
      "loaimathangcha",
      "douutien",
      "mota",
    ];

    controlNames.forEach((controlName, index) => {
      const control = this.formGroup.get(controlName);
      if (control) {
        if (checkboxValues[index]) {
          control.enable();
          // To ensure validators are applied correctly when enabled
          control.updateValueAndValidity();
        } else {
          control.disable();
          // To ensure no validators are applied when disabled
          control.updateValueAndValidity();
        }
      }
    });
  }

  private prepareData(): LoaiMatHangModel {
    let model = new LoaiMatHangModel();
    model.empty();
    model.TenLMH = this.formGroup.controls.tenloaimathang.value;
    model.IdLMHParent = this.formGroup.controls.loaimathangcha.value;
    model.Mota = this.formGroup.controls.mota.value;
    model.DoUuTien = this.formGroup.controls.douutien.value;
    model.isFilter = this.checkboxes.value;
    return model;
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
  Loc() {
    debugger;
    const model = this.prepareData();
    if (!model.isFilter.every((filter) => !filter)) {
      if (this.formGroup.valid) {
        this.filterItem(model);
      } else {
        this.validateAllFormFields(this.formGroup);
      }
    } else {
      const message = this.translateService.instant("ERROR.NONEDATAFILLTER");
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
  }
  public filterResult: LoaiMatHangModel[] = [];
  filterItem(model: LoaiMatHangModel) {
    debugger;
    this.isLoadingSubmit$.next(true);
    this.loaimathangManagementService.FilterLMH(model).subscribe((result) => {
      this.isLoadingSubmit$.next(false);
      if (result && result.status === 1) {
        this.filterResult = result;
        this.dialogRef.close();
        this.changeDetector.detectChanges();
      } else {
        const message = this.translateService.instant("ERROR.FILTER_FAILED");
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
  onChangeNumber(event: any) {
    const kq = event.target.value.replace(/^0+(?=\d)/, "");
    this.formGroup.get("douutien").setValue(kq);
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
      res ? this.dialogRef.close() : undefined;
    });
  }

  @HostListener("window:beforeunload", ["$event"])
  beforeunloadHandler(e) {
    e.preventDefault(); //for Firefox
    return (e.returnValue = ""); //for Chorme
  }
}
