import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";
import { MAT_DIALOG_DEFAULT_OPTIONS } from "@angular/material/dialog";
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { InlineSVGModule } from "ng-inline-svg";
import { JeeCustomerModule } from "src/app/pages/jee-customer.module";
import { XuatXuManagementService } from "./Services/xuatxu-management.service";
import { XuatXuManagementListComponent } from "./xuatxu-management-list/xuatxu-management-list.component";
import { XuatXuManagementEditDialogComponent } from "./xuatxu-management-edit-dialog/xuatxu-management-edit-dialog.component";
import { XuatXuManagementDetailDialogComponent } from "./xuatxu-management-detail-dialog/xuatxu-management-detail-dialog.component";
import { XuatXuManagementComponent } from "./xuatxu-management.component";
import { TranslateModule } from "@ngx-translate/core";
const routes: Routes = [
  {
    path: "",
    component: XuatXuManagementComponent,
    children: [
      {
        path: "",
        component: XuatXuManagementListComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [
    XuatXuManagementEditDialogComponent,
    XuatXuManagementListComponent,
    XuatXuManagementDetailDialogComponent,
    XuatXuManagementComponent,
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
    XuatXuManagementService,
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { hasBackdrop: true, height: "auto", width: "900px" },
    },
  ],
  entryComponents: [
    XuatXuManagementEditDialogComponent,
    XuatXuManagementListComponent,
    XuatXuManagementDetailDialogComponent,
    XuatXuManagementComponent,
  ],
})
export class XuatXuManagementModule {}
