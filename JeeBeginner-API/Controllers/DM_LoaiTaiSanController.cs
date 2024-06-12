using DpsLibs.Data;
using JeeBeginner.Models.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using JeeBeginner.Models.DM_LoaiTaiSan;
using JeeBeginner.Classes;
using System.Threading.Tasks;
using JeeBeginner.Services.DM_LoaiTaiSan;
using JeeBeginner.Services.Authorization;
using JeeBeginner.Services.PartnerManagement;
using Microsoft.Extensions.Configuration;
using static JeeBeginner.Models.Common.Panigator;
using JeeBeginner.Services;
using System.Security.Cryptography;
using JeeAccount.Classes;
using JeeBeginner.Services.DM_LoaiTaiSan;
using System.Collections;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/dm_loaitaisan")]
    [ApiController]
    public class DM_LoaiTaiSanController : ControllerBase
    {
        private readonly IDM_LoaiTaiSanService _service;
        private readonly ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public DM_LoaiTaiSanController(IDM_LoaiTaiSanService dmltsService, IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _service = dmltsService;
            _configuration = configuration;
            _authService = authService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        #region Danh sách loại tài sản
        [HttpPost("DM_LoaiTaiSan_List")]
        public async Task<object> DM_LoaiTaiSan_List([FromBody] QueryRequestParams query)
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
                conds.Add("DM_LoaiTaiSan.isDel", 0);
                bool Visible = true;
                Visible = !_authService.IsReadOnlyPermit("1", user.Username);
                string whereStr = "";
                if (query.Filter != null)
                {
                    if (!string.IsNullOrEmpty(query.SearchValue))
                    {
                        whereStr += $"MaLoai like N'%{query.SearchValue}%' or TenLoai like N'%{query.SearchValue}%'";
                    }
                }
                var dmLoaiTaiSanlist = await _service.DM_LoaiTaiSan_List(whereStr);
                if (dmLoaiTaiSanlist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmLoaiTaiSanlist is null)
                    return JsonResultCommon.KhongTonTai();
                pageModel.TotalCount = dmLoaiTaiSanlist.Count();
                pageModel.AllPage = (int)Math.Ceiling(dmLoaiTaiSanlist.Count() / (decimal)query.Panigator.PageSize);
                pageModel.Size = query.Panigator.PageSize;
                pageModel.Page = query.Panigator.PageIndex;
                dmLoaiTaiSanlist = dmLoaiTaiSanlist.AsEnumerable().Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize).Take(query.Panigator.PageSize);
                return JsonResultCommon.ThanhCong(dmLoaiTaiSanlist, pageModel, Visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        
        
        #region Thêm mới loại tài sản
        [HttpPost("DM_LoaiTaiSan_Insert")]
        public async Task<object> DM_LoaiTaiSan_Insert(DM_LoaiTaiSanDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                var create = await _service.DM_LoaiTaiSan_Insert(model);
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
        #region Sửa loại tài sản
        [HttpPost("UpdateLoaiTaiSan")]
        public async Task<object> UpdateLoaiTaiSan(DM_LoaiTaiSanDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdLoaiTS from TS_DM_LoaiTS where IdLoaiTS = {model.IdLoaiTS}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("loại tài sản");

                var update = await _service.UpdateLoaiTaiSan(model);
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
        #region Xóa loại tài sản
        [HttpPost("DeleteLoaiTaiSan")]
        public async Task<object> DeleteLoaiTaiSan(DM_LoaiTaiSanDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                string sqlCheckCode = $"select IdLoaiTS from TS_DM_LoaiTS where IdLoaiTS = {model.IdLoaiTS}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    return BadRequest(MessageReturnHelper.KhongTonTai("loại tài sản"));
                if (model.TrangThai == true)
                {
                    return JsonResultCommon.Trung("Còn hiệu lực");
                }
                else
                {
                    var update = await _service.DeleteLoaiTaiSan(model);
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
        
        #region Tìm kiếm tên loại tài sản
        [HttpGet("SearchLoaiTaiSan")]
        public async Task<object> SearchLoaiTaiSan(string TenLoaiTaiSan)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.SearchLoaiTaiSan(TenLoaiTaiSan);
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
        #region Tìm mã loại tài sản
        [HttpGet("GetLoaiTaiSanID")]
        public async Task<object> GetLoaiTaiSanID(int IdLoaiTS)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetLoaiTaiSanID(IdLoaiTS);
                if (create.IdLoaiTS == 0)
                {
                    return JsonResultCommon.KhongTonTai("LoaiTaiSan");
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
