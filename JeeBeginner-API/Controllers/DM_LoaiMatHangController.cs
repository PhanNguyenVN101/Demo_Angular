using DpsLibs.Data;
using JeeBeginner.Models.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using JeeBeginner.Models.DM_LoaiMatHang;
using JeeBeginner.Classes;
using System.Threading.Tasks;
using JeeBeginner.Services.DM_LoaiMatHang;
using JeeBeginner.Services.Authorization;
using JeeBeginner.Services.PartnerManagement;
using Microsoft.Extensions.Configuration;
using static JeeBeginner.Models.Common.Panigator;
using JeeBeginner.Services;
using System.Security.Cryptography;
using JeeAccount.Classes;
using System.Collections;
using System.IO;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/dm_loaimathang")]
    [ApiController]
    public class DM_LoaiMatHangController : ControllerBase
    {
        private readonly IDM_LoaiMatHangService _service;
        private readonly ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public DM_LoaiMatHangController(IDM_LoaiMatHangService dmLoaiMatHangService, IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _service = dmLoaiMatHangService;
            _configuration = configuration;
            _authService = authService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        #region Danh sách Loại mặt hàng
        [HttpPost("DM_LoaiMatHang_List")]
        public async Task<object> DM_LoaiMatHang_List([FromBody] QueryRequestParams query)
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
                conds.Add("DM_LoaiMatHang.isDel", 0);
                bool Visible = true;
                Visible = !_authService.IsReadOnlyPermit("1", user.Username);
                string whereStr = "";
                if (query.Filter != null)
                {
                    if (!string.IsNullOrEmpty(query.SearchValue))
                    {
                        whereStr += $"Mota like N'%{query.SearchValue}%' or " +
                            
                            $"TenLMH like N'%{query.SearchValue}%'";
                    }
                }
                var dmLoaiMatHanglist = await _service.DM_LoaiMatHang_List(whereStr);
                if (dmLoaiMatHanglist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmLoaiMatHanglist is null)
                    return JsonResultCommon.KhongTonTai();
                pageModel.TotalCount = dmLoaiMatHanglist.Count();
                pageModel.AllPage = (int)Math.Ceiling(dmLoaiMatHanglist.Count() / (decimal)query.Panigator.PageSize);
                pageModel.Size = query.Panigator.PageSize;
                pageModel.Page = query.Panigator.PageIndex;
                dmLoaiMatHanglist = dmLoaiMatHanglist.AsEnumerable().Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize).Take(query.Panigator.PageSize);
                return JsonResultCommon.ThanhCong(dmLoaiMatHanglist, pageModel, Visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Danh sách kho
        [HttpGet("DM_Kho_List")]
        public async Task<object> DM_Kho_List()
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var dmKholist = await _service.DM_Kho_List();
                if (dmKholist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmKholist is null)
                    return JsonResultCommon.KhongTonTai();
                return JsonResultCommon.ThanhCong(dmKholist);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Danh sách loại mặt hàng cha
        [HttpGet("LoaiMatHangCha_List")]
        public async Task<object> LoaiMatHangCha_List()
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var lmhclist = await _service.LoaiMatHangCha_List();
                if (lmhclist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (lmhclist is null)
                    return JsonResultCommon.KhongTonTai();
                return JsonResultCommon.ThanhCong(lmhclist);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Thêm mới Loại mặt hàng
        [HttpPost("DM_LoaiMatHang_Insert")]
        public async Task<object> DM_LoaiMatHang_Insert(DM_LoaiMatHangDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                var create = await _service.DM_LoaiMatHang_Insert(model, user.Id);
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
        #region Sửa Loại mặt hàng
        [HttpPost("UpdateLoaiMatHang")]
        public async Task<object> UpdateLoaiMatHang(DM_LoaiMatHangDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdLMH from DM_LoaiMatHang where IdLMH = {model.IdLMH}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("Loại mặt hàng");

                var update = await _service.UpdateLoaiMatHang(model, user.Id);
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
        #region Xóa Loại mặt hàng
        [HttpPost("DeleteLMH")]
        public async Task<object> DeleteLMH(DM_LoaiMatHangDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdLMH from DM_LoaiMatHang where IdLMH = {model.IdLMH}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    return BadRequest(MessageReturnHelper.KhongTonTai("Loại mặt hàng"));

                var update = await _service.DeleteLMH(model, user.Id);
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
        #region Xóa nhiều loại mặt hàng
        [HttpPost("Deletes")]
        public async Task<object> Deletes(decimal[] ids)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var update = await _service.Deletes(ids, user.Id);
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
        #region Lọc mặt hàng
        [HttpPost("FilterLMH")]
        public async Task<object> FilterLMH(DM_LoaiMatHangDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.FilterLMH(model);
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
        #region Tìm mã kho
        [HttpGet("GetKhoID")]
        public async Task<object> GetKhoID(int IdK)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetKhoID(IdK);
                if (create.IdKho == 0)
                {
                    return JsonResultCommon.KhongTonTai("Kho");
                }

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        #endregion
        #region Tìm kiếm mã loại mặt hàng cha
        [HttpGet("GetLoaiMHChaID")]
        public async Task<object> GetLoaiMHChaID(int IdLMHParent)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetLoaiMHChaID(IdLMHParent);
                if (create.IdLMH == 0)
                {
                    return JsonResultCommon.KhongTonTai("Loai mat hang cha");
                }

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Tìm kiếm mã loại mặt hàng
        [HttpGet("GetLoaiMHID")]
        public async Task<object> GetLoaiMHID(int IdLMH)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetLoaiMHID(IdLMH);
                if (create.IdLMH == 0)
                {
                    return JsonResultCommon.ThatBai("Loai mat hang");
                }

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Xuất File Excel
        [HttpGet("export")]
        public async Task<object> ExportToExcel(string whereStr)
        {
            try
            {
                //var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                //if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var update = await _service.Export(whereStr);

                return update;
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        [HttpPost("Upload")]
        public async Task<object> Upload(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {


                    var currentDirectory = Directory.GetCurrentDirectory();
                    var parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
                    if (parentDirectory == null)
                    {
                        return BadRequest("Failed to get parent directory.");
                    }

                    var uploadsFolder = Path.Combine(parentDirectory, "JeeBeginner-API", "Images");
                    var filePath = Path.Combine(uploadsFolder, file.FileName);
                    if (!System.IO.File.Exists(filePath))
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    

                    return JsonResultCommon.ThanhCong(filePath);
                }
                else
                {
                    return BadRequest("No file uploaded.");
                }
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
        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(int IdLMH)
        {
            var imageFile = await _service.GetImageById(IdLMH);

            if (imageFile == null)
            {
                return NotFound("Image not found");
            }

            return imageFile;
        }
    }
}
