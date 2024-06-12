using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_DoiTacBaoHiem;
using JeeBeginner.Reponsitories.DM_XuatXu;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JeeBeginner.Services.DM_DoiTacBaoHiem
{
    public class DM_DoiTacBaoHiemService:IDM_DoiTacBaoHiemService
    {
        private IDM_DoiTacBaoHiemRepository _reposiory;

        public DM_DoiTacBaoHiemService(IDM_DoiTacBaoHiemRepository dmXuatXuRepository)
        {
            _reposiory = dmXuatXuRepository;
        }
        #region Danh sách đối tác bảo hiểm

        public async Task<IEnumerable<DM_DoiTacBaoHiemDTO>> DM_DoiTacBaoHiem_List(string whereStr)
        {
            return await _reposiory.DM_DoiTacBaoHiem_List(whereStr);

        }
        #endregion


        #region Thêm mới đối tác bảo hiểm
        public async Task<ReturnSqlModel> DM_DoiTacBaoHiem_Insert(DM_DoiTacBaoHiemDTO model, long CreatedBy)
        {
            return await _reposiory.DM_DoiTacBaoHiem_Insert(model, CreatedBy);
        }
        #endregion

        #region Sửa đối tác bảo hiểm
        public async Task<ReturnSqlModel> UpdateDoiTacBaoHiem(DM_DoiTacBaoHiemDTO model, long CreatedBy)
        {
            
            return await _reposiory.UpdateDoiTacBaoHiem(model, CreatedBy);
        }
        #endregion

        #region Xóa đối tác bảo hiểm
        public async Task<ReturnSqlModel> DeleteDoiTacBaoHiem(DM_DoiTacBaoHiemDTO model)
        {
           
            return await _reposiory.DeleteDoiTacBaoHiem(model);
        }
        #endregion

        #region Tìm kiếm đối tác bảo hiểm
        public async Task<IEnumerable<DM_DoiTacBaoHiemDTO>> SearchDoiTacBaoHiem(string TenDonVi)
        {

            return await _reposiory.SearchDoiTacBaoHiem(TenDonVi);

        }
        #endregion


        #region Tìm mã đối tác bảo hiểm
        public async Task<DM_DoiTacBaoHiemDTO> GetDoiTacBaoHiemID(int Id_DV)
        {
            return await _reposiory.GetDoiTacBaoHiemID(Id_DV);

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
