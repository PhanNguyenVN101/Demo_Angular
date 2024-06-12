using System;

namespace JeeBeginner.Models.DM_DVT
{
    public class DM_DVTDTO
    {
        public int IdDVT { get; set; }
        public string TenDVT { get; set; }
        public int IdCustomer { get; set; }
        public bool isDel { get;set; }
        public int CreatedBy { get; set; }
        public int DeleteBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set;}
        public DateTime DeleteDate { get; set;}

    }
}
 
