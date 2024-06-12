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
using JeeBeginner.Services.DM_DoiTacBaoHiem;
using JeeBeginner.Models.DM_DoiTacBaoHiem;
using System.Collections;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/dm_doitacbaohiem")]
    [ApiController]
    public class DM_DoiTacBaoHiemController : ControllerBase
    {
        private readonly IDM_DoiTacBaoHiemService _service;
        private readonly ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public DM_DoiTacBaoHiemController(IDM_DoiTacBaoHiemService dmnhService, IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _service = dmnhService;
            _configuration = configuration;
            _authService = authService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        #region Danh sách đối tác bảo hiểm
        [HttpPost("DM_DoiTacBaoHiem_List")]
        public async Task<object> DM_DoiTacBaoHiem_List([FromBody] QueryRequestParams query)
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
                conds.Add("DM_DoiTacBaoHiem.isDel", 0);
                bool Visible = true;
                Visible = !_authService.IsReadOnlyPermit("1", user.Username);
                string whereStr = "";
                if (query.Filter != null)
                {
                    if (!string.IsNullOrEmpty(query.SearchValue))
                    {
                        whereStr += $"SoDT like N'%{query.SearchValue}%' or NguoiLienHe like N'%{query.SearchValue}%' or TenDonVi like N'%{query.SearchValue}%'";
                    }
                }
                var dmDTBHlist = await _service.DM_DoiTacBaoHiem_List(whereStr);
                if (dmDTBHlist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmDTBHlist is null)
                    return JsonResultCommon.KhongTonTai();
                pageModel.TotalCount = dmDTBHlist.Count();
                pageModel.AllPage = (int)Math.Ceiling(dmDTBHlist.Count() / (decimal)query.Panigator.PageSize);
                pageModel.Size = query.Panigator.PageSize;
                pageModel.Page = query.Panigator.PageIndex;
                dmDTBHlist = dmDTBHlist.AsEnumerable().Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize).Take(query.Panigator.PageSize);
                return JsonResultCommon.ThanhCong(dmDTBHlist, pageModel, Visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion


        #region Thêm mới đối tác bảo hiểm
        [HttpPost("DM_DoiTacBaoHiem_Insert")]
        public async Task<object> DM_DoiTacBaoHiem_Insert(DM_DoiTacBaoHiemDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                var create = await _service.DM_DoiTacBaoHiem_Insert(model, user.Id);
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
        #region Sửa đối tác bảo hiểm
        [HttpPost("UpdateDoiTacBaoHiem")]
        public async Task<object> UpdateDoiTacBaoHiem(DM_DoiTacBaoHiemDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select Id_DV from DM_DoiTacBaoHiem where Id_DV = {model.Id_DV}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("đối tác bảo hiểm");

                var update = await _service.UpdateDoiTacBaoHiem(model, user.Id);
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
        #region Xóa đối tác bảo hiểm
        [HttpPost("DeleteDoiTacBaoHiem")]
        public async Task<object> DeleteDoiTacBaoHiem(DM_DoiTacBaoHiemDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select Id_DV from DM_DoiTacBaoHiem where Id_DV = {model.Id_DV}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    return BadRequest(MessageReturnHelper.KhongTonTai("đối tác bảo hiểm"));

                var update = await _service.DeleteDoiTacBaoHiem(model);
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
        
        #region Tìm kiếm tên đối tác bảo hiểm
        [HttpGet("SearchDoiTacBaoHiem")]
        public async Task<object> SearchDoiTacBaoHiem(string TenDonVi)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.SearchDoiTacBaoHiem(TenDonVi);
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
        #region Tìm mã đối tác bảo hiểm
        [HttpGet("GetDoiTacBaoHiemID")]
        public async Task<object> GetDoiTacBaoHiemID(int Id_DV)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetDoiTacBaoHiemID(Id_DV);
                if (create.Id_DV == 0)
                {
                    return JsonResultCommon.KhongTonTai("DoiTacBaoHiem");
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
        public async Task<object> ImportFromExcel(IFormFile file)
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
