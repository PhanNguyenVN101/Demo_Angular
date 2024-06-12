import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { LayoutComponent } from "./_layout/layout.component";
import { AuthGuard } from "../modules/auth/_services/auth.guard";

const routes: Routes = [
  {
    path: "",
    component: LayoutComponent,
    children: [
      {
        path: "builder",
        loadChildren: () =>
          import("./builder/builder.module").then((m) => m.BuilderModule),
      },
      {
        path: "dashboard",
        loadChildren: () =>
          import(
            "./JeeBeginner/page-girdters-dashboard/page-girdters-dashboard.module"
          ).then((m) => m.PageGirdtersDashboardModule),
      },
      {
        path: "Management/CustomerManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/CustomerManagement/customer-management.module"
          ).then((m) => m.CustomerManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3900" },
      },
      {
        path: "Management/PartnerManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/PartnerManagement/partner-management.module"
          ).then((m) => m.PartnerManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3900" },
      },
      {
        path: "Management/AccountManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/AccountManagement/account-management.module"
          ).then((m) => m.AccountManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3900" },
      },
      {
        path: "Management/TaikhoanManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/TaikhoanManagement/taikhoan-management.module"
          ).then((m) => m.TaikhoanManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3900" },
      },
      {
        path: "Management/AccountRoleManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/AccountRoleManagement/accountrole-management.module"
          ).then((m) => m.AccountRoleManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3900" },
      },
      {
        path: "Management/DanhMucManagement/LoaiMatHangManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/LoaiMatHangManagement/loaimathang-management.module"
          ).then((m) => m.LoaiMatHangManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3904" },
      },
      {
        path: "Management/DanhMucManagement/DVTManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/DVTManagement/dvt-management.module"
          ).then((m) => m.DVTManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3903" },
      },
      {
        path: "Management/DanhMucManagement/NhanHieuManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/NhanHieuManagement/nhanhieu-management.module"
          ).then((m) => m.NhanHieuManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3902" },
      },
      {
        path: "Management/DanhMucManagement/XuatXuManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/XuatXuManagement/xuatxu-management.module"
          ).then((m) => m.XuatXuManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3610" },
      },
      {
        path: "Management/DanhMucManagement/DoiTacBaoHiemManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/DoiTacBaoHiemManagement/doitacbaohiem-management.module"
          ).then((m) => m.DoiTacBaoHiemManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3905" },
      },
      {
        path: "Management/DanhMucManagement/LoaiTaiSanManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/LoaiTaiSanManagement/loaitaisan-management.module"
          ).then((m) => m.LoaiTaiSanManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3906" },
      },
      {
        path: "Management/DanhMucManagement/NhomTaiSanManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/NhomTaiSanManagement/nhomtaisan-management.module"
          ).then((m) => m.NhomTaiSanManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3700" },
      },
      {
        path: "Management/DanhMucManagement/LyDoTangGiamTaiSanManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/LyDoTangGiamTaiSanManagement/lydotanggiamtaisan-management.module"
          ).then((m) => m.LyDoTangGiamTaiSanManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3901" },
      },
      {
        path: "Management/DanhMucManagement/MatHangManagement",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/DanhMucManagement/MatHangManagement/mathang-management.module"
          ).then((m) => m.MatHangManagementModule),
        canActivate: [AuthGuard],
        data: { role: "3800" },
      },
      {
        path: "Abc",
        loadChildren: () =>
          import(
            "./JeeBeginner/Management/AccountManagement/account-management.module"
          ).then((m) => m.AccountManagementModule),
      },
      {
        path: "",
        redirectTo: "/Management/CustomerManagement",
        pathMatch: "full",
      },
      {
        path: "**",
        redirectTo: "error/404",
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PagesRoutingModule {}
