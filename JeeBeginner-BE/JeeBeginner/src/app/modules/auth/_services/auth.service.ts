import { Injectable, OnDestroy } from "@angular/core";
import {
  Observable,
  BehaviorSubject,
  of,
  Subscription,
  throwError,
} from "rxjs";
import { map, catchError, switchMap, finalize, tap } from "rxjs/operators";
import { LoginModel, User, UserModel } from "../_models/user.model";
import { AuthModel } from "../_models/auth.model";
import { AuthHTTPService } from "./auth-http";
import { environment } from "src/environments/environment";
import { Router } from "@angular/router";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import jwt_decode from "jwt-decode";

const API_AUTH_URL = `${environment.ApiRoot}/authorization`;

@Injectable({
  providedIn: "root",
})
export class AuthService implements OnDestroy {
  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  private authLocalStorageToken = `${environment.appVersion}-${environment.USERDATA_KEY}`;

  // public fields
  currentUser$: Observable<User>;
  isLoading$: Observable<boolean>;
  currentUserSubject: BehaviorSubject<User>;
  isLoadingSubject: BehaviorSubject<boolean>;
  errorMessageSubject: BehaviorSubject<string>;

  isLoggedIn$: Observable<boolean>;
  isLoggedOut$: Observable<boolean>;

  get currentUserValue(): User {
    return this.currentUserSubject.value;
  }

  set currentUserValue(user: User) {
    this.currentUserSubject.next(user);
  }

  constructor(private http: HttpClient, private router: Router) {
    this.currentUserSubject = new BehaviorSubject<User>(null);
    this.currentUser$ = this.currentUserSubject.asObservable();

    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.isLoading$ = this.isLoadingSubject.asObservable();

    this.errorMessageSubject = new BehaviorSubject<string>("");

    this.isLoggedIn$ = this.currentUser$.pipe(map((user) => !!user));

    this.isLoggedOut$ = this.isLoggedIn$.pipe(map((loggedIn) => !loggedIn));

    if (!this.getAuthFromLocalStorage()) {
      this.prepareLogout();
    } else {
      this.currentUserSubject.next(this.getAuthFromLocalStorage().user as User);
    }
  }

  login(username: string, password: string): Observable<User> {
    this.isLoadingSubject.next(true);
    const loginModel = new LoginModel();
    loginModel.Username = username;
    loginModel.Password = password;
    return this.getUser(loginModel).pipe(
      map((res) => {
        const result = this.setAuthFromLocalStorage(res);
        return result;
      }),
      catchError((err) => {
        this.errorMessageSubject.next(err.error.message);
        return of(undefined);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  prepareLogout() {
    localStorage.removeItem(this.authLocalStorageToken);
    this.router.navigate(["/auth/login"], {
      queryParams: {},
    });
  }

  logout() {
    if (this.getAuthFromLocalStorage()) {
      this.updateLastlogin().subscribe();
      this.prepareLogout();
    } else {
      this.prepareLogout();
    }
  }
  getUserRolesFromToken(): Observable<string[]> {
    debugger;
    const auth = this.getAuthFromLocalStorage();
    if (auth && auth.user && auth.user.Role) {
      // Split the roles string by comma and trim any whitespace
      const rolesArray = auth.user.Role.split(",").map((role) => role.trim());
      // Return the roles array wrapped in an observable
      return of(rolesArray);
    }
    // Return an empty observable if roles couldn't be retrieved
    return of([]);
  }
  getUserByToken(): Observable<any> {
    return of(true);
  }

  registration(user: UserModel): Observable<any> {
    return of(true);
  }

  forgotPassword(email: string): Observable<boolean> {
    return of(true);
  }
  renewToken(): Observable<any> {
    // Gọi API renew token ở đây
    return this.http.post<any>(`${API_AUTH_URL}/renewToken`, {}).pipe(
      tap((response) => {
        // Xử lý phản hồi từ máy chủ, cập nhật token mới vào local storage nếu cần
        const newToken = response.token;
        // Cập nhật token mới vào local storage hoặc thực hiện các bước khác
      }),
      catchError((error) => {
        // Xử lý lỗi nếu có
        console.error("Error occurred while renewing token:", error);
        return throwError("Failed to renew token.");
      })
    );
  }
  private setAuthFromLocalStorage(auth): boolean {
    if (auth && auth.token) {
      localStorage.setItem(this.authLocalStorageToken, JSON.stringify(auth));
      this.currentUserSubject.next(auth.user);
      return true;
    }
    return false;
  }

  getAuthFromLocalStorage(): any {
    try {
      const authData = JSON.parse(
        localStorage.getItem(this.authLocalStorageToken)
      );
      return authData;
    } catch (error) {
      return undefined;
    }
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }

  getUser(item: LoginModel): Observable<any> {
    return this.http.post<any>(API_AUTH_URL + "/login", item);
  }

  updateLastlogin(): Observable<any> {
    const httpHeaders = this.getHTTPHeaders();
    return this.http.get<any>(API_AUTH_URL + "/updateLastlogin", {
      headers: httpHeaders,
    });
  }

  isAuthenticated(): boolean {
    const auth = this.getAuthFromLocalStorage();
    if (auth) {
      if (this.isTokenExpired(auth.token)) {
        if (!auth.user) return false;
        if (!this.currentUserValue) {
          this.currentUserSubject.next(auth.user);
        }
        this.updateLastlogin().subscribe((res) => {});
        return true;
      }
      return false;
    }
    return false;
  }

  isTokenExpired(token: string): boolean {
    const date = this.getTokenExpirationDate(token);
    if (!date) return false;
    return date.valueOf() > new Date().valueOf();
  }

  getTokenExpirationDate(token: string): Date {
    let decoded: any = jwt_decode(token);
    if (!decoded.exp) return null;
    const date = new Date(0);
    date.setUTCSeconds(decoded.exp);
    return date;
  }

  getHTTPHeaders(): HttpHeaders {
    const auth = this.getAuthFromLocalStorage();
    if (auth) {
      const token = this.getAuthFromLocalStorage().token;
      let result = new HttpHeaders({
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
        "Access-Control-Allow-Origin": "*",
        "Access-Control-Allow-Headers": "Content-Type",
      });
      return result;
    }
    return undefined;
  }
}
