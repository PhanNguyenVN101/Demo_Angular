export class DoiTacBaoHiemModel {
  Id_DV: number;
  TenDonVi: string;
  DiaChi: string;
  SoDT: string;
  NguoiLienHe: string;
  GhiChu: string;

  empty() {
    this.Id_DV = 0;
    this.TenDonVi = "";
    this.DiaChi = "";
    this.SoDT = "";
    this.NguoiLienHe = "";
    this.GhiChu = "";
  }
}

// export class AccountStatusModel {
//   RowID: number;
//   IsLock: boolean;
//   Note: string;

//   empty() {
//     this.RowID = 0;
//     this.IsLock = false;
//     this.Note = "";
//   }
// }
