using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_LoaiMatHang;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeeBeginner.Services.DM_LoaiMatHang
{
    public interface IDM_LoaiMatHangService
    {
        Task<IEnumerable<DM_LoaiMatHangDTO>> DM_LoaiMatHang_List(string whereStr);
        Task<ReturnSqlModel> DM_LoaiMatHang_Insert(DM_LoaiMatHangDTO model, long CreatedBy);
        Task<ReturnSqlModel> UpdateLoaiMatHang(DM_LoaiMatHangDTO model, long CreatedBy);
        Task<ReturnSqlModel> DeleteLMH(DM_LoaiMatHangDTO model, long DeleteBy);
        Task<IEnumerable<DM_LoaiMatHangDTO>> FilterLMH(DM_LoaiMatHangDTO model);
        Task<IEnumerable<DM_LoaiMatHangDTO>> DM_Kho_List();
        Task<DM_LoaiMatHangDTO> GetKhoID(int IdK);
        Task<DM_LoaiMatHangDTO> GetLoaiMHChaID(int IdLMHParent);
        Task<IEnumerable<DM_LoaiMatHangDTO>> LoaiMatHangCha_List();
        Task<DM_LoaiMatHangDTO> GetLoaiMHID(int IdLMH);
        Task<ReturnSqlModel> Deletes(decimal[] ids, long DeleteBy);
        Task<FileContentResult> Export(string whereStr);
        Task<FileContentResult> GetImageById(int IdLMH);
    }
}
