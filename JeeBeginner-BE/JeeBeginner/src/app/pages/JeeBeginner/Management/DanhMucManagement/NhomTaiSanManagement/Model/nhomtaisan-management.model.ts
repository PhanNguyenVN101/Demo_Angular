// export class AccountModel {
//   RowId: number;
//   PartnerId: number;
//   Gender: string;
//   Fullname: string;
//   Mobile: string;
//   Email: string;
//   Username: string;
//   Password: string;
//   Note: string;

//   empty() {
//     this.RowId = 0;
//     this.PartnerId = 0;
//     this.Gender = "Nam";
//     this.Fullname = "";
//     this.Mobile = "";
//     this.Email = "";
//     this.Username = "";
//     this.Password = "";
//     this.Note = "";
//   }
// }

export class NhomTaiSanModel {
  IdPNTS: number;
  MaNhom: string;
  TenNhom: string;
  TrangThai: boolean;

  empty() {
    this.IdPNTS = 0;
    this.MaNhom = "";
    this.TenNhom = "";
    this.TrangThai = false;
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
