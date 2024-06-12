import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";
import { MAT_DIALOG_DEFAULT_OPTIONS } from "@angular/material/dialog";
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { InlineSVGModule } from "ng-inline-svg";
import { JeeCustomerModule } from "src/app/pages/jee-customer.module";
import { DVTManagementService } from "./Services/dvt-management.service";
import { DVTManagementListComponent } from "./dvt-management-list/dvt-management-list.component";
import { DVTManagementEditDialogComponent } from "./dvt-management-edit-dialog/dvt-management-edit-dialog.component";
import { DVTManagementDetailDialogComponent } from "./dvt-management-detail-dialog/dvt-management-detail-dialog.component";
import { DVTManagementComponent } from "./dvt-management.component";
import { TranslateModule } from "@ngx-translate/core";
const routes: Routes = [
  {
    path: "",
    component: DVTManagementComponent,
    children: [
      {
        path: "",
        component: DVTManagementListComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [
    DVTManagementEditDialogComponent,
    DVTManagementListComponent,
    DVTManagementDetailDialogComponent,
    DVTManagementComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    JeeCustomerModule,
    NgxMatSelectSearchModule,
    InlineSVGModule,
    TranslateModule,
  ],
  providers: [
    DVTManagementService,
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { hasBackdrop: true, height: "auto", width: "900px" },
    },
  ],
  entryComponents: [
    DVTManagementEditDialogComponent,
    DVTManagementListComponent,
    DVTManagementDetailDialogComponent,
    DVTManagementComponent,
  ],
})
export class DVTManagementModule {}
