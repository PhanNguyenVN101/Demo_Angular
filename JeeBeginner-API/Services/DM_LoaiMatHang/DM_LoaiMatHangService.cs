using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_LoaiMatHang;
using JeeBeginner.Reponsitories.AccountRoleManagement;
using JeeBeginner.Reponsitories.DM_LoaiMatHang;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace JeeBeginner.Services.DM_LoaiMatHang
{
    public class DM_LoaiMatHangService : IDM_LoaiMatHangService
    {
        private IDM_LoaiMatHangRepository _reposiory;

        public DM_LoaiMatHangService(IDM_LoaiMatHangRepository dmLoaiMatHangRepository)
        {
            _reposiory = dmLoaiMatHangRepository;
        }
        #region Danh sách Loại mặt hàng
        public async Task<IEnumerable<DM_LoaiMatHangDTO>> DM_LoaiMatHang_List(string whereStr)
        {
            return await _reposiory.DM_LoaiMatHang_List(whereStr);

        }
        #endregion
        #region Thêm mới Loại mặt hàng
        public async Task<ReturnSqlModel> DM_LoaiMatHang_Insert(DM_LoaiMatHangDTO model, long CreatedBy)
        {
            return await _reposiory.DM_LoaiMatHang_Insert(model, CreatedBy);
        }
        #endregion
        #region Sửa Loại mặt hàng
        public async Task<ReturnSqlModel> UpdateLoaiMatHang(DM_LoaiMatHangDTO model, long CreatedBy)
        {
            return await _reposiory.UpdateLoaiMatHang(model, CreatedBy);
        }
        #endregion
        #region Xóa Loại mặt hàng
        public async Task<ReturnSqlModel> DeleteLMH(DM_LoaiMatHangDTO model, long DeleteBy)
        {
            return await _reposiory.DeleteLMH(model, DeleteBy);
        }
        #endregion

        #region Xoá nhiều loại mặt hàng
        public async Task<ReturnSqlModel> Deletes(decimal[] ids, long DeleteBy)
        {
            
            return await _reposiory.Deletes(ids, DeleteBy);
        }
        #endregion
        #region Tìm kiếm Loại mặt hàng
        public async Task<IEnumerable<DM_LoaiMatHangDTO>> FilterLMH(DM_LoaiMatHangDTO model)
        {
            return await _reposiory.FilterLMH(model);

        }
        #endregion
        #region Danh sách kho
        public async Task<IEnumerable<DM_LoaiMatHangDTO>> DM_Kho_List()
        {
            return await _reposiory.DM_Kho_List();
        }
        #endregion
        #region Tìm mã kho
        public async Task<DM_LoaiMatHangDTO> GetKhoID(int IdK)
        {
            return await _reposiory.GetKhoID(IdK);
        }
        #endregion
        #region Tìm kiếm mã loại mặt hàng cha
        public async Task<DM_LoaiMatHangDTO> GetLoaiMHChaID(int IdLMHParent)
        {
            return await _reposiory.GetLoaiMHChaID(IdLMHParent);
        }
        #endregion
        #region Danh sách loại mặt hàng cha
        public async Task<IEnumerable<DM_LoaiMatHangDTO>> LoaiMatHangCha_List()
        {
            return await _reposiory.LoaiMatHangCha_List();
        }
        #endregion
        #region Tìm kiếm mã loại mặt hàng
        public async Task<DM_LoaiMatHangDTO> GetLoaiMHID(int IdLMH)
        {
            return await _reposiory.GetLoaiMHID(IdLMH);
        }
        #endregion
        #region Xuất File Execl
        public async Task<FileContentResult> Export(string whereStr)
        {
            return await _reposiory.Export(whereStr);
        }
        #endregion
        public async Task<FileContentResult> GetImageById(int IdLMH)
        {
            return await _reposiory.GetImageById(IdLMH);
        }


    }
}
