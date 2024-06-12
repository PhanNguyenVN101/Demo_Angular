using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_DoiTacBaoHiem;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using System;
using JeeBeginner.Classes;
using System.Linq;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Drawing;

namespace JeeBeginner.Services.DM_DoiTacBaoHiem
{
    public class DM_DoiTacBaoHiemRepository:IDM_DoiTacBaoHiemRepository
    {
        private readonly string _connectionString;

        public DM_DoiTacBaoHiemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        #region Danh sách đối tác bảo hiểm

        public async Task<IEnumerable<DM_DoiTacBaoHiemDTO>> DM_DoiTacBaoHiem_List(string whereStr)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string sql = $@"select Id_DV,TenDonVi,DiaChi,SoDT,NguoiLienHe,GhiChu from DM_DoiTacBaoHiem where 1=1 
                        {(string.IsNullOrEmpty(whereStr) ? "" : "AND " + whereStr)}
                       and IsDisable = 0";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_DoiTacBaoHiemDTO
                {
                    Id_DV = Int32.Parse(row["Id_DV"].ToString()),
                    TenDonVi = row["TenDonVi"].ToString(),
                    DiaChi = row["DiaChi"].ToString(),
                    SoDT = row["SoDT"].ToString(),
                    NguoiLienHe = row["NguoiLienHe"].ToString(),
                    GhiChu = row["GhiChu"].ToString(),
                });
                return await Task.FromResult(result);

            }

        }
        #endregion

        private Hashtable InitDataDM_DoiTacBaoHiem(DM_DoiTacBaoHiemDTO dmnh, long CreatedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();
            val.Add("TenDonVi", dmnh.TenDonVi);
            val.Add("DiaChi", dmnh.DiaChi);
            val.Add("SoDT", dmnh.SoDT);
            val.Add("NguoiLienHe", dmnh.NguoiLienHe);
            val.Add("GhiChu", dmnh.GhiChu);
            val.Add("IsDisable", 0);
            if (!isUpdate)
            {
                val.Add("NgayTao", DateTime.UtcNow);
                val.Add("NguoiTao", CreatedBy);
            }
            else
            {
                val.Add("NgaySua", DateTime.UtcNow);
                val.Add("NguoiSua", CreatedBy);
            }
            return val;
        }

        #region Thêm mới đối tác bảo hiểm
        public async Task<ReturnSqlModel> DM_DoiTacBaoHiem_Insert(DM_DoiTacBaoHiemDTO model, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDM_DoiTacBaoHiem(model, CreatedBy);
                    int x = cnn.Insert(val, "DM_DoiTacBaoHiem");
                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_EXCEPTION));
                    }
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }
        #endregion

        #region Sửa đối tác bảo hiểm
        public async Task<ReturnSqlModel> UpdateDoiTacBaoHiem(DM_DoiTacBaoHiemDTO model, long CreatedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("Id_DV", model.Id_DV);
                    val = InitDataDM_DoiTacBaoHiem(model, CreatedBy, true);
                    int x = cnn.Update(val, conds, "DM_DoiTacBaoHiem");
                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }
                }
                catch (Exception ex)
                {
                    cnn.RollbackTransaction();
                    cnn.EndTransaction();
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }
        #endregion

        #region Xóa đối tác bảo hiểm
        public async Task<ReturnSqlModel> DeleteDoiTacBaoHiem(DM_DoiTacBaoHiemDTO model)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("Id_DV", model.Id_DV);
                    val.Add("IsDisable", 1);
                    int x = cnn.Update(val, conds, "DM_DoiTacBaoHiem");
                    if (x <= 0)
                    {
                        return await Task.FromResult(new ReturnSqlModel(cnn.LastError.ToString(), Constant.ERRORCODE_SQL));
                    }
                }
                catch (Exception ex)
                {
                    cnn.RollbackTransaction();
                    cnn.EndTransaction();
                    return await Task.FromResult(new ReturnSqlModel(ex.Message, Constant.ERRORCODE_EXCEPTION));
                }
            }
            return await Task.FromResult(new ReturnSqlModel());
        }
        #endregion

        #region Tìm kiếm đối tác bảo hiểm
        public async Task<IEnumerable<DM_DoiTacBaoHiemDTO>> SearchDoiTacBaoHiem(string TenDonVi)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("TenDonVi", TenDonVi);
            string sql = @"select Id_DV,TenDonVi,DiaChi,SoDT,NguoiLienHe,GhiChu from DM_DoiTacBaoHiem where TenDonVi like N'%'+ @TenDonVi +'%' and IsDisable = 0";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_DoiTacBaoHiemDTO
                {
                    Id_DV = Int32.Parse(row["Id_DV"].ToString()),
                    TenDonVi = row["TenDonVi"].ToString(),
                    DiaChi = row["DiaChi"].ToString(),
                    SoDT = row["SoDT"].ToString(),
                    NguoiLienHe = row["NguoiLienHe"].ToString(),
                    GhiChu = row["GhiChu"].ToString(),
                });
                return await Task.FromResult(result);
            }
        }
        #endregion


        #region Tìm mã đối tác bảo hiểm
        public async Task<DM_DoiTacBaoHiemDTO> GetDoiTacBaoHiemID(int Id_DV)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("Id_DV", Id_DV);
            string sql = @"select Id_DV,TenDonVi,DiaChi,SoDT,NguoiLienHe,GhiChu from DM_DoiTacBaoHiem where Id_DV = @Id_DV";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_DoiTacBaoHiemDTO
                {
                    Id_DV = Int32.Parse(row["Id_DV"].ToString()),
                    TenDonVi = row["TenDonVi"].ToString(),
                    DiaChi = row["DiaChi"].ToString(),
                    SoDT = row["SoDT"].ToString(),
                    NguoiLienHe = row["NguoiLienHe"].ToString(),
                    GhiChu = row["GhiChu"].ToString() != "" ? row["TenMatHang"].ToString() : "",
                }).SingleOrDefault();
                return await Task.FromResult(result);
            }
        }
        #endregion
        private bool IsNumeric(string value)
        {
            double number;
            return double.TryParse(value, out number);
        }
        public async Task<bool> ImportFromExcel(IFormFile file, long CreatedBy)
        {
            if (file == null || file.Length <= 0)
                return false;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        // Kiểm tra số lượng cột
                        if (worksheet.Dimension.Columns != 6)
                        {
                            return false; // Số lượng cột không đúng, trả về false
                        }

                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            int isEmptyRow = 0;
                            // Kiểm tra hàng đó có dữ liệu không
                            for (int col = 1; col <= 6; col++)
                            {
                                if (!string.IsNullOrEmpty(worksheet.Cells[row, col].Value?.ToString()))
                                {
                                    isEmptyRow +=1;
                                }
                            }
                            if (isEmptyRow > 0)
                            {
                                // Kiểm tra cột thứ nhất và thứ tư có phải toàn số hay không
                                if (!IsNumeric(worksheet.Cells[row, 1].Value?.ToString()) || !IsNumeric(worksheet.Cells[row, 4].Value?.ToString()))
                                {
                                    return false; // Nếu không phải toàn số, trả về false
                                }

                                Hashtable val = new Hashtable();
                                val.Add("NgayTao", DateTime.UtcNow);
                                val.Add("NguoiTao", CreatedBy);
                                val.Add("isDisable", 0);
                                val.Add("TenDonVi", worksheet.Cells[row, 2].Value?.ToString() ?? "");
                                val.Add("DiaChi", worksheet.Cells[row, 3].Value?.ToString() ?? "");
                                val.Add("SoDT", worksheet.Cells[row, 4].Value?.ToString() ?? "");
                                val.Add("NguoiLienHe", worksheet.Cells[row, 5].Value?.ToString() ?? "");
                                val.Add("GhiChu", worksheet.Cells[row, 6].Value?.ToString() ?? "");
                                // Tạo kết nối đến cơ sở dữ liệu
                                using (DpsConnection cnn = new DpsConnection(_connectionString))
                                {
                                    int x = cnn.Insert(val, "DM_DoiTacBaoHiem");
                                    if (x <= 0)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<FileContentResult> TaiFileMau()
        {
            using (var package = new ExcelPackage())
            {
                // Add a worksheet to the workbook
                var worksheet = package.Workbook.Worksheets.Add("Doi_Tac_Bao_Hiem");
                string[] columnNames = new string[] { "STT", "Tên đơn vị", "Địa chỉ", "Số điện thoại", "Người liên hệ", "Ghi chú" };
                double[] columnWidths = new double[] { 10, 20, 30, 20, 20, 20 };

                // Add column headers with formatted appearance
                for (int i = 0; i < columnNames.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = columnNames[i];
                    worksheet.Column(i + 1).Width = columnWidths[i];

                    // Set font style for column headers
                    worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                // Add 20 empty rows
                for (int row = 2; row <= 20; row++)
                {
                    for (int col = 1; col <= columnNames.Length; col++)
                    {
                        worksheet.Cells[row, col].Value = ""; // Empty cell
                    }

                    // Apply alternate row color
                    if (row % 2 == 0)
                    {
                        for (int col = 1; col <= columnNames.Length; col++)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        }
                    }
                }

                // Save the Excel package to a memory stream
                worksheet.Cells.AutoFitColumns();

                var border = worksheet.Cells[worksheet.Dimension.Address].Style.Border;
                border.Top.Style = border.Left.Style = border.Right.Style = border.Bottom.Style = ExcelBorderStyle.Thin;
                var stream = new MemoryStream(package.GetAsByteArray());

                // Return as FileContentResult
                return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "Data_Mau.xlsx"
                };
            }
        }

    }
}
