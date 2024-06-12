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
using JeeBeginner.Services.DM_MatHang;
using JeeBeginner.Models.DM_MatHang;
using JeeBeginner.Models.DM_LoaiMatHang;
using JeeBeginner.Services.DM_XuatXu;
using JeeBeginner.Services.DM_LoaiMatHang;
using JeeBeginner.Services.DM_NhanHieu;
using JeeBeginner.Services.DM_DVT;
using System.Collections;
using System.IO;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/dm_mathang")]
    [ApiController]
    public class DM_MatHangController : ControllerBase
    {
        private readonly IDM_MatHangService _service;
        private readonly IDM_XuatXuService _serviceXS;
        private readonly IDM_LoaiMatHangService _serviceLMH;
        private readonly IDM_NhanHieuService _serviceNH;
        private readonly IDM_DVTService _serviceDVT;
        private readonly ICustomAuthorizationService _authService;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _jwtSecret;

        public DM_MatHangController(IDM_MatHangService dmmhService, IDM_XuatXuService dmxxService,
            IDM_LoaiMatHangService dmlmhService, IDM_NhanHieuService dmnhService, IDM_DVTService dmdvtService,
            IConfiguration configuration, ICustomAuthorizationService authService)
        {
            _service = dmmhService;
            _serviceXS = dmxxService;
            _serviceLMH = dmlmhService;
            _serviceNH=dmnhService;
            _serviceDVT = dmdvtService;
            _configuration = configuration;
            _authService = authService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }
        #region Danh sách Đơn vị tính
        [HttpGet("DM_DVT_List")]
        public async Task<object> DM_DVT_List()
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var dmDVTlist = await _serviceDVT.DM_DVT_List(null);
                if (dmDVTlist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmDVTlist is null)
                    return JsonResultCommon.KhongTonTai();
                return JsonResultCommon.ThanhCong(dmDVTlist);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Danh sách nhãn hiệu
        [HttpGet("DM_NhanHieu_List")]
        public async Task<object> DM_NhanHieu_List()
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");


                var dmDVTlist = await _serviceNH.DM_NhanHieu_List(null);
                if (dmDVTlist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmDVTlist is null)
                    return JsonResultCommon.KhongTonTai();

                
                return JsonResultCommon.ThanhCong(dmDVTlist);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Danh sách Loại mặt hàng
        [HttpGet("DM_LoaiMatHang_List")]
        public async Task<object> DM_LoaiMatHang_List()
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var dmLoaiMatHanglist = await _serviceLMH.DM_LoaiMatHang_List(null);
                if (dmLoaiMatHanglist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmLoaiMatHanglist is null)
                    return JsonResultCommon.KhongTonTai();
                return JsonResultCommon.ThanhCong(dmLoaiMatHanglist);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        #region Danh sách xuất xứ
        [HttpGet("DM_XuatXu_List")]
        public async Task<object> DM_XuatXu_List()
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var dmDVTlist = await _serviceXS.DM_XuatXu_List(null);
                if (dmDVTlist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmDVTlist is null)
                    return JsonResultCommon.KhongTonTai();
                return JsonResultCommon.ThanhCong(dmDVTlist);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        //#region Danh sách xuất xứ
        //[HttpPost("DM_XuatXu_List")]
        //public async Task<object> DM_XuatXu_List()
        //{
        //    try
        //    {
        //        var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
        //        if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

        //        var dmDVTlist = await _service.DM_XuatXu_List();
        //        if (dmDVTlist.Count() == 0)
        //            return JsonResultCommon.ThatBai("Không có dữ liệu");
        //        if (dmDVTlist is null)
        //            return JsonResultCommon.KhongTonTai();
        //        return JsonResultCommon.ThanhCong(dmDVTlist);
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonResultCommon.Exception(ex);
        //    }
        //}
        //#endregion

        #region Danh sách mặt hàng
        [HttpPost("DM_MatHang_List")]
        public async Task<object> DM_MatHang_List([FromQuery] QueryParams query)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                query = query == null ? new QueryParams() : query;
                BaseModel<object> model = new BaseModel<object>();
                PageModel pageModel = new PageModel();
                ErrorModel error = new ErrorModel();
                SqlConditions conds = new SqlConditions();
                //conds.Add("DM_MatHang.isDel", 0);
                bool Visible = true;
                Visible = !_authService.IsReadOnlyPermit("1", user.Username);
                string whereStr = "";
                if (query.filter != null)
                {
                    if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    {
                        whereStr += $" and (TenMatHang LIKE '%{query.filter["keyword"]}%' OR MaHang LIKE N'%{query.filter["keyword"]}%')";
                    }
                    //var keyword = query.filter["keyword"];
                    //if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    //{
                    //    whereStr += @" and (TenMatHang  like @kw )";
                    //    conds.Add("kw", "%" + query.filter["keyword"].Trim() + "%");
                    //}
                    if (!string.IsNullOrEmpty(query.filter["IdLMH"]))
                    {
                        whereStr += $@" and IdLMH IN ({query.filter["IdLMH"]})";
                    }
                    if (!string.IsNullOrEmpty(query.filter["IdDVT"]))
                    {
                        whereStr += $@" and IdDVT IN ({query.filter["IdDVT"]})";
                    }
                    if (!string.IsNullOrEmpty(query.filter["IdNhanHieu"]))
                    {
                        whereStr += $@" and IdNhanHieu IN ({query.filter["IdNhanHieu"]})";
                    }
                    if (!string.IsNullOrEmpty(query.filter["IdXuatXu"]))
                    {
                        whereStr += $@" and IdXuatXu IN ({query.filter["IdXuatXu"]})";
                    }

                    if (!string.IsNullOrEmpty(query.filter["TenMatHang"]))
                    {
                        whereStr += $@" and (TenMatHang  like N'%{query.filter["TenMatHang "].Trim()}%')";
                    }
                    if (!string.IsNullOrEmpty(query.filter["MaHang"]))
                    {
                        whereStr += $@" and (MaHang  like '%{query.filter["MaHang "].Trim()}%')";
                    }


                }
                var dmMatHanglist = await _service.DM_MatHang_List(whereStr);
                if (dmMatHanglist.Count() == 0)
                    return JsonResultCommon.ThatBai("Không có dữ liệu");
                if (dmMatHanglist is null)
                    return JsonResultCommon.KhongTonTai();
                pageModel.TotalCount = dmMatHanglist.Count();
                //pageModel.AllPage = (int)Math.Ceiling(dmMatHanglist.Count() / (decimal)query.Panigator.PageSize);
                //pageModel.Size = query.Panigator.PageSize;
                //pageModel.Page = query.Panigator.PageIndex;
                //dmMatHanglist = dmMatHanglist.AsEnumerable().Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize).Take(query.Panigator.PageSize);
                return JsonResultCommon.ThanhCong(dmMatHanglist, pageModel, Visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
        #endregion
        
        
        #region Thêm mới mặt hàng
        [HttpPost("DM_MatHang_Insert")]
        public async Task<object> DM_MatHang_Insert(DM_MatHangDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");
                var create = await _service.DM_MatHang_Insert(model,user.Id);
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
        #region Sửa mặt hàng
        [HttpPost("UpdateMatHang")]
        public async Task<object> UpdateMatHang(DM_MatHangDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdMH from DM_MatHang where IdMH = {model.IdMH}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    if (!isExist) return JsonResultCommon.KhongTonTai("mặt hàng");

                var update = await _service.UpdateMatHang(model,user.Id);
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
        #region Xóa mặt hàng
        [HttpPost("DeleteMH")]
        public async Task<object> DeleteMH(DM_MatHangDTO model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                string sqlCheckCode = $"select IdMH from DM_MatHang where IdMH = {model.IdMH}";
                bool isExist = GeneralService.IsExistDB(sqlCheckCode, _connectionString);
                if (!isExist)
                    return BadRequest(MessageReturnHelper.KhongTonTai("Loại mặt hàng"));

                var update = await _service.DeleteMH(model, user.Id);
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

        #region Tìm kiếm tên mặt hàng
        [HttpGet("SearchMatHang")]
        public async Task<object> SearchMatHang(string TenMatHang)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.SearchMatHang(TenMatHang);
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
        #region Tìm mã mặt hàng
        [HttpGet("GetMatHangID")]
        public async Task<object> GetMatHangID(int IdMH)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var create = await _service.GetMatHangID(IdMH);
                if (create.IdMH == 0)
                {
                    return JsonResultCommon.KhongTonTai("DM_MatHang");
                }

                return JsonResultCommon.ThanhCong(create);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }

        #endregion
        #region Xóa nhiều mặt hàng
        [HttpPost("DeleteMHs")]
        public async Task<object> DeleteMHs(decimal[] ids)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user is null) return JsonResultCommon.BatBuoc("Đăng nhập");

                var update = await _service.DeleteMHs(ids, user.Id);
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
                    if (System.IO.File.Exists(filePath))
                    {
                        return BadRequest("File with the same name already exists.");
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
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
        
    }
}
