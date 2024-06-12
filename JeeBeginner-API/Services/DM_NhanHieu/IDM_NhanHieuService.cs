using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_NhanHieu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Services.DM_NhanHieu
{
    public interface IDM_NhanHieuService
    {
        Task<IEnumerable<DM_NhanHieuDTO>> DM_NhanHieu_List(string whereStr);
        Task<ReturnSqlModel> DM_NhanHieu_Insert(DM_NhanHieuDTO model, long CreatedBy);
        Task<ReturnSqlModel> UpdateNhanHieu(DM_NhanHieuDTO model, long CreatedBy);
        Task<ReturnSqlModel> DeleteNhanHieu(DM_NhanHieuDTO model, long DeleteBy);
        Task<IEnumerable<DM_NhanHieuDTO>> SearchNhanHieu(string TenNhanHieu);
        Task<DM_NhanHieuDTO> GetNhanHieuID(int IdNhanHieu);
        Task<ReturnSqlModel> DeleteNhanHieus(decimal[] ids, long DeleteBy);
    }
}
