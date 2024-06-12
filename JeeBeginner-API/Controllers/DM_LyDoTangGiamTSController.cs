using DpsLibs.Data;
using JeeBeginner.Models.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using JeeBeginner.Models.DM_LyDoTangGiamTS;
using JeeBeginner.Classes;
using System.Threading.Tasks;
using JeeBeginner.Services.DM_LyDoTangGiamTS;
using JeeBeginner.Services.Authorization;
using JeeBeginner.Services.PartnerManagement;
using Microsoft.Extensions.Configuration;
using static JeeBeginner.Models.Common.Panigator;
using JeeBeginner.Services;
using System.Security.Cryptography;
using JeeAccount.Classes;
using System.Collections;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/dm_lydotanggiamts")]
    [ApiController]
    public class DM_LyDoTangGiamTSController : ControllerBase
    {
        private readonly IDM_LyDoTangGiamTSService _service;
        private readonly ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public DM_LyDoTangGiamTSController(IDM_LyDoTangGiamTSService dmltsService, IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _service = dmltsService;
            _configuration = configuration;
            _authService = authService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        #region Danh sách lý do tăng giảm tài sản
        [HttpPost("DM_LyDoTangGiamTS_List")]
        public async Task<object> DM_LyDoTangGiamTS_List([FromBody] QueryRequestParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                query = query == null ? new QueryRequestParams() : query;
                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                ErrorModel error = new ErrorModel();
                SqlConditions conds = new SqlConditions();
                conds.Add("DM_LyDoTangGiamTS.isDel", 0);
                bool Visible = true;
                Visible = !_authService.IsReadOnlyPermit("1", user.Username);
                string whereStr = "";
                if (query.Filter != null)
                {
                    if (!string.IsNullOrEmpty(query.SearchValue))
                    {
                        whereStr += $"MaTangGiam like N'%{query.SearchValue}%' or TenTangGiam like N'%{query.SearchValue}%'";
                    }
                }
                var dmLyDoTangGiamTSlist = await _service.DM_LyDoTangGiamTS_List(whereStr);
                if (dmLyDoTangGiamTSlist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmLyDoTangGiamTSlist is null)
                    return JsonResultCommon.KhongTonTai();
                pageModel.TotalCount = dmLyDoTangGiamTSlist.Count();
                pageModel.AllPage = (int)Math.Ceiling(dmLyDoTangGiamTSlist.Count() / (decimal)query.Panigator.PageSize);
                pageModel.Size = query.Panigator.PageSize;
                pageModel.Page = query.Panigator.PageIndex;
                dmLyDoTangGiamTSlist = dmLyDoTangGiamTSlist.AsEnumerable().Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize).Take(query.Panigator.PageSize);
                return JsonResultCommon.ThanhCong(dmLyDoTangGiamTSlist, pageModel, Visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        
        
        #region Thêm mới lý do tăng giảm tài sản
        [HttpPost("DM_LyDoTangGiamTS_Insert")]
        public async Task<object> DM_LyDoTangGiamTS_Insert(DM_LyDoTangGiamTSDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                var create = await _service.DM_LyDoTangGiamTS_Insert(model);
                if (!create.Susscess)
                {
                    return JsonResultCommon.ThatBai(create.ErrorMessgage);
                }

                return JsonResultCommon.ThanhCong(model);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Sửa lý do tăng giảm tài sản
        [HttpPost("UpdateLyDoTangGiamTS")]
        public async Task<object> UpdateLyDoTangGiamTS(DM_LyDoTangGiamTSDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdRow from TS_DM_LyDoTangGiamTS where IdRow = {model.IdRow}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("lý do tăng giảm tài sản");

                var update = await _service.UpdateLyDoTangGiamTS(model);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(model);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Xóa lý do tăng giảm tài sản
        [HttpPost("DeleteLyDoTangGiamTS")]
        public async Task<object> DeleteLyDoTangGiamTS(DM_LyDoTangGiamTSDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdRow from TS_DM_LyDoTangGiamTS where IdRow = {model.IdRow}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    return BadRequest(MessageReturnHelper.KhongTonTai("lý do tăng giảm tài sản"));
                if (model.TrangThai == true)
                {
                    return JsonResultCommon.Trung("Còn hiệu lực");
                }
                else
                {
                    var update = await _service.DeleteLyDoTangGiamTS(model);
                    if (!update.Susscess)
                    {
                        return JsonResultCommon.ThatBai(update.ErrorMessgage);
                    }
                }
                
                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return BadRequest(MessageReturnHelper.Exception(ex));
            }
        }
        #endregion
        
        #region Tìm kiếm tên lý do tăng giảm tài sản
        [HttpGet("SearchLyDoTangGiamTS")]
        public async Task<object> SearchLyDoTangGiamTS(string TenTangGiam)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.SearchLyDoTangGiamTS(TenTangGiam);
                //if (int.Parse(create.TenLMH) == 0)
                //{
                //    return JsonResultCommon.KhongTonTai("Các loại mặt hàng này");
                //}

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Tìm mã lý do tăng giảm tài sản
        [HttpGet("GetLyDoTangGiamTSID")]
        public async Task<object> GetLyDoTangGiamTSID(int IdLoaiTS)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetLyDoTangGiamTSID(IdLoaiTS);
                if (create.IdRow == 0)
                {
                    return JsonResultCommon.KhongTonTai("LyDoTangGiamTS");
                }

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        #endregion
        [HttpPost("import")]
        public async Task<object> ImportDVTFromExcel(IFormFile file)
        {
            try
            {
                bool result = await _service.ImportFromExcel(file, 66621);
                if (result == false)
                {
                    return JsonResultCommon.ThatBai("Import thất bại");
                }

                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        [HttpGet("TaiFileMau")]
        public async Task<object> TaiFileMau()
        {
            try
            {
                //var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                //if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var update = await _service.TaiFileMau();

                return update;
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        [HttpGet("IsReadOnlyPermitAccountRole")]
        public async Task<object> IsReadOnlyPermitAccountRole(string roleName)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                bool Visible = true;
                Visible = !_authService.IsReadOnlyPermit(roleName, user.Username);
                return Visible;
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
    }
}
