using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_MatHang;
using JeeBeginner.Reponsitories.DM_LyDoTangGiamTS;
using JeeBeginner.Reponsitories.DM_MatHang;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using System;
using JeeBeginner.Models.DM_LoaiMatHang;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JeeBeginner.Services.DM_MatHang
{
    public class DM_MatHangService:IDM_MatHangService
    {
        private IDM_MatHangRepository _reposiory;

        public DM_MatHangService(IDM_MatHangRepository dmmhRepository)
        {
            _reposiory = dmmhRepository;
        }
        #region Danh sách mặt hàng

        public async Task<IEnumerable<DM_MatHangDTO>> DM_MatHang_List(string whereStr)
        {
            return await _reposiory.DM_MatHang_List(whereStr);

        }
        #endregion

        #region Thêm mới mặt hàng
        public async Task<ReturnSqlModel> DM_MatHang_Insert(DM_MatHangDTO model,long CreatedBy)
        {

            return await _reposiory.DM_MatHang_Insert(model, CreatedBy);
        }
        #endregion

        #region Sửa mặt hàng
        public async Task<ReturnSqlModel> UpdateMatHang(DM_MatHangDTO model,long CreatedBy)
        {
            
            return await _reposiory.UpdateMatHang(model, CreatedBy);
        }
        #endregion

        #region Xóa mặt hàng
        public async Task<ReturnSqlModel> DeleteMH(DM_MatHangDTO model, long DeleteBy)
        {
            
            return await _reposiory.DeleteMH(model, DeleteBy);
        }
        #endregion

        #region Tìm kiếm tên mặt hàng
        public async Task<IEnumerable<DM_MatHangDTO>> SearchMatHang(string TenMatHang)
        {
            return await _reposiory.SearchMatHang(TenMatHang);
        }
        #endregion


        #region Tìm mã mặt hàng
        public async Task<DM_MatHangDTO> GetMatHangID(int IdMH)
        {
            return await _reposiory.GetMatHangID(IdMH);
        }
        #endregion
        #region Xóa nhiều mặt hàng
        public async Task<ReturnSqlModel> DeleteMHs(decimal[] ids, long DeleteBy)
        {
            
            return await _reposiory.DeleteMHs(ids, DeleteBy);
        }
        #endregion
        public async Task<bool> ImportFromExcel(IFormFile file, long CreatedBy)
        {
            return await _reposiory.ImportFromExcel(file, CreatedBy);
        }

        public async Task<FileContentResult> TaiFileMau()
        {
            return await _reposiory.TaiFileMau();
        }
    }
}
