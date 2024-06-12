import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";
import { MAT_DIALOG_DEFAULT_OPTIONS } from "@angular/material/dialog";
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { InlineSVGModule } from "ng-inline-svg";
import { JeeCustomerModule } from "src/app/pages/jee-customer.module";
import { LoaiMatHangManagementService } from "./Services/loaimathang-management.service";
import { LoaiMatHangManagementListComponent } from "./loaimathang-management-list/loaimathang-management-list.component";
import { LoaiMatHangManagementEditDialogComponent } from "./loaimathang-management-edit-dialog/loaimathang-management-edit-dialog.component";
import { LoaiMatHangManagementDetailDialogComponent } from "./loaimathang-management-detail-dialog/loaimathang-management-detail-dialog.component";
import { LoaiMatHangManagementComponent } from "./loaimathang-management.component";
import { LoaiMatHangManagementFillDialogComponent } from "./loaimathang-management-fill-dialog/loaimathang-management-fill-dialog.component";
import { TranslateModule } from "@ngx-translate/core";
const routes: Routes = [
  {
    path: "",
    component: LoaiMatHangManagementComponent,
    children: [
      {
        path: "",
        component: LoaiMatHangManagementListComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [
    LoaiMatHangManagementEditDialogComponent,
    LoaiMatHangManagementListComponent,
    LoaiMatHangManagementComponent,
    LoaiMatHangManagementDetailDialogComponent,
    LoaiMatHangManagementFillDialogComponent,
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
    LoaiMatHangManagementService,
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { hasBackdrop: true, height: "auto", width: "900px" },
    },
  ],
  entryComponents: [
    LoaiMatHangManagementEditDialogComponent,
    LoaiMatHangManagementListComponent,
    LoaiMatHangManagementComponent,
    LoaiMatHangManagementDetailDialogComponent,
    LoaiMatHangManagementFillDialogComponent,
  ],
})
export class LoaiMatHangManagementModule {}
