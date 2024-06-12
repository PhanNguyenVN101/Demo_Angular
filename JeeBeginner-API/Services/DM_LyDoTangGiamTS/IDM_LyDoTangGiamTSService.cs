using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_LyDoTangGiamTS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Services.DM_LyDoTangGiamTS
{
    public interface IDM_LyDoTangGiamTSService
    {
        Task<IEnumerable<DM_LyDoTangGiamTSDTO>> DM_LyDoTangGiamTS_List(string whereStr);
        Task<ReturnSqlModel> DM_LyDoTangGiamTS_Insert(DM_LyDoTangGiamTSDTO model);
        Task<ReturnSqlModel> UpdateLyDoTangGiamTS(DM_LyDoTangGiamTSDTO model);
        Task<ReturnSqlModel> DeleteLyDoTangGiamTS(DM_LyDoTangGiamTSDTO model);
        Task<IEnumerable<DM_LyDoTangGiamTSDTO>> SearchLyDoTangGiamTS(string TenTangGiam);
        Task<DM_LyDoTangGiamTSDTO> GetLyDoTangGiamTSID(int IdLoaiTS);
        Task<bool> ImportFromExcel(IFormFile file, long CreatedBy);
        Task<FileContentResult> TaiFileMau();
    }
}
