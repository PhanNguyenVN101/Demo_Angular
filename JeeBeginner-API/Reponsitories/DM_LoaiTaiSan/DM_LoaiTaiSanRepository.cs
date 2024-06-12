using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_LoaiTaiSan;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using System;
using JeeBeginner.Classes;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;

namespace JeeBeginner.Reponsitories.DM_LoaiTaiSan
{
    public class DM_LoaiTaiSanRepository:IDM_LoaiTaiSanRepository
    {
        private readonly string _connectionString;

        public DM_LoaiTaiSanRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        #region Danh sách loại tài sản

        public async Task<IEnumerable<DM_LoaiTaiSanDTO>> DM_LoaiTaiSan_List(string whereStr)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string sql = $@"select TS_DM_LoaiTS.* from TS_DM_LoaiTS where 1=1 
                        {(string.IsNullOrEmpty(whereStr) ? "" : "AND " + whereStr)}";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiTaiSanDTO
                {
                    IdLoaiTS = Int32.Parse(row["IdLoaiTS"].ToString()),
                    MaLoai = row["MaLoai"].ToString(),
                    TenLoai = row["TenLoai"].ToString(),
                    TrangThai = Convert.ToBoolean(row["TrangThai"].ToString()),
                });
                return await Task.FromResult(result);

            }

        }
        #endregion

        private Hashtable InitDataDM_LoaiTaiSan(DM_LoaiTaiSanDTO dmLoaiTaiSan)
        {
            Hashtable val = new Hashtable();
            val.Add("MaLoai", dmLoaiTaiSan.MaLoai);
            val.Add("TenLoai", dmLoaiTaiSan.TenLoai);
            val.Add("TrangThai", dmLoaiTaiSan.TrangThai);

            return val;
        }

        #region Thêm mới loại tài sản
        public async Task<ReturnSqlModel> DM_LoaiTaiSan_Insert(DM_LoaiTaiSanDTO model)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDM_LoaiTaiSan(model);
                    int x = cnn.Insert(val, "TS_DM_LoaiTS");
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

        #region Sửa loại tài sản
        public async Task<ReturnSqlModel> UpdateLoaiTaiSan(DM_LoaiTaiSanDTO model)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdLoaiTS", model.IdLoaiTS);
                    val = InitDataDM_LoaiTaiSan(model);
                    int x = cnn.Update(val, conds, "TS_DM_LoaiTS");
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

        #region Xóa loại tài sản
        public async Task<ReturnSqlModel> DeleteLoaiTaiSan(DM_LoaiTaiSanDTO model)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdLoaiTS", model.IdLoaiTS);
                    int x = cnn.Delete(conds, "TS_DM_LoaiTS");
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
        
        #region Tìm kiếm tên loại tài sản
        public async Task<IEnumerable<DM_LoaiTaiSanDTO>> SearchLoaiTaiSan(string TenLoai)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("TenLoai", TenLoai);
            string sql = @" select TS_DM_LoaiTS.* from TS_DM_LoaiTS where TenLoai like N'%'+ @TenLoai +'%'";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiTaiSanDTO
                {
                    IdLoaiTS = Int32.Parse(row["IdLoaiTS"].ToString()),
                    MaLoai = row["MaLoai"].ToString(),
                    TenLoai = row["TenLoai"].ToString(),
                    TrangThai = Convert.ToBoolean(row["TrangThai"].ToString()),
                });
                return await Task.FromResult(result);
            }
        }
        #endregion


        #region Tìm mã loại tài sản
        public async Task<DM_LoaiTaiSanDTO> GetLoaiTaiSanID(int IdLoaiTS)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("IdLoaiTS", IdLoaiTS);
            string sql = @"select TS_DM_LoaiTS.* from TS_DM_LoaiTS where IdLoaiTS = @IdLoaiTS";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiTaiSanDTO
                {
                    IdLoaiTS = Int32.Parse(row["IdLoaiTS"].ToString()),
                    MaLoai = row["MaLoai"].ToString(),
                    TenLoai = row["TenLoai"].ToString(),
                    TrangThai = Convert.ToBoolean(row["TrangThai"].ToString()),
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
        private bool IsBool(string value)
        {
            bool result;
            return bool.TryParse(value, out result);
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
                        if (worksheet.Dimension.Columns != 4)
                        {
                            return false; // Số lượng cột không đúng, trả về false
                        }

                        int rowCount = worksheet.Dimension.Rows;
                        //for (int row = 2; row <= rowCount; row++)
                        //{
                        //    if (!IsNumeric(worksheet.Cells[row, 1].Value?.ToString()) || !IsBool(worksheet.Cells[row, 4].Value?.ToString()))
                        //    {
                        //        return false;
                                
                        //    }
                        //}
                        for (int row = 2; row <= rowCount; row++)
                        {
                            int isEmptyRow = 0;
                            // Kiểm tra hàng đó có dữ liệu không
                            for (int col = 1; col <= 6; col++)
                            {
                                if (!string.IsNullOrEmpty(worksheet.Cells[row, col].Value?.ToString()))
                                {
                                    isEmptyRow += 1;
                                    break;
                                }
                            }
                            if (isEmptyRow > 0)
                            {
                                //Kiểm tra cột thứ nhất và thứ tư có phải toàn số hay không
                                    if (!IsNumeric(worksheet.Cells[row, 1].Value?.ToString()))
                                {
                                    return false; // Nếu không phải toàn số, trả về false
                                }
                                if (!IsBool(worksheet.Cells[row, 4].Value?.ToString()))
                                {
                                    return false;
                                }
                                Hashtable val = new Hashtable();
                                val.Add("MaLoai", worksheet.Cells[row, 2].Value?.ToString() ?? "");
                                val.Add("TenLoai", worksheet.Cells[row, 3].Value?.ToString() ?? "");
                                val.Add("TrangThai", worksheet.Cells[row, 4].Value?.ToString() ?? "false");
                                // Tạo kết nối đến cơ sở dữ liệu
                                using (DpsConnection cnn = new DpsConnection(_connectionString))
                                {
                                    int x = cnn.Insert(val, "TS_DM_LoaiTS");
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
                var worksheet = package.Workbook.Worksheets.Add("Loai_Tai_San");
                string[] columnNames = new string[] { "STT", "Mã loại", "Tên loại", "Còn hiệu lực"};
                double[] columnWidths = new double[] { 10, 20, 30, 20};

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
