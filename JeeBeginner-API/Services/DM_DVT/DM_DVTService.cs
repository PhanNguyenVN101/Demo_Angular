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
using Microsoft.AspNetCore.Mvc;

namespace JeeBeginner.Services.DM_DVT
{
    public class DM_DVTService : IDM_DVTService
    {
        private IDM_DVTRepository _reposiory;

        public DM_DVTService(IDM_DVTRepository dmDVTRepository)
        {
            _reposiory = dmDVTRepository;
        }
        #region Danh sách đơn vị tính
        public async Task<IEnumerable<DM_DVTDTO>> DM_DVT_List(string whereStr)
        {
            return await _reposiory.DM_DVT_List(whereStr);

        }
        #endregion
        #region Thêm mới đơn vị tính
        public async Task<ReturnSqlModel> DM_DVT_Insert(DM_DVTDTO model, long CreatedBy)
        {
            return await _reposiory.DM_DVT_Insert(model, CreatedBy);
        }
        #endregion
        #region Sửa đơn vị tính
        public async Task<ReturnSqlModel> UpdateDVT(DM_DVTDTO model, long CreatedBy)
        {
            return await _reposiory.UpdateDVT(model, CreatedBy);
        }
        #endregion
        #region Xóa đơn vị tính
        public async Task<ReturnSqlModel> DeleteDVT(DM_DVTDTO model, long DeleteBy)
        {
            return await _reposiory.DeleteDVT(model, DeleteBy);
        }
        #endregion
        #region Tìm kiếm tên đơn vị tính
        public async Task<IEnumerable<DM_DVTDTO>> SearchDVT(string TenDVT)
        {
            return await _reposiory.SearchDVT(TenDVT);

        }
        #endregion
        
        #region Tìm mã đơn vị tính
        public async Task<DM_DVTDTO> GetDVTID(int IdDVT)
        {
            return await _reposiory.GetDVTID(IdDVT);
        }
        #endregion
        #region Xóa nhiều Đơn vị tính
        public async Task<ReturnSqlModel> DeleteDVTs(decimal[] ids, long DeleteBy)
        {
            
            return await _reposiory.DeleteDVTs(ids, DeleteBy);
        }
        #endregion

        #region Xuất File Execl
        public async Task<FileContentResult> Export(string whereStr)
        {
            return await _reposiory.Export(whereStr);
        }
        #endregion

    }
}
