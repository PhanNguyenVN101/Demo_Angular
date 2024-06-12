using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_DVT;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Reponsitories.DM_DVT
{
    public interface IDM_DVTRepository
    {
        Task<IEnumerable<DM_DVTDTO>> DM_DVT_List(string whereStr);
        Task<ReturnSqlModel> DM_DVT_Insert(DM_DVTDTO model, long CreatedBy);
        Task<ReturnSqlModel> UpdateDVT(DM_DVTDTO model, long CreatedBy);
        Task<ReturnSqlModel> DeleteDVT(DM_DVTDTO model, long DeleteBy);
        Task<ReturnSqlModel> DeleteDVTs(decimal[] ids, long DeleteBy);
        Task<IEnumerable<DM_DVTDTO>> SearchDVT(string TenDVT);
        Task<DM_DVTDTO> GetDVTID(int IdDVT);
        Task<FileContentResult> Export(string whereStr);
    }
}
