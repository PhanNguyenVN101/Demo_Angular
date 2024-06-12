using System;
using System.Collections.Generic;

namespace JeeBeginner.Models.DM_LoaiMatHang
{
    public class DM_LoaiMatHangDTO
    {
        public int IdLMH { get; set; }
        public string MaLMH { get; set; }
        public string? TenLMH { get; set; }
        public int IdCustomer { get; set; }
        public int IdLMHParent { get; set; }
        public string? Mota { get; set; }
        public string? HinhAnh { get;set;}
        public int DoUuTien { get;set; }
        public bool isDel { get;set; }
        public int CreatedBy { get;set; }
        public int DeleteBy { get;set;}
        public int ModifiedBy { get;set;}
        public DateTime CreatedDate { get;set;}
        public DateTime ModifiedDate { get;set;}
        public DateTime DeleteDate { get;set;}
        public int IdKho { get; set; }
        public string LoaiMatHangCha { get;set; }
        public string TenK { get; set; }

        public List<bool> isFilter { get; set; }

    }
}
