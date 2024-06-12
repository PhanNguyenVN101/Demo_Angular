using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_DVT;
using JeeBeginner.Models.DM_LoaiMatHang;
using JeeBeginner.Reponsitories.AccountRoleManagement;
using JeeBeginner.Reponsitories.DM_DVT;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using JeeBeginner.Reponsitories.DM_XuatXu;
using JeeBeginner.Models.DM_XuatXu;
using JeeBeginner.Services.DM_XuatXu;

namespace JeeBeginner.Services.DM_XuatXu
{
    public class DM_XuatXuService:IDM_XuatXuService
    {
        private IDM_XuatXuRepository _reposiory;

        public DM_XuatXuService(IDM_XuatXuRepository dmXuatXuRepository)
        {
            _reposiory = dmXuatXuRepository;
        }
        #region Danh sách xuất xứ
        public async Task<IEnumerable<DM_XuatXuDTO>> DM_XuatXu_List(string whereStr)
        {
            return await _reposiory.DM_XuatXu_List(whereStr);

        }
        #endregion
        #region Thêm mới xuất xứ
        public async Task<ReturnSqlModel> DM_XuatXu_Insert(DM_XuatXuDTO model, long CreatedBy)
        {
            return await _reposiory.DM_XuatXu_Insert(model, CreatedBy);
        }
        #endregion
        #region Sửa xuất xứ
        public async Task<ReturnSqlModel> UpdateXuatXu(DM_XuatXuDTO model, long CreatedBy)
        {
            return await _reposiory.UpdateXuatXu(model, CreatedBy);
        }
        #endregion
        #region Xóa xuất xứ
        public async Task<ReturnSqlModel> DeleteXuatXu(DM_XuatXuDTO model, long DeleteBy)
        {
            return await _reposiory.DeleteXuatXu(model, DeleteBy);
        }
        #endregion
        #region Tìm kiếm tên xuất xứ
        public async Task<IEnumerable<DM_XuatXuDTO>> SearchXuatXu(string TenXuatXu)
        {
            return await _reposiory.SearchXuatXu(TenXuatXu);

        }
        #endregion

        #region Tìm mã xuất xứ
        public async Task<DM_XuatXuDTO> GetXuatXuID(int IdXuatXu)
        {
            return await _reposiory.GetXuatXuID(IdXuatXu);
        }
        #endregion
        #region Xóa nhiều xuất xứ
        public async Task<ReturnSqlModel> DeleteXuatXus(decimal[] ids, long DeleteBy)
        {

            return await _reposiory.DeleteXuatXus(ids, DeleteBy);
        }
        #endregion
    }
}
