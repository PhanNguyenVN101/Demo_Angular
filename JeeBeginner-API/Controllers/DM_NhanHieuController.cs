using DpsLibs.Data;
using JeeBeginner.Models.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using JeeBeginner.Models.DM_DVT;
using JeeBeginner.Classes;
using System.Threading.Tasks;
using JeeBeginner.Services.DM_DVT;
using JeeBeginner.Services.Authorization;
using JeeBeginner.Services.PartnerManagement;
using Microsoft.Extensions.Configuration;
using static JeeBeginner.Models.Common.Panigator;
using JeeBeginner.Services;
using System.Security.Cryptography;
using JeeAccount.Classes;
using JeeBeginner.Services.DM_NhanHieu;
using JeeBeginner.Models.DM_NhanHieu;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using PageModel = JeeBeginner.Models.Common.PageModel;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/dm_nhanhieu")]
    [ApiController]
    public class DM_NhanHieuController : ControllerBase
    {
        private readonly IDM_NhanHieuService _service;
        private readonly ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public DM_NhanHieuController(IDM_NhanHieuService dmnhService, IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _service = dmnhService;
            _configuration = configuration;
            _authService = authService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        #region Danh sách nhãn hiệu
        [HttpPost("DM_NhanHieu_List")]
        public async Task<object> DM_NhanHieu_List([FromBody] QueryRequestParams query)
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
                conds.Add("DM_NhanHieu.isDel", 0);
                bool Visible = true;
                Visible = !_authService.IsReadOnlyPermit("1", user.Username);
                string whereStr = "";
                if (query.Filter != null)
                {
                    if (!string.IsNullOrEmpty(query.SearchValue))
                    {
                        whereStr += $"TenNhanHieu like N'%{query.SearchValue}%'";
                    }
                }
                var dmNHlist = await _service.DM_NhanHieu_List(whereStr);
                if (dmNHlist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmNHlist is null)
                    return JsonResultCommon.KhongTonTai();

                pageModel.TotalCount = dmNHlist.Count();
                pageModel.AllPage = (int)Math.Ceiling(dmNHlist.Count() / (decimal)query.Panigator.PageSize);
                pageModel.Size = query.Panigator.PageSize;
                pageModel.Page = query.Panigator.PageIndex;
                dmNHlist = dmNHlist.AsEnumerable().Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize).Take(query.Panigator.PageSize);
                return JsonResultCommon.ThanhCong(dmNHlist, pageModel, Visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
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

        #region Thêm mới nhãn hiệu
        [HttpPost("DM_NhanHieu_Insert")]
        public async Task<object> DM_NhanHieu_Insert(DM_NhanHieuDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                var create = await _service.DM_NhanHieu_Insert(model, user.Id);
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
        #region Sửa nhãn hiệu
        [HttpPost("UpdateNhanHieu")]
        public async Task<object> UpdateNhanHieu(DM_NhanHieuDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdNhanHieu from DM_NhanHieu where IdNhanHieu = {model.IdNhanHieu}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Nhãn hiệu");

                var update = await _service.UpdateNhanHieu(model, user.Id);
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
        #region Xóa nhãn hiệu
        [HttpPost("DeleteNhanHieu")]
        public async Task<object> DeleteNhanHieu(DM_NhanHieuDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdNhanHieu from DM_NhanHieu where IdNhanHieu = {model.IdNhanHieu}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    return BadRequest(MessageReturnHelper.KhongTonTai("Nhãn hiệu"));

                var update = await _service.DeleteNhanHieu(model, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return BadRequest(MessageReturnHelper.Exception(ex));
            }
        }
        #endregion
        #region Xóa nhiều nhãn hiệu
        [HttpPost("DeleteNhanHieus")]
        public async Task<object> DeleteNhanHieus(decimal[] ids)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var update = await _service.DeleteNhanHieus(ids, user.Id);
                if (!update.Susscess)
                {
                    return JsonResultCommon.ThatBai(update.ErrorMessgage);
                }
                return JsonResultCommon.ThanhCong(ids);
            }
            catch (Exception ex)
            {
                return BadRequest(MessageReturnHelper.Exception(ex));
            }
        }
        #endregion
        #region Tìm kiếm tên nhãn hiệu
        [HttpGet("SearchNhanHieu")]
        public async Task<object> SearchNhanHieu(string TenNhanHieu)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.SearchNhanHieu(TenNhanHieu);
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
        #region Tìm mã nhãn hiệu
        [HttpGet("GetNhanHieuID")]
        public async Task<object> GetNhanHieuID(int IdNhanHieu)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetNhanHieuID(IdNhanHieu);
                if (create.IdNhanHieu == 0)
                {
                    return JsonResultCommon.KhongTonTai("NhanHieu");
                }

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        #endregion
        
    }
}
