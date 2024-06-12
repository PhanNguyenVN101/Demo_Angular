import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable, of, Subscription } from "rxjs";
import { Inject, Injectable } from "@angular/core";

import { QueryParamsModelNew } from "../../../../_core/models/query-models/query-params.model";
import {
  ResultModel,
  ResultObjModel,
} from "../../../../_core/models/_base.model";
import { environment } from "../../../../../../../environments/environment";
import { CATCH_ERROR_VAR } from "@angular/compiler/src/output/output_ast";
import { catchError, finalize, tap } from "rxjs/operators";
import {
  GroupingState,
  ITableState,
  PaginatorState,
  SortState,
} from "src/app/_metronic/shared/crud-table";
import { XuatXuModel } from "../Model/xuatxu-management.model";
import { HttpUtilsService } from "../../../../_core/utils/http-utils.service";

const API_PRODUCTS_URL = environment.ApiRoot + "/dm_xuatxu";
const DEFAULT_STATE: ITableState = {
  filter: {},
  paginator: new PaginatorState(),
  sorting: new SortState(),
  searchTerm: "",
  grouping: new GroupingState(),
  entityId: undefined,
};

@Injectable()
export class XuatXuManagementService {
  // Private fields
  private _items$ = new BehaviorSubject<XuatXuModel[]>([]);
  private _isLoading$ = new BehaviorSubject<boolean>(false);
  private _isFirstLoading$ = new BehaviorSubject<boolean>(true);
  private _tableState$ = new BehaviorSubject<ITableState>(DEFAULT_STATE);
  private _errorMessage = new BehaviorSubject<string>("");
  private _subscriptions: Subscription[] = [];
  private _tableAppCodeState$ = new BehaviorSubject<ITableState>(DEFAULT_STATE);
  // Getters
  get items$() {
    return this._items$.asObservable();
  }

  get isLoading$() {
    return this._isLoading$.asObservable();
  }
  get isFirstLoading$() {
    return this._isFirstLoading$.asObservable();
  }
  get errorMessage$() {
    return this._errorMessage.asObservable();
  }
  get subscriptions() {
    return this._subscriptions;
  }
  get tableAppCodeState$() {
    return this._tableAppCodeState$.asObservable();
  }
  // State getters
  get paginator() {
    return this._tableState$.value.paginator;
  }
  get paginatorAppList() {
    return this._tableAppCodeState$.value.paginator;
  }
  get filter() {
    return this._tableState$.value.filter;
  }
  get sorting() {
    return this._tableState$.value.sorting;
  }
  get searchTerm() {
    return this._tableState$.value.searchTerm;
  }
  get grouping() {
    return this._tableState$.value.grouping;
  }

  constructor(private http: HttpClient, private httpUtils: HttpUtilsService) {}

  public fetch() {
    this._isLoading$.next(true);
    this._errorMessage.next("");
    const request = this.findData(this._tableState$.value)
      .pipe(
        tap((res: ResultModel<XuatXuModel>) => {
          if (res && res.status === 1) {
            this._items$.next(res.data);
            this._tableState$.value.paginator.total = res.panigator.TotalCount;
          } else {
            this._errorMessage.next(res.error.message);
            return of({
              items: [],
              total: 0,
            });
          }
        }),
        finalize(() => {
          this._isLoading$.next(false);
        })
      )
      .subscribe();
    this._subscriptions.push(request);
  }

  // Base Methods
  public patchState(patch: Partial<ITableState>) {
    this.patchStateWithoutFetch(patch);
    this.fetch();
    // đổi tên thành fetch thì chạy các chức năng còn lại bth
  }

  public fetchStateSort(patch: Partial<ITableState>) {
    this.patchStateWithoutFetch(patch);
    this.fetch();
  }

  public patchStateWithoutFetch(patch: Partial<ITableState>) {
    const newState = Object.assign(this._tableState$.value, patch);
    this._tableState$.next(newState);
  }

  private findData(tableState: ITableState): Observable<any> {
    this._errorMessage.next("");
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url = API_PRODUCTS_URL + "/DM_XuatXu_List";
    return this.http
      .post<any>(url, tableState, {
        headers: httpHeaders,
      })
      .pipe(
        catchError((err) => {
          this._errorMessage.next(err);
          return of({ items: [], total: 0 });
        })
      );
  }
  DM_XuatXu_Insert(item: XuatXuModel): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url = API_PRODUCTS_URL + "/DM_XuatXu_Insert";
    return this.http.post<any>(url, item, { headers: httpHeaders });
  }
  UpdateXuatXu(item: XuatXuModel): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url = API_PRODUCTS_URL + "/UpdateXuatXu";
    return this.http.post<any>(url, item, { headers: httpHeaders });
  }
  DeleteXuatXu(item: XuatXuModel): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url = API_PRODUCTS_URL + "/DeleteXuatXu";
    return this.http.post<any>(url, item, { headers: httpHeaders });
  }
  DeleteXuatXus(ids: any[] = []): Observable<any> {
    const url = API_PRODUCTS_URL + "/DeleteXuatXus";
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    return this.http.post<any>(url, ids, { headers: httpHeaders });
  }

  // createAccount(item: AccountRoleModel): Observable<any> {
  //   const httpHeaders = this.httpUtils.getHTTPHeaders();
  //   const url = API_PRODUCTS_URL + "/createAccount";
  //   return this.http.post<any>(url, item, { headers: httpHeaders });
  // }

  // updateAccount(item: AccountRoleModel): Observable<any> {
  //   const httpHeaders = this.httpUtils.getHTTPHeaders();
  //   const url = API_PRODUCTS_URL + "/updateAccount";
  //   return this.http.post<any>(url, item, { headers: httpHeaders });
  // }

  // UpdateStatusAccount(item: AccountRoleStatusModel): Observable<any> {
  //   const httpHeaders = this.httpUtils.getHTTPHeaders();
  //   const url = API_PRODUCTS_URL + "/UpdateStatusAccount";
  //   return this.http.post<any>(url, item, { headers: httpHeaders });
  // }

  getAccountModelByRowID(RowID: number): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url = API_PRODUCTS_URL + `/GetAccountByRowID?RowID=${RowID}`;
    return this.http.get<any>(url, { headers: httpHeaders });
  }
  GetXuatXuID(IdXuatXu: number): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url = API_PRODUCTS_URL + `/GetXuatXuID?IdXuatXu=${IdXuatXu}`;
    return this.http.get<any>(url, { headers: httpHeaders });
  }

  SearchXuatXu(TenXuatXu: string): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url = API_PRODUCTS_URL + `/SearchXuatXu?TenXuatXu=${TenXuatXu}`;
    return this.http.get<any>(url, { headers: httpHeaders });
  }
  // getRoleByUsername(Username: string): Observable<any> {
  //   const httpHeaders = this.httpUtils.getHTTPHeaders();
  //   const url = API_PRODUCTS_URL + `/GetRoleUsername?Username=${Username}`;
  //   return this.http.post<any>(url, { headers: httpHeaders });
  // }
  // public getRole(Username: string): Observable<any> {
  //   this._errorMessage.next("");
  //   const httpHeaders = this.httpUtils.getHTTPHeaders();
  //   const url = API_PRODUCTS_URL + `/GetRoleUsername?Username=${Username}`;

  //   return this.http
  //     .get<any>(url, {
  //       headers: httpHeaders,
  //     })
  //     .pipe(
  //       catchError((err) => {
  //         this._errorMessage.next(err);
  //         return of({ items: [] });
  //       })
  //     );
  // }

  getNoteLock(RowID: number): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url = API_PRODUCTS_URL + `/GetNoteLock?RowID=${RowID}`;
    return this.http.get<any>(url, { headers: httpHeaders });
  }

  IsReadOnlyPermitAccountRole(roleName: string): Observable<any> {
    const httpHeaders = this.httpUtils.getHTTPHeaders();
    const url =
      API_PRODUCTS_URL + `/IsReadOnlyPermitAccountRole?roleName=${roleName}`;
    return this.http.get<any>(url, { headers: httpHeaders });
  }
}
