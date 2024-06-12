using System;

namespace JeeBeginner.Models.DM_DoiTacBaoHiem
{
    public class DM_DoiTacBaoHiemDTO
    {
        public int Id_DV { get; set; }
        public string TenDonVi { get; set; }
        public string? DiaChi { get; set; }  
        public string? SoDT { get; set; }
        public string? NguoiLienHe { get; set; }
        public string? GhiChu { get; set; }
        public bool IsDisable { get; set; }
        public int NguoiTao { get; set; }
        public int NguoiSua { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgaySua { get; set; }
    }
}
