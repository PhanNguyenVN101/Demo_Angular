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
using JeeBeginner.Reponsitories.DM_NhanHieu;
using JeeBeginner.Models.DM_NhanHieu;
using JeeBeginner.Services.DM_NhanHieu;

namespace JeeBeginner.Services.DM_DVT
{
    public class DM_NhanHieuService : IDM_NhanHieuService
    {
        private IDM_NhanHieuRepository _reposiory;

        public DM_NhanHieuService(IDM_NhanHieuRepository dmNhanHieuRepository)
        {
            _reposiory = dmNhanHieuRepository;
        }
        #region Danh sách nhãn hiệu
        public async Task<IEnumerable<DM_NhanHieuDTO>> DM_NhanHieu_List(string whereStr)
        {
            return await _reposiory.DM_NhanHieu_List(whereStr);

        }
        #endregion
        #region Thêm mới nhãn hiệu
        public async Task<ReturnSqlModel> DM_NhanHieu_Insert(DM_NhanHieuDTO model, long CreatedBy)
        {
            return await _reposiory.DM_NhanHieu_Insert(model, CreatedBy);
        }
        #endregion
        #region Sửa nhãn hiệu
        public async Task<ReturnSqlModel> UpdateNhanHieu(DM_NhanHieuDTO model, long CreatedBy)
        {
            return await _reposiory.UpdateNhanHieu(model, CreatedBy);
        }
        #endregion
        #region Xóa nhãn hiệu
        public async Task<ReturnSqlModel> DeleteNhanHieu(DM_NhanHieuDTO model, long DeleteBy)
        {
            return await _reposiory.DeleteNhanHieu(model, DeleteBy);
        }
        #endregion
        #region Tìm kiếm tên nhãn hiệu
        public async Task<IEnumerable<DM_NhanHieuDTO>> SearchNhanHieu(string TenNhanHieu)
        {
            return await _reposiory.SearchNhanHieu(TenNhanHieu);

        }
        #endregion

        #region Tìm mã nhãn hiệu
        public async Task<DM_NhanHieuDTO> GetNhanHieuID(int IdNhanHieu)
        {
            return await _reposiory.GetNhanHieuID(IdNhanHieu);
        }
        #endregion
        #region Xóa nhiều nhãn hiệu
        public async Task<ReturnSqlModel> DeleteNhanHieus(decimal[] ids, long DeleteBy)
        {
            
            return await _reposiory.DeleteNhanHieus(ids, DeleteBy);
        }
        #endregion

    }
}
