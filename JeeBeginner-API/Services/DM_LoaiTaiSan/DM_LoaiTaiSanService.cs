using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_LoaiTaiSan;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using System;
using JeeBeginner.Models.DM_DVT;
using JeeBeginner.Reponsitories.DM_DVT;
using JeeBeginner.Reponsitories.DM_LoaiTaiSan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JeeBeginner.Services.DM_LoaiTaiSan
{
    public class DM_LoaiTaiSanService:IDM_LoaiTaiSanService
    {
        private IDM_LoaiTaiSanRepository _reposiory;

        public DM_LoaiTaiSanService(IDM_LoaiTaiSanRepository dmLoaiTSRepository)
        {
            _reposiory = dmLoaiTSRepository;
        }
        #region Danh sách loại tài sản

        public async Task<IEnumerable<DM_LoaiTaiSanDTO>> DM_LoaiTaiSan_List(string whereStr)
        {
            return await _reposiory.DM_LoaiTaiSan_List(whereStr);

        }
        #endregion

        #region Thêm mới loại tài sản
        public async Task<ReturnSqlModel> DM_LoaiTaiSan_Insert(DM_LoaiTaiSanDTO model)
        {

            return await _reposiory.DM_LoaiTaiSan_Insert(model);
        }
        #endregion

        #region Sửa loại tài sản
        public async Task<ReturnSqlModel> UpdateLoaiTaiSan(DM_LoaiTaiSanDTO model)
        {

            return await _reposiory.UpdateLoaiTaiSan(model);
        }
        #endregion

        #region Xóa loại tài sản
        public async Task<ReturnSqlModel> DeleteLoaiTaiSan(DM_LoaiTaiSanDTO model)
        {

            return await _reposiory.DeleteLoaiTaiSan(model);
        }
        #endregion

        #region Tìm kiếm tên loại tài sản
        public async Task<IEnumerable<DM_LoaiTaiSanDTO>> SearchLoaiTaiSan(string TenLoai)
        {
            return await _reposiory.SearchLoaiTaiSan(TenLoai);
        }
        #endregion


        #region Tìm mã loại tài sản
        public async Task<DM_LoaiTaiSanDTO> GetLoaiTaiSanID(int IdLoaiTS)
        {
            return await _reposiory.GetLoaiTaiSanID(IdLoaiTS);
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
