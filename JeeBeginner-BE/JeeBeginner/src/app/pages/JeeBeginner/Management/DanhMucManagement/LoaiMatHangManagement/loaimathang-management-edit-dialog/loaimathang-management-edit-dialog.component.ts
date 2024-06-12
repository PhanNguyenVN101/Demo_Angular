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
import { DomSanitizer, SafeUrl } from "@angular/platform-browser";
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
import { environment } from "src/environments/environment";
import { LoaiMatHangModel } from "../Model/loaimathang-management.model";
import { TranslateService } from "@ngx-translate/core";
import { GeneralService } from "../../../../_core/services/general.service";
import { debug } from "console";
import { ElementRef, ViewChild } from "@angular/core";

@Component({
  selector: "app-loaimathang-management-edit-dialog",
  templateUrl: "./loaimathang-management-edit-dialog.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoaiMatHangManagementEditDialogComponent
  implements OnInit, OnDestroy
{
  item: LoaiMatHangModel = new LoaiMatHangModel();
  itemslmh: LoaiMatHangModel[] = [];
  // itemcha: LoaiMatHangModel = new LoaiMatHangModel();
  // itemkho: LoaiMatHangModel = new LoaiMatHangModel();
  isLoading;
  formGroup: FormGroup;
  imageUrl: SafeUrl;
  khoFilters: LoaiMatHangModel[] = [];
  loaiMHFilters: LoaiMatHangModel[] = [];
  private subscriptions: Subscription[] = [];
  file: File;
  selectedImageUrl: string;
  src: string;
  KhofilterForm: FormControl;
  loaiMHfilterForm: FormControl;
  isInitData: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoadingSubmit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<LoaiMatHangManagementEditDialogComponent>,
    private fb: FormBuilder,
    private sanitizer: DomSanitizer,
    public loaimathangManagementService: LoaiMatHangManagementService,
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
    this.loaimathangManagementService.items$.subscribe((data) => {
      this.itemslmh = data;
    });
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
    debugger;
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
  getImageUrl(imageName: string): string {
    debugger;
    return `${environment.ApiUrlRoot}/Images/${imageName}`;
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
  selectedImages: { name: string; url: string }[] = [];

  async onFileSelected(event: any) {
    const files = event.target.files;

    if (files && files.length > 0) {
      for (const file of files) {
        const imageExists = this.selectedImages.some(
          (image) => image.name === file.name
        );

        if (!imageExists) {
          try {
            // for (let i = 0; i < 2; i++) {
            //   this.loaimathangManagementService.uploadImage(file);
            // }
            this.loaimathangManagementService.uploadImage(file);
          } catch {
            console.log("Error Upload File");
          }
          this.selectedImages.push({
            name: file.name,
            url: this.getImageUrl(file.name),
          });
        } else {
          const message = this.translateService.instant("ERROR.ANHBITRUNG");
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
      }
    }
  }

  removeImage(index: number) {
    this.selectedImages.splice(index, 1);
  }
  onChangeNumber(event: any) {
    const kq = event.target.value.replace(/^0+(?=\d)/, "");
    this.formGroup.get("douutien").setValue(kq);
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
      tenloaimathang: ["", Validators.compose([Validators.required])],
      loaimathangcha: [
        this.item.IdLMHParent == null ? "0" : this.item.IdLMHParent.toString(),
      ],
      kho: [this.item.IdKho == null ? "0" : this.item.IdKho.toString()],
      douutien: [
        0,
        Validators.compose([
          Validators.required,
          Validators.pattern(/^-?(0|[0-9]\d*)?$/),
          Validators.maxLength(50),
        ]),
      ],
      mota: ["", Validators.compose([Validators.maxLength(500)])],
    });
  }
  createNew(item: LoaiMatHangModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.loaimathangManagementService
        .DM_LoaiMatHang_Insert(item)
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
  LuuVaThemMoi() {
    if (this.formGroup.valid) {
      if (this.selectedImages.length > 1) {
        const message = this.translateService.instant("ERROR.LUUHINHANH");
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
      const model = this.prepareData();
      const exists = this.itemslmh.some(
        (item) => item.TenLMH.toLowerCase() === model.TenLMH.toLowerCase()
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
      this.item.IdLMH === 0 ? this.createNew(model) : this.update(model);
    } else {
      this.validateAllFormFields(this.formGroup);
    }
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
  setValueToForm(model: LoaiMatHangModel) {
    debugger;
    if (this.item.IdLMH !== 0) this.Visible = false;
    this.formGroup.controls.tenloaimathang.setValue(model.TenLMH);
    this.formGroup.controls.loaimathangcha.setValue(model.IdLMHParent);
    this.formGroup.controls.mota.setValue(model.Mota);
    this.formGroup.controls.douutien.setValue(model.DoUuTien);
    this.formGroup.controls.kho.setValue(model.IdKho);
    if (model.HinhAnh !== "") {
      this.selectedImages.push({
        url: this.getImageUrl(model.HinhAnh),
        name: model.HinhAnh,
      });
    }
  }

  private prepareData(): LoaiMatHangModel {
    debugger;
    let model = new LoaiMatHangModel();
    model.empty();
    model.TenLMH = this.formGroup.controls.tenloaimathang.value;
    model.IdLMHParent = this.formGroup.controls.loaimathangcha.value;
    model.IdKho = this.formGroup.controls.kho.value;
    model.Mota = this.formGroup.controls.mota.value;
    model.DoUuTien = this.formGroup.controls.douutien.value;
    model.IdLMH = this.item.IdLMH;
    this.selectedImages.length === 0
      ? (model.HinhAnh = "")
      : (model.HinhAnh = this.selectedImages[0].name);

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
    this.selectedImages.splice(0, this.selectedImages.length);
  }
  getTitle() {
    if (this.item.IdLMH === 0) {
      return this.translateService.instant("COMMOM.THEMMOI");
    }
    return this.translateService.instant("COMMOM.CAPNHAT");
  }
  Luu() {
    debugger;
    if (this.formGroup.valid) {
      if (this.selectedImages.length > 1) {
        const message = this.translateService.instant("ERROR.LUUHINHANH");
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
      const model = this.prepareData();
      const exists = this.itemslmh.some(
        (item) => item.TenLMH.toLowerCase() === model.TenLMH.toLowerCase()
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

      this.item.IdLMH === 0 ? this.create(model) : this.update(model);
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

  create(item: LoaiMatHangModel) {
    this.isLoadingSubmit$.next(true);
    if (
      !this.authService.currentUserValue.IsMasterAccount ||
      this.authService.currentUserValue.IsMasterAccount
    )
      this.loaimathangManagementService
        .DM_LoaiMatHang_Insert(item)
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

  update(item: LoaiMatHangModel) {
    this.isLoadingSubmit$.next(true);
    this.loaimathangManagementService
      .UpdateLoaiMatHang(item)
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

  checkDataBeforeClose(): boolean {
    const model = this.prepareData();
    if (this.item.IdLMH === 0) {
      const empty = new LoaiMatHangModel();
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
