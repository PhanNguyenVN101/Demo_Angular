using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_NhomTaiSan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Reponsitories.DM_NhomTaiSan
{
    public interface IDM_NhomTaiSanRepository
    {
        Task<IEnumerable<DM_NhomTaiSanDTO>> DM_NhomTaiSan_List(string whereStr);
        Task<ReturnSqlModel> DM_NhomTaiSan_Insert(DM_NhomTaiSanDTO model);
        Task<ReturnSqlModel> UpdateNhomTaiSan(DM_NhomTaiSanDTO model);
        Task<ReturnSqlModel> DeleteNhomTaiSan(DM_NhomTaiSanDTO model);
        Task<IEnumerable<DM_NhomTaiSanDTO>> SearchNhomTaiSan(string TenLoai);
        Task<DM_NhomTaiSanDTO> GetNhomTaiSanID(int IdLoaiTS);
        Task<bool> ImportFromExcel(IFormFile file, long CreatedBy);
        Task<FileContentResult> TaiFileMau();
    }
}
