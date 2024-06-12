import { NhomTaiSanManagementEditDialogComponent } from "../nhomtaisan-management-edit-dialog/nhomtaisan-management-edit-dialog.component";
import { SelectionModel } from "@angular/cdk/collections";
import { NhomTaiSanManagementImportDialogComponent } from "../nhomtaisan-management-import-dialog/nhomtaisan-management-import-dialog.component";
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  HostListener,
  OnDestroy,
  OnInit,
  ViewChild,
} from "@angular/core";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { TranslateService } from "@ngx-translate/core";
import { BehaviorSubject, merge, Subscription } from "rxjs";
import { debounceTime, distinctUntilChanged, tap } from "rxjs/operators";
import {
  LayoutUtilsService,
  MessageType,
} from "../../../../_core/utils/layout-utils.service";
import { DanhMucChungService } from "../../../../_core/services/danhmuc.service";
import { QueryParamsModelNew } from "../../../../_core/models/query-models/query-params.model";
import { DeleteEntityDialogComponent } from "../../../../_shared/delete-entity-dialog/delete-entity-dialog.component";
import { FormBuilder, FormGroup } from "@angular/forms";
import { TokenStorage } from "../../../../../../modules/auth/_services/token-storage.service";
import { NhomTaiSanManagementService } from "../Services/nhomtaisan-management.service";
import {
  GroupingState,
  PaginatorState,
  SortState,
} from "src/app/_metronic/shared/crud-table";
import { SubheaderService } from "src/app/_metronic/partials/layout";
// import { nhanhieuModels, NhomTaiSanDeleteModel } from '../Model/NhomTaiSan-management.model';
import { NhomTaiSanModel } from "../Model/nhomtaisan-management.model";
import { AuthService } from "src/app/modules/auth";
import {
  CdkDragStart,
  CdkDropList,
  moveItemInArray,
} from "@angular/cdk/drag-drop";
import { PartnerFilterDTO } from "../../../PartnerManagement/Model/partner-management.model";
import { ResultModel } from "../../../../_core/models/_base.model";
//import { NhomTaiSanManagementStatusDialogComponent } from '../NhomTaiSan-management-status-dialog/NhomTaiSan-management-status-dialog.component';

const COLOR_DANGHIEULUC = "#3699ff";
const COLOR_THANHLY = "#1bc5bd";
const COLOR_CHUAHIEULUC = "#ffa800";
const COLOR_HETHIEULUC = "#f64e60";

@Component({
  selector: "app-nhomtaisan-management-list",
  templateUrl: "./nhomtaisan-management-list.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NhomTaiSanManagementListComponent implements OnInit, OnDestroy {
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  Visible: boolean;

  selection = new SelectionModel<NhomTaiSanModel>(true, []);
  searchGroup: FormGroup;
  NhomTaiSansResult: NhomTaiSanModel[] = [];
  private subscriptions: Subscription[] = [];
  displayedColumns = ["stt", "MaNhom", "TenNhom", "TrangThai", "thaotac"];
  previousIndex: number;
  partnerFilters: PartnerFilterDTO[] = [];
  constructor(
    private changeDetect: ChangeDetectorRef,
    public nhomtaisanManagementService: NhomTaiSanManagementService,
    private translate: TranslateService,
    public subheaderService: SubheaderService,
    private layoutUtilsService: LayoutUtilsService,
    private tokenStorage: TokenStorage,
    public dialog: MatDialog,
    public danhmuc: DanhMucChungService,
    public auth: AuthService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.nhomtaisanManagementService.fetch();
    this.grouping = this.nhomtaisanManagementService.grouping;
    this.paginator = this.nhomtaisanManagementService.paginator;
    this.sorting = this.nhomtaisanManagementService.sorting;
    const sb = this.nhomtaisanManagementService.isLoading$.subscribe(
      (res) => (this.isLoading = res)
    );
    this.subscriptions.push(sb);
    const vi = this.nhomtaisanManagementService
      .IsReadOnlyPermitAccountRole("3700")
      .subscribe((res) => (this.Visible = res));
    this.subscriptions.push(vi);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      doitac: ["-1"],
      status: ["-1"],
    });
    this.subscriptions.push(
      this.filterGroup.controls.status.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.doitac.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter = {};
    const status = this.filterGroup.get("status").value;
    if (status) {
      filter["status"] = status;
    }

    const doitac = this.filterGroup.get("doitac").value;
    if (doitac) {
      filter["doitac"] = doitac;
    }
    this.nhomtaisanManagementService.patchState({ filter });
  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [""],
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .pipe(
        /*
      The user can type quite quickly in the input box, and that could trigger a lot of server requests. With this operator,
      we are limiting the amount of server requests emitted to a maximum of one every 150ms
      */
        debounceTime(150),
        distinctUntilChanged()
      )
      .subscribe((val) => this.search(val));
    this.subscriptions.push(searchEvent);
  }
  import() {
    debugger;
    const item = new NhomTaiSanModel();
    item.empty();
    let saveMessageTranslateParam = "";
    saveMessageTranslateParam += "Import thành công";
    const saveMessage = this.translate.instant(saveMessageTranslateParam);
    const messageType = MessageType.Create;
    const dialogRef = this.dialog.open(
      NhomTaiSanManagementImportDialogComponent,
      {
        data: { item: item },
        width: "500px",
        height: "310px",
        disableClose: true,
      }
    );
    dialogRef.afterClosed().subscribe((res) => {
      console.log("Res", res);
      if (!res) {
        this.nhomtaisanManagementService.fetch();
      } else {
        this.layoutUtilsService.showActionNotification(
          saveMessage,
          messageType,
          4000,
          true,
          false
        );
        this.nhomtaisanManagementService.fetch();
      }
    });
  }
  search(searchTerm: string) {
    this.nhomtaisanManagementService.patchState({ searchTerm });
  }

  sort(column: string): void {
    const sorting = this.sorting;
    const isActiveColumn = sorting.column === column;
    if (!isActiveColumn) {
      sorting.column = column;
      sorting.direction = "asc";
    } else {
      sorting.direction = sorting.direction === "asc" ? "desc" : "asc";
    }
    this.nhomtaisanManagementService.fetchStateSort({ sorting });
  }

  paginate(paginator: PaginatorState) {
    this.nhomtaisanManagementService.fetchStateSort({ paginator });
  }

  getHeight(): any {
    let tmp_height = 0;
    tmp_height = window.innerHeight - 236;
    return tmp_height + "px";
  }

  // 05/05/2021 09:40:58 => 05/05/2021
  getDate(date: string) {
    return date.split(" ")[0];
  }
  // 05/05/2021 09:40:58 => 09:40:58 05/05/2021
  getDateTime(date: string) {
    const time = date.split(" ")[1];
    const day = date.split(" ")[0];
    return time + " " + day;
  }

  create() {
    const item = new NhomTaiSanModel();
    item.empty();
    let saveMessageTranslateParam = "";
    saveMessageTranslateParam += "Thêm thành công";
    const saveMessage = this.translate.instant(saveMessageTranslateParam);
    const messageType = MessageType.Create;
    const dialogRef = this.dialog.open(
      NhomTaiSanManagementEditDialogComponent,
      {
        data: { item: item },
        // width: "900px",
        disableClose: true,
      }
    );
    dialogRef.afterClosed().subscribe((res) => {
      if (!res) {
        this.nhomtaisanManagementService.fetch();
      } else {
        this.layoutUtilsService.showActionNotification(
          saveMessage,
          messageType,
          4000,
          true,
          false
        );
        this.nhomtaisanManagementService.fetch();
      }
    });
  }

  update(item) {
    let saveMessageTranslateParam = "";
    saveMessageTranslateParam += "Cập nhật thành công";
    const saveMessage = this.translate.instant(saveMessageTranslateParam);
    const messageType = MessageType.Create;
    const dialogRef = this.dialog.open(
      NhomTaiSanManagementEditDialogComponent,
      {
        data: { item: item },
        // width: "900px",
        disableClose: true,
      }
    );
    dialogRef.afterClosed().subscribe((res) => {
      if (!res) {
        this.nhomtaisanManagementService.fetch();
      } else {
        this.layoutUtilsService.showActionNotification(
          saveMessage,
          messageType,
          4000,
          true,
          false
        );
        this.nhomtaisanManagementService.fetch();
      }
    });
  }
  delete(_item: NhomTaiSanModel) {
    debugger;
    const _title: string = "Xóa hiệu lực loại tài sản";
    const _description: string =
      "Bạn có chắc muốn xóa hiệu lực loại tài sản không?";
    const _waitDesciption: string = "hiệu lực loại tài sản đang được xóa...";
    const _deleteMessage = `Xóa thành công`;
    const _deleteMessageTrung = `Loại tài sản còn hiệu lực`;
    const dialogRef = this.layoutUtilsService.deleteElement(
      _title,
      _description,
      _waitDesciption
    );
    dialogRef.afterClosed().subscribe((res) => {
      if (!res) {
        return;
      }
      this.nhomtaisanManagementService
        .DeleteNhomTaiSan(_item)
        .subscribe((res) => {
          if (res.status === 2) {
            this.layoutUtilsService.showActionNotification(
              _deleteMessageTrung,
              MessageType.Delete,
              2000,
              true,
              false
            );
          }
          if (res && res.status === 1) {
            this.layoutUtilsService.showActionNotification(
              _deleteMessage,
              MessageType.Delete,
              2000,
              true,
              false
            );
          } else {
            this.layoutUtilsService.showActionNotification(
              res.error.message,
              MessageType.Delete,
              0
            );
          }
          this.nhomtaisanManagementService.fetch();
        });
    });
  }
  getVisibleColumns(): string[] {
    return this.Visible
      ? this.displayedColumns
      : this.displayedColumns.filter((col) => col !== "thaotac");
  }
  @HostListener("window:beforeunload", ["$event"])
  beforeunloadHandler(e) {
    this.auth.updateLastlogin().subscribe();
  }

  getTileStatus(value: boolean) {
    if (value) return this.translate.instant("COMMOM.TAMKHOA");
    return this.translate.instant("COMMOM.DANGSUDUNG");
  }
  getColorStatus(value: boolean) {
    if (!value) return COLOR_DANGHIEULUC;
    return COLOR_HETHIEULUC;
  }
}
