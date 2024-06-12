using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_LyDoTangGiamTS;
using JeeBeginner.Reponsitories.DM_LoaiTaiSan;
using JeeBeginner.Reponsitories.DM_LyDoTangGiamTS;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JeeBeginner.Services.DM_LyDoTangGiamTS
{
    public class DM_LyDoTangGiamTSService:IDM_LyDoTangGiamTSService
    {
        private IDM_LyDoTangGiamTSRepository _reposiory;

        public DM_LyDoTangGiamTSService(IDM_LyDoTangGiamTSRepository dmLoaiTSRepository)
        {
            _reposiory = dmLoaiTSRepository;
        }
        #region Danh sách lý do tăng giảm tài sản

        public async Task<IEnumerable<DM_LyDoTangGiamTSDTO>> DM_LyDoTangGiamTS_List(string whereStr)
        {
            return await _reposiory.DM_LyDoTangGiamTS_List(whereStr);

        }
        #endregion

        

        #region Thêm mới lý do tăng giảm tài sản
        public async Task<ReturnSqlModel> DM_LyDoTangGiamTS_Insert(DM_LyDoTangGiamTSDTO model)
        {
            return await _reposiory.DM_LyDoTangGiamTS_Insert(model);
        }
        #endregion

        #region Sửa lý do tăng giảm tài sản
        public async Task<ReturnSqlModel> UpdateLyDoTangGiamTS(DM_LyDoTangGiamTSDTO model)
        {

            return await _reposiory.UpdateLyDoTangGiamTS(model);
        }
        #endregion

        #region Xóa lý do tăng giảm tài sản
        public async Task<ReturnSqlModel> DeleteLyDoTangGiamTS(DM_LyDoTangGiamTSDTO model)
        {

            return await _reposiory.DeleteLyDoTangGiamTS(model);
        }
        #endregion

        #region Tìm kiếm tên lý do tăng giảm tài sản
        public async Task<IEnumerable<DM_LyDoTangGiamTSDTO>> SearchLyDoTangGiamTS(string TenTangGiam)
        {
            return await _reposiory.SearchLyDoTangGiamTS(TenTangGiam);
        }
        #endregion


        #region Tìm mã lý do tăng giảm tài sản
        public async Task<DM_LyDoTangGiamTSDTO> GetLyDoTangGiamTSID(int IdRow)
        {
            return await _reposiory.GetLyDoTangGiamTSID(IdRow);
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
