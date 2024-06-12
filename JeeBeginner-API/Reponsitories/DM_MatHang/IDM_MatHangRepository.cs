using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_LoaiMatHang;
using JeeBeginner.Models.DM_MatHang;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Reponsitories.DM_MatHang
{
    public interface IDM_MatHangRepository
    {
        Task<IEnumerable<DM_MatHangDTO>> DM_MatHang_List(string whereStr);
        Task<ReturnSqlModel> DM_MatHang_Insert(DM_MatHangDTO model, long CreatedBy);
        Task<ReturnSqlModel> UpdateMatHang(DM_MatHangDTO model,long CreatedBy);
        Task<ReturnSqlModel> DeleteMH(DM_MatHangDTO model, long DeleteBy);
        Task<IEnumerable<DM_MatHangDTO>> SearchMatHang(string TenMatHang);
        Task<DM_MatHangDTO> GetMatHangID(int IdMH);
        Task<ReturnSqlModel> DeleteMHs(decimal[] ids, long DeleteBy);
        Task<bool> ImportFromExcel(IFormFile file, long CreatedBy);
        Task<FileContentResult> TaiFileMau();
    }
}
