using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_LoaiTaiSan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Reponsitories.DM_LoaiTaiSan
{
    public interface IDM_LoaiTaiSanRepository
    {
        Task<IEnumerable<DM_LoaiTaiSanDTO>> DM_LoaiTaiSan_List(string whereStr);
        Task<ReturnSqlModel> DM_LoaiTaiSan_Insert(DM_LoaiTaiSanDTO model);
        Task<ReturnSqlModel> UpdateLoaiTaiSan(DM_LoaiTaiSanDTO model);
        Task<ReturnSqlModel> DeleteLoaiTaiSan(DM_LoaiTaiSanDTO model);
        Task<IEnumerable<DM_LoaiTaiSanDTO>> SearchLoaiTaiSan(string TenLoai);
        Task<DM_LoaiTaiSanDTO> GetLoaiTaiSanID(int IdLoaiTS);
        Task<bool> ImportFromExcel(IFormFile file, long CreatedBy);
        Task<FileContentResult> TaiFileMau();
        
    }
}
