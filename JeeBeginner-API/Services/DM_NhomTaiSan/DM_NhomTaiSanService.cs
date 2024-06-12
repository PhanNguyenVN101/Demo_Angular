using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_NhomTaiSan;
using JeeBeginner.Reponsitories.DM_NhomTaiSan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Services.DM_NhomTaiSan
{
    public class DM_NhomTaiSanService: IDM_NhomTaiSanService
    {
        private IDM_NhomTaiSanRepository _reposiory;

        public DM_NhomTaiSanService(IDM_NhomTaiSanRepository dmLoaiTSRepository)
        {
            _reposiory = dmLoaiTSRepository;
        }
        #region Danh sách nhóm tài sản

        public async Task<IEnumerable<DM_NhomTaiSanDTO>> DM_NhomTaiSan_List(string whereStr)
        {
            return await _reposiory.DM_NhomTaiSan_List(whereStr);

        }
        #endregion

        #region Thêm mới nhóm tài sản
        public async Task<ReturnSqlModel> DM_NhomTaiSan_Insert(DM_NhomTaiSanDTO model)
        {

            return await _reposiory.DM_NhomTaiSan_Insert(model);
        }
        #endregion

        #region Sửa nhóm tài sản
        public async Task<ReturnSqlModel> UpdateNhomTaiSan(DM_NhomTaiSanDTO model)
        {

            return await _reposiory.UpdateNhomTaiSan(model);
        }
        #endregion

        #region Xóa nhóm tài sản
        public async Task<ReturnSqlModel> DeleteNhomTaiSan(DM_NhomTaiSanDTO model)
        {

            return await _reposiory.DeleteNhomTaiSan(model);
        }
        #endregion

        #region Tìm kiếm tên nhóm tài sản
        public async Task<IEnumerable<DM_NhomTaiSanDTO>> SearchNhomTaiSan(string TenNhom)
        {
            return await _reposiory.SearchNhomTaiSan(TenNhom);
        }
        #endregion


        #region Tìm mã nhóm tài sản
        public async Task<DM_NhomTaiSanDTO> GetNhomTaiSanID(int IdLoaiTS)
        {
            return await _reposiory.GetNhomTaiSanID(IdLoaiTS);
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
