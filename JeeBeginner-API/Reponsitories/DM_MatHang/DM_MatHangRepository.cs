using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_MatHang;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using System;
using JeeBeginner.Classes;
using System.Linq;
using JeeBeginner.Models.DM_LoaiMatHang;
using JeeBeginner.Models.DM_XuatXu;
using JeeBeginner.Models.DM_NhanHieu;
using JeeBeginner.Models.DM_DVT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using System.Drawing;

namespace JeeBeginner.Reponsitories.DM_MatHang
{
    public class DM_MatHangRepository:IDM_MatHangRepository
    {
        private readonly string _connectionString;

        public DM_MatHangRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        #region Danh sách mặt hàng

        public async Task<IEnumerable<DM_MatHangDTO>> DM_MatHang_List(string whereStr)
        {

            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string sql = "";

            if (string.IsNullOrEmpty(whereStr))
            {
                sql = $@"SELECT  IdMH, MaHang, TenMatHang,
                                                IdLMH,
                                                IdDVT,
                                                Mota, GiaMua, GiaBan, VAT, Barcode, NgungKinhDoanh,
                                                IdDVTCap2, QuyDoiDVTCap2,
                                                IdDVTCap3, QuyDoiDVTCap3,
                                                TenOnSite, ChiTietMoTa,
                                                IdNhanHieu,
                                                IdXuatXu,
                                                MaPhu, ThongSo, TheoDoiTonKho, TheodoiLo,
                                                MaLuuKho, MaViTriKho,
                                                ISNULL(HinhAnh, '') as HinhAnh,SoKyTinhKhauHaoToiThieu,SoKyTinhKhauHaoToiDa
                                    FROM        DM_MatHang
                    where 1=1 and DeletedDate is null";
            }
            else
            {
                sql = $@"SELECT  IdMH, MaHang, TenMatHang,
                                                IdLMH,
                                                IdDVT,
                                                Mota, GiaMua, GiaBan, VAT, Barcode, NgungKinhDoanh,
                                                IdDVTCap2, QuyDoiDVTCap2,
                                                IdDVTCap3, QuyDoiDVTCap3,
                                                TenOnSite, ChiTietMoTa,
                                                IdNhanHieu,
                                                IdXuatXu,
                                                MaPhu, ThongSo, TheoDoiTonKho, TheodoiLo,
                                                MaLuuKho, MaViTriKho,
                                                ISNULL(HinhAnh, '') as HinhAnh,SoKyTinhKhauHaoToiThieu,SoKyTinhKhauHaoToiDa
                                    FROM        DM_MatHang
                    where 1=1  {whereStr} and DeletedDate is null";
            }
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = cnn.CreateDataTable(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_MatHangDTO
                {
                    IdMH = Int32.Parse(row["IdMH"].ToString()),
                    MaHang = row["MaHang"].ToString(),
                    TenMatHang = row["TenMatHang"].ToString(),
                    SoKyTinhKhauHaoToiThieu = Int32.Parse(row["SoKyTinhKhauHaoToiThieu"].ToString()),
                    SoKyTinhKhauHaoToiDa = Int32.Parse(row["SoKyTinhKhauHaoToiDa"].ToString()),
                    HinhAnh = row["HinhAnh"].ToString(),
                    //HinhAnh = row["HinhAnh"].ToString(),
                    //IsDel = Convert.ToBoolean((bool)row["isDel"]),

                });
                return await Task.FromResult(result);
            }


            //DataTable dt = new DataTable();
            //SqlConditions Conds = new SqlConditions();
            //string sql = $@"select IdMH,ISNULL(HinhAnh, '') as HinhAnh,MaHang,TenMatHang,
            //            SoKyTinhKhauHaoToiThieu,SoKyTinhKhauHaoToiDa from DM_MatHang where 1=1 
            //            {(string.IsNullOrEmpty(whereStr) ? "" : "AND " + whereStr)}
            //           and DeletedDate is null";
            //using (DpsConnection cnn = new DpsConnection(_connectionString))
            //{
            //    dt = await cnn.CreateDataTableAsync(sql, Conds);
            //    var result = dt.AsEnumerable().Select(row => new DM_MatHangDTO
            //    {
            //        IdMH = Int32.Parse(row["IdMH"].ToString()),
            //        HinhAnh = row["HinhAnh"].ToString(),
            //        MaHang = row["MaHang"].ToString(),
            //        TenMatHang = row["TenMatHang"].ToString(),
            //        SoKyTinhKhauHaoToiThieu = Int32.Parse(row["SoKyTinhKhauHaoToiThieu"].ToString()),
            //        SoKyTinhKhauHaoToiDa = Int32.Parse(row["SoKyTinhKhauHaoToiDa"].ToString()),
            //    });
            //    return await Task.FromResult(result);

            //}

        }
        #endregion

        private Hashtable InitDataDM_MatHang(DM_MatHangDTO dmMatHang, long CreatedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();
            val.Add("HinhAnh", dmMatHang.HinhAnh);
            val.Add("MaHang", dmMatHang.MaHang);
            val.Add("TenMatHang", dmMatHang.TenMatHang);
            val.Add("IdLMH", dmMatHang.IdLMH);
            val.Add("IdDVT", dmMatHang.IdDVT);
            val.Add("Mota", dmMatHang.Mota);
            val.Add("ChiTietMoTa", dmMatHang.ChiTietMoTa);
            val.Add("VAT", dmMatHang.VAT);
            val.Add("TenOnSite", dmMatHang.TenOnSite);
            val.Add("GiaMua", dmMatHang.GiaMua);
            val.Add("GiaBan", dmMatHang.GiaBan);
            val.Add("LowerLimit", dmMatHang.LowerLimit);
            val.Add("UpperLimit", dmMatHang.UpperLimit);
            val.Add("IdDVTCap2", dmMatHang.IdDVTCap2);
            val.Add("QuyDoiDVTCap2", dmMatHang.QuyDoiDVTCap2);
            val.Add("IdDVTCap3", dmMatHang.IdDVTCap3);
            val.Add("QuyDoiDVTCap3", dmMatHang.QuyDoiDVTCap3);
            val.Add("TheodoiLo", dmMatHang.TheodoiLo);
            val.Add("IsTaiSan", dmMatHang.IsTaiSan);
            val.Add("SoKyTinhKhauHaoToiThieu", dmMatHang.SoKyTinhKhauHaoToiThieu);
            val.Add("SoKyTinhKhauHaoToiDa", dmMatHang.SoKyTinhKhauHaoToiDa);
            val.Add("MaPhu", dmMatHang.MaPhu);
            val.Add("Barcode", dmMatHang.Barcode);
            val.Add("ThongSo", dmMatHang.ThongSo);
            val.Add("IdNhanHieu", dmMatHang.IdNhanHieu);
            val.Add("IdXuatXu", dmMatHang.IdXuatXu);
            val.Add("IsDel", 0);
            if (!isUpdate)
            {
                val.Add("CreatedDate", DateTime.UtcNow);
                val.Add("CreatedBy", CreatedBy);
            }
            return val;
        }

        #region Thêm mới mặt hàng
        public async Task<ReturnSqlModel> DM_MatHang_Insert(DM_MatHangDTO model, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDM_MatHang(model, CreatedBy);
                    int x = cnn.Insert(val, "DM_MatHang");
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

        #region Sửa mặt hàng
        public async Task<ReturnSqlModel> UpdateMatHang(DM_MatHangDTO model, long CreatedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdMH", model.IdMH);
                    val = InitDataDM_MatHang(model, CreatedBy,true);
                    int x = cnn.Update(val, conds, "DM_MatHang");
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

        #region Xóa mặt hàng
        public async Task<ReturnSqlModel> DeleteMH(DM_MatHangDTO model, long DeleteBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdMH", model.IdMH);
                    val.Add("IsDel", 1);
                    val.Add("DeletedBy", DeleteBy);
                    val.Add("DeletedDate", DateTime.UtcNow);
                    int x = cnn.Update(val, conds, "DM_MatHang");
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

        #region Tìm kiếm tên mặt hàng
        public async Task<IEnumerable<DM_MatHangDTO>> SearchMatHang(string TenMatHang)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("TenMatHang", TenMatHang);
            string sql = @"select IdMH,HinhAnh,MaHang,TenMatHang,
                        SoKyTinhKhauHaoToiThieu,SoKyTinhKhauHaoToiDa from DM_MatHang
                        where TenMatHang like N'%'+ @TenMatHang +'%'";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_MatHangDTO
                {
                    IdMH = Int32.Parse(row["IdMH"].ToString()),
                    HinhAnh = row["HinhAnh"].ToString(),
                    MaHang = row["MaHang"].ToString(),
                    TenMatHang = row["TenMatHang"].ToString(),
                    SoKyTinhKhauHaoToiThieu = Int32.Parse(row["SoKyTinhKhauHaoToiThieu"].ToString()),
                    SoKyTinhKhauHaoToiDa = Int32.Parse(row["SoKyTinhKhauHaoToiDa"].ToString()),
                });
                return await Task.FromResult(result);
            }
        }
        #endregion


        #region Tìm mã mặt hàng
        public async Task<DM_MatHangDTO> GetMatHangID(int IdMH)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("IdMH", IdMH);
            string sql = @"select IdMH,MaHang,TenMatHang,IdLMH,IdDVT,Mota,ChiTietMoTa,VAT,TenOnSite,GiaMua,GiaBan,
                         LowerLimit,UpperLimit,IdDVTCap2,QuyDoiDVTCap2,IdDVTCap3,QuyDoiDVTCap3,TheodoiLo,IsTaiSan,
		                 SoKyTinhKhauHaoToiThieu,SoKyTinhKhauHaoToiDa,MaPhu,Barcode,ThongSo,IdNhanHieu,IdXuatXu,ISNULL(HinhAnh, '') as HinhAnh 
		                 from DM_MatHang where IdMH = @IdMH";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_MatHangDTO
                {
                    IdMH = Int32.Parse(row["IdMH"].ToString()),
                    HinhAnh = row["HinhAnh"].ToString() != "" ? row["HinhAnh"].ToString() : "",
                    MaHang = row["MaHang"].ToString() != "" ? row["MaHang"].ToString() : "",
                    TenMatHang = row["TenMatHang"].ToString() != "" ? row["TenMatHang"].ToString() : "",
                    IdLMH = Int32.Parse(row["IdLMH"].ToString()),
                    IdDVT = Int32.Parse(row["IdDVT"].ToString()),
                    Mota = row["Mota"].ToString() != "" ? row["Mota"].ToString() : "",
                    ChiTietMoTa = row["ChiTietMoTa"].ToString() != "" ? row["ChiTietMoTa"].ToString() : "",
                    VAT = float.Parse(row["VAT"].ToString()),
                    TenOnSite = row["TenOnSite"].ToString() != "" ? row["TenOnSite"].ToString() : "",
                    GiaMua = float.Parse(row["GiaMua"].ToString()),
                    GiaBan = float.Parse(row["GiaBan"].ToString()),
                    LowerLimit = !string.IsNullOrEmpty(row["LowerLimit"].ToString()) ? int.Parse(row["LowerLimit"].ToString()) : 0,
                    UpperLimit = !string.IsNullOrEmpty(row["UpperLimit"].ToString()) ? int.Parse(row["UpperLimit"].ToString()) : 0,
                    IdDVTCap2 = Int32.Parse(row["IdDVTCap2"].ToString()),
                    QuyDoiDVTCap2 = float.Parse(row["QuyDoiDVTCap2"].ToString()),
                    IdDVTCap3 = Int32.Parse(row["IdDVTCap3"].ToString()),
                    QuyDoiDVTCap3 = float.Parse(row["QuyDoiDVTCap3"].ToString()),
                    TheodoiLo = bool.Parse(row["TheodoiLo"].ToString()),
                    IsTaiSan = bool.Parse(row["IsTaiSan"].ToString()),
                    SoKyTinhKhauHaoToiThieu = Int32.Parse(row["SoKyTinhKhauHaoToiThieu"].ToString()),
                    SoKyTinhKhauHaoToiDa = Int32.Parse(row["SoKyTinhKhauHaoToiDa"].ToString()),
                    MaPhu = row["MaPhu"].ToString() != "" ? row["MaPhu"].ToString() : "",
                    Barcode = row["Barcode"].ToString() != "" ? row["Barcode"].ToString() : "",
                    ThongSo = row["ThongSo"].ToString() != "" ? row["ThongSo"].ToString() : "",
                    IdNhanHieu = Int32.Parse(row["IdNhanHieu"].ToString()),
                    IdXuatXu = Int32.Parse(row["IdXuatXu"].ToString()),
                }).SingleOrDefault();
                return await Task.FromResult(result);
            }
        }
        #endregion
        
        #region Xóa nhiều mặt hàng
        public async Task<ReturnSqlModel> DeleteMHs(decimal[] ids, long DeleteBy)
        {
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    foreach (long _Id in ids)
                    {
                        Hashtable _item = new Hashtable();
                        _item.Add("IsDel", 1);
                        _item.Add("DeletedBy", DeleteBy);
                        _item.Add("DeletedDate", DateTime.Now);
                        cnn.BeginTransaction();
                        if (cnn.Update(_item, new SqlConditions { { "IdMH", _Id } }, "DM_MatHang") != 1)
                        {
                            cnn.RollbackTransaction();


                        }
                        
                    }
                    cnn.EndTransaction();
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
                        if (worksheet.Dimension.Columns != 8)
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
                            for (int col = 1; col <= 8; col++)
                            {
                                if (!string.IsNullOrEmpty(worksheet.Cells[row, col].Value?.ToString()))
                                {
                                    isEmptyRow += 1;
                                    break;
                                }
                            }
                            if (isEmptyRow > 0)
                            {

                                if (!IsNumeric(worksheet.Cells[row, 1].Value?.ToString()) || !IsNumeric(worksheet.Cells[row, 5].Value?.ToString())
                                    || !IsNumeric(worksheet.Cells[row, 6].Value?.ToString()) || !IsNumeric(worksheet.Cells[row, 7].Value?.ToString()))
                                {
                                    return false; // Nếu không phải toàn số, trả về false
                                }
                                Hashtable val = new Hashtable();
                                val.Add("MaHang", worksheet.Cells[row, 2].Value?.ToString() ?? "");
                                val.Add("TenMatHang", worksheet.Cells[row, 3].Value?.ToString() ?? "");
                                val.Add("TenOnSite", worksheet.Cells[row, 4].Value?.ToString() ?? "");
                                val.Add("GiaBan", double.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? ""));
                                val.Add("IdLMH", int.Parse(worksheet.Cells[row, 6].Value?.ToString() ?? ""));
                                val.Add("IdDVT", int.Parse(worksheet.Cells[row, 7].Value?.ToString() ?? ""));
                                val.Add("Mota", worksheet.Cells[row, 8].Value?.ToString() ?? "");
                                val.Add("SoKyTinhKhauHaoToiThieu", 0);
                                val.Add("SoKyTinhKhauHaoToiDa", 0);
                                val.Add("IdDVTCap2", 0);
                                val.Add("QuyDoiDVTCap2", 0);
                                val.Add("IdDVTCap3", 0);
                                val.Add("QuyDoiDVTCap3", 0);
                                val.Add("ThongSo", "");
                                // Tạo kết nối đến cơ sở dữ liệu
                                using (DpsConnection cnn = new DpsConnection(_connectionString))
                                {
                                    int x = cnn.Insert(val, "DM_MatHang");
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
                var worksheet = package.Workbook.Worksheets.Add("Mat_Hang");
                string[] columnNames = new string[] { "STT", "Mã hàng", "Tên mặt hàng", "Tên khác", "Đơn giá", "ID loại mặt hàng", "ID đơn vị tính", "Mô tả" };
                double[] columnWidths = new double[] { 10, 30, 30, 30, 30, 20, 20, 30 };

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
