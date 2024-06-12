using DpsLibs.Data;
using JeeBeginner.Models.Common;
using System.Collections.Generic;
using System.Data;
using System;
using JeeBeginner.Models.DM_LoaiMatHang;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Microsoft.Extensions.Configuration;
using JeeBeginner.Models.AccountRoleManagement;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data.Common;
using JeeBeginner.Models.AccountManagement;
using System.Collections;
using JeeBeginner.Classes;
using System.Security.Cryptography;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace JeeBeginner.Reponsitories.DM_LoaiMatHang
{
    public class DM_LoaiMatHangRepository : IDM_LoaiMatHangRepository
    {
        private readonly string _connectionString;

        public DM_LoaiMatHangRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        #region Danh sách Loại mặt hàng
        
        public async Task<IEnumerable<DM_LoaiMatHangDTO>> DM_LoaiMatHang_List(string whereStr)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();

            string sql = $@"SELECT TenLMH, IdLMH, isdel, deletedate,
                           COALESCE((SELECT TenLMH FROM DM_LoaiMatHang WHERE IdLMH = LMH.IdLMHParent), ' ') AS 'LoaiMatHangCha',
                           Mota, DoUuTien
                           FROM DM_LoaiMatHang LMH
                           WHERE 1 = 1
                           AND isdel = 0
                           AND DeleteDate IS NULL
                           {(string.IsNullOrEmpty(whereStr) ? "" : "AND (" + whereStr + ")")}";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiMatHangDTO
                {
                    TenLMH = row["TenLMH"].ToString(),
                    LoaiMatHangCha = row["LoaiMatHangCha"].ToString(),
                    Mota = row["Mota"].ToString(),
                    DoUuTien = Int32.Parse(row["DoUuTien"].ToString()),
                    IdLMH = Int32.Parse(row["IdLMH"].ToString())
                });
                return await Task.FromResult(result);

            }

        }
        #endregion

        private Hashtable InitDataDM_LoaiMatHang(DM_LoaiMatHangDTO dmlmh, long CreatedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();
            val.Add("TenLMH", dmlmh.TenLMH);
            val.Add("IdLMHParent", dmlmh.IdLMHParent);
            val.Add("Mota", dmlmh.Mota);
            val.Add("DoUuTien", dmlmh.DoUuTien);
            val.Add("IdKho", dmlmh.IdKho);
            val.Add("HinhAnh",  dmlmh.HinhAnh);
            val.Add("isDel", 0);
            if (!isUpdate)
            {
                val.Add("CreatedDate", DateTime.UtcNow);
                val.Add("CreatedBy", CreatedBy);
            }
            return val;
        }

        #region Thêm mới Loại mặt hàng
        public async Task<ReturnSqlModel> DM_LoaiMatHang_Insert(DM_LoaiMatHangDTO model, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDM_LoaiMatHang(model, CreatedBy);
                    int x = cnn.Insert(val, "DM_LoaiMatHang");
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

        #region Sửa Loại mặt hàng
        public async Task<ReturnSqlModel> UpdateLoaiMatHang(DM_LoaiMatHangDTO model, long CreatedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdLMH", model.IdLMH);
                    val = InitDataDM_LoaiMatHang(model, CreatedBy, true);
                    int x = cnn.Update(val, conds, "DM_LoaiMatHang");
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

        #region Xóa Loại mặt hàng
        public async Task<ReturnSqlModel> DeleteLMH(DM_LoaiMatHangDTO model, long DeleteBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdLMH", model.IdLMH);
                    val.Add("isDel", 1);
                    val.Add("DeleteBy", DeleteBy);
                    val.Add("DeleteDate", DateTime.UtcNow);
                    int x = cnn.Update(val, conds, "DM_LoaiMatHang");
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

        #region Xoá nhiều loại mặt hàng
        public async Task<ReturnSqlModel> Deletes(decimal[] ids, long DeleteBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    foreach (long _Id in ids)
                    {
                        Hashtable _item = new Hashtable();
                        _item.Add("isDel", 1);
                        _item.Add("DeleteBy", DeleteBy);
                        _item.Add("DeleteDate", DateTime.Now);
                        cnn.BeginTransaction();
                        if (cnn.Update(_item, new SqlConditions { { "IdLMH", _Id } }, "DM_LoaiMatHang") != 1)
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
        
        #region Tìm kiếm loại mặt hàng
        public async Task<IEnumerable<DM_LoaiMatHangDTO>> FilterLMH(DM_LoaiMatHangDTO model)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string filterTenLMH = "";
            string filterIdLMHParent = "";
            string filterDoUuTien = "";
            string filterMoTa = "";
           
          
            if (model.isFilter[0])
            {

                Conds.Add("TenLMH", model.TenLMH);
                filterTenLMH = "TenLMH like N'%' + @TenLMH + '%'";
            }
            if (model.isFilter[1] && model.isFilter[0])
            {
                Conds.Add("IdLMHParent", model.IdLMHParent);
                filterIdLMHParent = "and LMH.IdLMHParent = @IdLMHParent";
            }
            if (model.isFilter[1])
            {
                Conds.Add("IdLMHParent", model.IdLMHParent);
                filterIdLMHParent = "LMH.IdLMHParent = @IdLMHParent";
            }
            if (model.isFilter[2] && (model.isFilter[0] || model.isFilter[1]))
            {
                Conds.Add("DoUuTien", model.DoUuTien);
                filterDoUuTien = "and DoUuTien = @DoUuTien";
            }
            if (model.isFilter[2])
            {
                Conds.Add("DoUuTien", model.DoUuTien);
                filterDoUuTien = "DoUuTien = @DoUuTien";
            }
            if (model.isFilter[3] && (model.isFilter[0] || model.isFilter[1] || model.isFilter[2]))
            {
                Conds.Add("Mota", model.Mota);
                filterMoTa = "and Mota like N'%' + @Mota + '%'";
            }
            if (model.isFilter[3])
            {
                Conds.Add("Mota", model.Mota);
                filterMoTa = "Mota like N'%' + @Mota + '%'";
            }
            string sql = $@"select TenLMH, 
                        COALESCE((select TenLMH from DM_LoaiMatHang where IdLMH = LMH.IdLMHParent),' ') 
                        as 'LoaiMatHangCha', Mota, DoUuTien from DM_LoaiMatHang LMH 
                        where {filterTenLMH} {filterIdLMHParent} {filterDoUuTien} {filterMoTa} and DeleteDate is null";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiMatHangDTO
                {
                    TenLMH = row["TenLMH"].ToString(),
                    LoaiMatHangCha = row["LoaiMatHangCha"].ToString(),
                    Mota = row["Mota"].ToString(),
                    DoUuTien = Int32.Parse(row["DoUuTien"].ToString())
                });
                return await Task.FromResult(result);
            }
        }
        #endregion
        #region Danh sách kho
        
        public async Task<IEnumerable<DM_LoaiMatHangDTO>> DM_Kho_List()
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string sql = $@"select IdK as IdKho, TenK from DM_Kho where DeleteDate is null";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiMatHangDTO
                {
                    IdKho = Int32.Parse(row["IdKho"].ToString()),
                    TenK = row["TenK"].ToString()
                });
                return await Task.FromResult(result);

            }

        }
        #endregion
        #region Danh sách loại mặt hàng cha

        public async Task<IEnumerable<DM_LoaiMatHangDTO>> LoaiMatHangCha_List()
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string sql = $@"select DISTINCT LMH.IdLMHParent,COALESCE((select TenLMH from DM_LoaiMatHang 
                            where IdLMH = LMH.IdLMHParent),' ') 
                            as 'LoaiMatHangCha' from DM_LoaiMatHang LMH
							where COALESCE((select TenLMH from DM_LoaiMatHang 
                            where IdLMH = LMH.IdLMHParent),' ') <> ' '";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiMatHangDTO
                {
                    IdLMHParent = Int32.Parse(row["IdLMHParent"].ToString()),
                    LoaiMatHangCha = row["LoaiMatHangCha"].ToString()
                });
                return await Task.FromResult(result);

            }

        }
        #endregion
        #region Tìm mã kho
        public async Task<DM_LoaiMatHangDTO> GetKhoID(int IdK)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("IdK", IdK);
            string sql = @"select IdK as IdKho, TenK from DM_Kho where IdK = @IdK";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiMatHangDTO
                {
                    IdKho = Int32.Parse(row["IdKho"].ToString()),
                    TenK = row["TenK"].ToString(),
                }).SingleOrDefault();
                return await Task.FromResult(result);
            }
        }
        #endregion
        #region Tìm kiếm mã loại mặt hàng cha
        public async Task<DM_LoaiMatHangDTO> GetLoaiMHChaID(int IdLMHParent)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("IdLMHParent", IdLMHParent);
            string sql = @"select LMH.IdLMHParent,COALESCE((select TenLMH from DM_LoaiMatHang 
                            where IdLMH = LMH.IdLMHParent),' ') 
                            as 'LoaiMatHangCha' from DM_LoaiMatHang LMH where IdLMHParent = @IdLMHParent";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiMatHangDTO
                {
                    IdLMHParent = Int32.Parse(row["IdLMHParent"].ToString()),
                    LoaiMatHangCha = row["LoaiMatHangCha"].ToString(),
                }).SingleOrDefault();
                return await Task.FromResult(result);
            }
        }
        #endregion
        #region Tìm kiếm mã loại mặt hàng
        public async Task<FileContentResult> GetImageById(int IdLMH)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                DataTable dt = new DataTable();
                SqlConditions Conds = new SqlConditions();
                Conds.Add("IdLMH", IdLMH);
                string sql = @"select HinhAnh from DM_LoaiMatHang where IdLMH = @IdLMH";
                dt = await cnn.CreateDataTableAsync(sql, Conds);

                if (dt.Rows.Count == 0)
                {
                    // Return a default image or handle as needed
                    return null;
                }

                var row = dt.Rows[0];
                string imagePath = row["HinhAnh"].ToString();
                imagePath = imagePath.Replace("../../../../", "").Replace("/", "\\");

                // Read the image file from the path
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
                if (!System.IO.File.Exists(fullPath))
                {
                    // Handle the case where the image file does not exist
                    return null;
                }

                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

                // Return the image as a FileContentResult
                return new FileContentResult(imageBytes, "image/jpeg");
            }
        }

        public async Task<DM_LoaiMatHangDTO> GetLoaiMHID(int IdLMH)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("IdLMH", IdLMH);
            string sql = @"select TenLMH,IdLMH,IdLMHParent,IdKho,DoUuTien,Mota,ISNULL(HinhAnh, '') as HinhAnh from DM_LoaiMatHang where IdLMH = @IdLMH";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_LoaiMatHangDTO
                {
                    TenLMH = row["TenLMH"].ToString(),
                    Mota = row["Mota"].ToString(),
                    
                    HinhAnh = row["HinhAnh"].ToString(),
                    IdLMHParent = int.Parse(row["IdLMHParent"].ToString()),
                    IdKho = Int32.Parse(row["IdKho"].ToString()),
                    DoUuTien = Int32.Parse(row["DoUuTien"].ToString()),
                    IdLMH = Int32.Parse(row["IdLMH"].ToString())
                }).SingleOrDefault();
                return await Task.FromResult(result);
            }
        }
        

        #endregion
        #region Export File Excel
        public async Task<FileContentResult> Export(string whereStr)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("select ROW_NUMBER() OVER (ORDER BY LMH.TenLMH) AS STT," +
                    "LMH.TenLMH as N'Tên loại mặt hàng',COALESCE((select TenLMH from DM_LoaiMatHang " +
                    "where IdLMH = LMH.IdLMHParent),' ') as N'Loại mặt hàng cha', Mota as N'Mô tả', " +
                    "DoUuTien as N'Độ ưu tiên' from DM_LoaiMatHang LMH where TenLMH LIKE N'%" + whereStr + "%'" + 
                    " and DeleteDate is null", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("Data");

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                worksheet.Cells[1, i + 1].Value = reader.GetName(i);
                                worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                            }
                            int row = 2;
                            while (await reader.ReadAsync())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (i == 0 ||  i== 4)
                                    {
                                        worksheet.Cells[row, i + 1].Value = reader[i];
                                        worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }
                                    else
                                    {
                                        worksheet.Cells[row, i + 1].Value = reader[i];
                                        worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }

                                }
                                if (row % 2 != 0)
                                {
                                    for (int i = 1; i <= reader.FieldCount; i++)
                                    {
                                        worksheet.Cells[row, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[row, i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                    }
                                }
                                row++;
                            }
                            worksheet.Cells.AutoFitColumns();

                            var border = worksheet.Cells[worksheet.Dimension.Address].Style.Border;
                            border.Top.Style = border.Left.Style = border.Right.Style = border.Bottom.Style = ExcelBorderStyle.Thin;
                            var stream = new MemoryStream(package.GetAsByteArray());

                            // Trả về file Excel
                            return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                            {
                                FileDownloadName = "Data.xlsx"
                            };

                        }
                    }
                }
            }
        }
        #endregion
        
        



    }
}
