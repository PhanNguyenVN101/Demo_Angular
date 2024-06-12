using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_XuatXu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Reponsitories.DM_XuatXu
{
    public interface IDM_XuatXuRepository
    {
        Task<IEnumerable<DM_XuatXuDTO>> DM_XuatXu_List(string whereStr);
        Task<ReturnSqlModel> DM_XuatXu_Insert(DM_XuatXuDTO model, long CreatedBy);
        Task<ReturnSqlModel> UpdateXuatXu(DM_XuatXuDTO model, long CreatedBy);
        Task<ReturnSqlModel> DeleteXuatXu(DM_XuatXuDTO model, long DeleteBy);
        Task<ReturnSqlModel> DeleteXuatXus(decimal[] ids, long DeleteBy);
        Task<IEnumerable<DM_XuatXuDTO>> SearchXuatXu(string TenXuatXu);
        Task<DM_XuatXuDTO> GetXuatXuID(int IdXUatXu);
    }
}
