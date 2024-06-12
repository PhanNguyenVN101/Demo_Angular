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

export class LoaiMatHangModel {
  IdLMH: number;
  MaLMH: string;
  TenLMH: string;
  IdCustomer: number;
  IdLMHParent: number;
  Mota: string;
  HinhAnh: string;
  DoUuTien: number;
  IdKho: number;
  TenK: string;
  LoaiMatHangCha: string;
  isFilter: Array<boolean>;
  empty() {
    this.IdLMH = 0;
    this.MaLMH = "";
    this.TenLMH = "";
    this.IdCustomer = 0;
    this.IdLMHParent = 0;
    this.Mota = "";
    this.HinhAnh = "";
    this.DoUuTien = 0;
    this.IdKho = 0;
    this.LoaiMatHangCha = "";
    this.TenK = "";
    this.isFilter = new Array<boolean>(4).fill(false);
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
