using System;

namespace JeeBeginner.Models.DM_XuatXu
{
    public class DM_XuatXuDTO
    {
        public int IdXuatXu { get; set; }
        public string TenXuatXu { get; set; }
        public bool isDel { get; set; }
        public int CreatedBy { get; set; }
        public int DeletedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime DeletedDate { get; set; }
        public int IdCustomer { get; set; }
    }
}
