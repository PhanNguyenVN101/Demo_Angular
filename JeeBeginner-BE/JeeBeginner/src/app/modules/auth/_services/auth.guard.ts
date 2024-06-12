import { Injectable } from "@angular/core";
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from "@angular/router";
import { AuthService } from "./auth.service";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}
  public MYRoles: string[] = [];
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    const expectedRole = next.data.role;

    return new Observable<boolean>((observer) => {
      this.authService.getUserRolesFromToken().subscribe(
        (userRoles: string[]) => {
          this.MYRoles = userRoles;
          if (!userRoles) {
            console.error("User roles not found.");
            this.router.navigate(["/access-denied"]);
            observer.next(false);
            observer.complete();
          } else {
            const hasAccess = userRoles.includes(expectedRole);
            if (hasAccess) {
              observer.next(true);
              observer.complete();
            } else {
              this.router.navigate(["/access-denied"]);
              observer.next(false);
              observer.complete();
            }
          }
        },
        (error) => {
          console.error("Error occurred while retrieving user roles:", error);
          this.router.navigate(["/access-denied"]);
          observer.next(false);
          observer.complete();
        }
      );
    });
  }
}
