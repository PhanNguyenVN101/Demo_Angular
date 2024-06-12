using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_DoiTacBaoHiem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Services.DM_DoiTacBaoHiem
{
    public interface IDM_DoiTacBaoHiemService
    {
        Task<IEnumerable<DM_DoiTacBaoHiemDTO>> DM_DoiTacBaoHiem_List(string whereStr);
        Task<ReturnSqlModel> DM_DoiTacBaoHiem_Insert(DM_DoiTacBaoHiemDTO model, long CreatedBy);
        Task<ReturnSqlModel> UpdateDoiTacBaoHiem(DM_DoiTacBaoHiemDTO model, long CreatedBy);
        Task<ReturnSqlModel> DeleteDoiTacBaoHiem(DM_DoiTacBaoHiemDTO model);
        Task<IEnumerable<DM_DoiTacBaoHiemDTO>> SearchDoiTacBaoHiem(string TenDoiTacBaoHiem);
        Task<DM_DoiTacBaoHiemDTO> GetDoiTacBaoHiemID(int IdDoiTacBaoHiem);
        Task<bool> ImportFromExcel(IFormFile file, long CreatedBy);
        Task<FileContentResult> TaiFileMau();
    }
}
