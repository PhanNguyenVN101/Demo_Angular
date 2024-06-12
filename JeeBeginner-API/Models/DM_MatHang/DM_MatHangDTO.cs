using Aspose.Imaging.FileFormats.Emf.EmfPlus.Objects;
using System;
using System.Security.Policy;

namespace JeeBeginner.Models.DM_MatHang
{
    public class DM_MatHangDTO
    {
        public int IdMH { get; set; }
        public string? MaHang { get; set; }
        public string? TenMatHang { get;set; }
        public int? IdLMH { get; set; }
        public int? IdDVT { get; set; }
        public string? Mota { get; set; }
        public float? GiaMua { get; set; }
        public float? GiaBan { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsDel { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime DeletedDate { get; set;}
        public float? VAT { get; set; }
        public string? Barcode { get; set; }
        public bool? NgungKinhDoanh { get; set; }
        public int? IdDVTCap2 { get; set; }
        public float? QuyDoiDVTCap2 { get;set; }
        public int? IdDVTCap3 { get; set; }
        public float? QuyDoiDVTCap3 { get; set; }
        public string? TenOnSite { get; set; }
        public int? IdNhanHieu { get; set; }
        public int? IdXuatXu { get; set; }
        public string? HinhAnh { get; set; }
        public string? ChiTietMoTa { get; set; }
        public string? MaPhu { get; set; }
        public string? KichThuoc { get; set; }
        public string? ThongSo { get; set; }
        public bool? TheoDoiTonKho { get; set; }
        public bool? TheodoiLo { get; set; }
        public int? MaLuuKho { get;set; }
        public string? MaViTriKho { get; set; }
        public int? UpperLimit { get; set; }
        public int? LowerLimit { get; set; }
        public bool? IsTaiSan { get; set; }
        public int? SoKyTinhKhauHaoToiThieu { get; set; }
        public int? SoKyTinhKhauHaoToiDa { get; set; }
        public int? SoNamDeNghi { get; set; }
        public decimal? TiLeHaoMon { get; set; }
        public string? TenXuatXu { get; set; }
        public string? TenNhanHieu { get; set; }
        public string? TenLMH { get; set; }
        public string? TenDVT { get; set; }
        public string? TenDVTCap2 { get; set; }
        public string? TenDVTCap3 { get; set; }
        public string? TenK { get; set; }

    }
}
