import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";
import { MAT_DIALOG_DEFAULT_OPTIONS } from "@angular/material/dialog";
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { InlineSVGModule } from "ng-inline-svg";
import { JeeCustomerModule } from "src/app/pages/jee-customer.module";
import { LyDoTangGiamTaiSanManagementService } from "./Services/lydotanggiamtaisan-management.service";
import { LyDoTangGiamTaiSanManagementListComponent } from "./lydotanggiamtaisan-management-list/lydotanggiamtaisan-management-list.component";
import { LyDoTangGiamTaiSanManagementEditDialogComponent } from "./lydotanggiamtaisan-management-edit-dialog/lydotanggiamtaisan-management-edit-dialog.component";
import { LyDoTangGiamTaiSanManagementImportDialogComponent } from "./lydotanggiamtaisan-management-import-dialog/lydotanggiamtaisan-management-import-dialog.component";
import { LyDoTangGiamTaiSanManagementComponent } from "./lydotanggiamtaisan-management.component";
import { TranslateModule } from "@ngx-translate/core";
const routes: Routes = [
  {
    path: "",
    component: LyDoTangGiamTaiSanManagementComponent,
    children: [
      {
        path: "",
        component: LyDoTangGiamTaiSanManagementListComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [
    LyDoTangGiamTaiSanManagementEditDialogComponent,
    LyDoTangGiamTaiSanManagementListComponent,
    LyDoTangGiamTaiSanManagementComponent,
    LyDoTangGiamTaiSanManagementImportDialogComponent,
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
    LyDoTangGiamTaiSanManagementService,
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { hasBackdrop: true, height: "auto", width: "auto" },
    },
  ],
  entryComponents: [
    LyDoTangGiamTaiSanManagementEditDialogComponent,
    LyDoTangGiamTaiSanManagementListComponent,
    LyDoTangGiamTaiSanManagementComponent,
    LyDoTangGiamTaiSanManagementImportDialogComponent,
  ],
})
export class LyDoTangGiamTaiSanManagementModule {}
