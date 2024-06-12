using DpsLibs.Data;
using JeeBeginner.Models.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using JeeBeginner.Models.DM_NhomTaiSan;
using JeeBeginner.Classes;
using System.Threading.Tasks;
using JeeBeginner.Services.DM_NhomTaiSan;
using JeeBeginner.Services.Authorization;
using JeeBeginner.Services.PartnerManagement;
using Microsoft.Extensions.Configuration;
using static JeeBeginner.Models.Common.Panigator;
using JeeBeginner.Services;
using System.Security.Cryptography;
using JeeAccount.Classes;
using JeeBeginner.Services.DM_NhomTaiSan;
using System.Collections;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/dm_nhomtaisan")]
    [ApiController]
    public class DM_NhomTaiSanController : ControllerBase
    {
        private readonly IDM_NhomTaiSanService _service;
        private readonly ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public DM_NhomTaiSanController(IDM_NhomTaiSanService dmltsService, IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _service = dmltsService;
            _configuration = configuration;
            _authService = authService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        #region Danh sách nhóm tài sản
        [HttpPost("DM_NhomTaiSan_List")]
        public async Task<object> DM_NhomTaiSan_List([FromBody] QueryRequestParams query)
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
                conds.Add("DM_NhomTaiSan.isDel", 0);
                bool Visible = true;
                Visible = !_authService.IsReadOnlyPermit("1", user.Username);
                string whereStr = "";
                if (query.Filter != null)
                {
                    if (!string.IsNullOrEmpty(query.SearchValue))
                    {
                        whereStr += $"MaNhom like N'%{query.SearchValue}%' or TenNhom like N'%{query.SearchValue}%'";
                    }
                }
                var dmNhomTaiSanlist = await _service.DM_NhomTaiSan_List(whereStr);
                if (dmNhomTaiSanlist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmNhomTaiSanlist is null)
                    return JsonResultCommon.KhongTonTai();
                pageModel.TotalCount = dmNhomTaiSanlist.Count();
                pageModel.AllPage = (int)Math.Ceiling(dmNhomTaiSanlist.Count() / (decimal)query.Panigator.PageSize);
                pageModel.Size = query.Panigator.PageSize;
                pageModel.Page = query.Panigator.PageIndex;
                dmNhomTaiSanlist = dmNhomTaiSanlist.AsEnumerable().Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize).Take(query.Panigator.PageSize);
                return JsonResultCommon.ThanhCong(dmNhomTaiSanlist, pageModel, Visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        
        
        #region Thêm mới nhóm tài sản
        [HttpPost("DM_NhomTaiSan_Insert")]
        public async Task<object> DM_NhomTaiSan_Insert(DM_NhomTaiSanDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                var create = await _service.DM_NhomTaiSan_Insert(model);
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
        #region Sửa nhóm tài sản
        [HttpPost("UpdateNhomTaiSan")]
        public async Task<object> UpdateNhomTaiSan(DM_NhomTaiSanDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdPNTS from TS_DM_PhanNhomTS where IdPNTS = {model.IdPNTS}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("nhóm tài sản");

                var update = await _service.UpdateNhomTaiSan(model);
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
        #region Xóa nhóm tài sản
        [HttpPost("DeleteNhomTaiSan")]
        public async Task<object> DeleteNhomTaiSan(DM_NhomTaiSanDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                string sqlCheckCode = $"select IdPNTS from TS_DM_PhanNhomTS where IdPNTS = {model.IdPNTS}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    return BadRequest(MessageReturnHelper.KhongTonTai("nhóm tài sản"));
                if (model.TrangThai == true)
                {
                    return JsonResultCommon.Trung("Còn hiệu lực");
                }
                else
                {
                    var update = await _service.DeleteNhomTaiSan(model);
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
        
        #region Tìm kiếm tên nhóm tài sản
        [HttpGet("SearchNhomTaiSan")]
        public async Task<object> SearchNhomTaiSan(string TenNhomTaiSan)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.SearchNhomTaiSan(TenNhomTaiSan);
                //if (int.Parse(create.TenLMH) == 0)
                //{
                //    return JsonResultCommon.KhongTonTai("Các nhóm mặt hàng này");
                //}

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Tìm mã nhóm tài sản
        [HttpGet("GetNhomTaiSanID")]
        public async Task<object> GetNhomTaiSanID(int IdPNTS)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetNhomTaiSanID(IdPNTS);
                if (create.IdPNTS == 0)
                {
                    return JsonResultCommon.KhongTonTai("NhomTaiSan");
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
