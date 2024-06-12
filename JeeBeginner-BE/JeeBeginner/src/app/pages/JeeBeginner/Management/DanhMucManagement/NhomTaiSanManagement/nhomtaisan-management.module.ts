import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";
import { MAT_DIALOG_DEFAULT_OPTIONS } from "@angular/material/dialog";
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { InlineSVGModule } from "ng-inline-svg";
import { JeeCustomerModule } from "src/app/pages/jee-customer.module";
import { NhomTaiSanManagementService } from "./Services/nhomtaisan-management.service";
import { NhomTaiSanManagementListComponent } from "./nhomtaisan-management-list/nhomtaisan-management-list.component";
import { NhomTaiSanManagementEditDialogComponent } from "./nhomtaisan-management-edit-dialog/nhomtaisan-management-edit-dialog.component";
import { NhomTaiSanManagementComponent } from "./nhomtaisan-management.component";
import { NhomTaiSanManagementImportDialogComponent } from "./nhomtaisan-management-import-dialog/nhomtaisan-management-import-dialog.component";
import { TranslateModule } from "@ngx-translate/core";
const routes: Routes = [
  {
    path: "",
    component: NhomTaiSanManagementComponent,
    children: [
      {
        path: "",
        component: NhomTaiSanManagementListComponent,
      },
    ],
  },
];

@NgModule({
  declarations: [
    NhomTaiSanManagementEditDialogComponent,
    NhomTaiSanManagementListComponent,
    NhomTaiSanManagementComponent,
    NhomTaiSanManagementImportDialogComponent,
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
    NhomTaiSanManagementService,
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { hasBackdrop: true, height: "auto", width: "auto" },
    },
  ],
  entryComponents: [
    NhomTaiSanManagementEditDialogComponent,
    NhomTaiSanManagementListComponent,
    NhomTaiSanManagementComponent,
    NhomTaiSanManagementImportDialogComponent,
  ],
})
export class NhomTaiSanManagementModule {}
