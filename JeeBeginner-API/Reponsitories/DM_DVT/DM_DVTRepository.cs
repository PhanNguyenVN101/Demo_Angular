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
using JeeBeginner.Models.DM_DVT;
using System.Collections;
using JeeBeginner.Classes;
using System.Security.Cryptography;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace JeeBeginner.Reponsitories.DM_DVT
{
    public class DM_DVTRepository : IDM_DVTRepository
    {
        private readonly string _connectionString;

        public DM_DVTRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        #region Danh sách đơn vị tính
        
        public async Task<IEnumerable<DM_DVTDTO>> DM_DVT_List(string whereStr)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string sql = $@"select IdDVT,TenDVT from DM_DVT where 1=1 
                        {(string.IsNullOrEmpty(whereStr) ? "" : "AND " + whereStr)}
                       and DeleteDate is null";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_DVTDTO
                {
                    IdDVT = Int32.Parse(row["IdDVT"].ToString()),
                    TenDVT = row["TenDVT"].ToString(),
                });
                return await Task.FromResult(result);

            }

        }
        #endregion

        private Hashtable InitDataDM_DVT(DM_DVTDTO dmdvt, long CreatedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();
            val.Add("TenDVT", dmdvt.TenDVT);
            val.Add("isDel", 0);
            if (!isUpdate)
            {
                val.Add("CreatedDate", DateTime.UtcNow);
                val.Add("CreatedBy", CreatedBy);
            }
            return val;
        }

        #region Thêm mới đơn vị tính
        public async Task<ReturnSqlModel> DM_DVT_Insert(DM_DVTDTO model, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDM_DVT(model, CreatedBy);
                    int x = cnn.Insert(val, "DM_DVT");
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

        #region Sửa đơn vị tính
        public async Task<ReturnSqlModel> UpdateDVT(DM_DVTDTO model, long CreatedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdDVT", model.IdDVT);
                    val = InitDataDM_DVT(model, CreatedBy, true);
                    int x = cnn.Update(val, conds, "DM_DVT");
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

        #region Xóa Đơn vị tính
        public async Task<ReturnSqlModel> DeleteDVT(DM_DVTDTO model, long DeleteBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdDVT", model.IdDVT);
                    val.Add("isDel", 1);
                    val.Add("DeleteBy", DeleteBy);
                    val.Add("DeleteDate", DateTime.UtcNow);
                    int x = cnn.Update(val, conds, "DM_DVT");
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
        #region Xóa nhiều Đơn vị tính
        public async Task<ReturnSqlModel> DeleteDVTs(decimal[] ids, long DeleteBy)
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
                        _item.Add("DeleteBy", DeleteBy);
                        _item.Add("DeleteDate", DateTime.Now);
                        cnn.BeginTransaction();
                        if (cnn.Update(_item, new SqlConditions { { "IdDVT", _Id } }, "DM_DVT") != 1)
                        {
                            cnn.RollbackTransaction();
                            //_baseModel.status = 0;
                            //_baseModel.error = new ErrorModel("Xóa thất bại!" + cnn.LastError, Constants.ERRORCODE_SQL);
                            //return _baseModel;

                           
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
        #region Tìm kiếm tên đơn vị tính
        public async Task<IEnumerable<DM_DVTDTO>> SearchDVT(string TenDVT)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("TenDVT", TenDVT);
            string sql = @"select IdDVT,TenDVT from DM_DVT where TenDVT like N'%'+ @TenDVT +'%' and DeleteDate is null";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_DVTDTO
                {
                    IdDVT = Int32.Parse(row["IdDVT"].ToString()),
                    TenDVT = row["TenDVT"].ToString(),
                });
                return await Task.FromResult(result);
            }
        }
        #endregion
       
        
        #region Tìm mã đơn vị tính
        public async Task<DM_DVTDTO> GetDVTID(int IdDVT)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("IdDVT", IdDVT);
            string sql = @"select IdDVT,TenDVT from DM_DVT where IdDVT = @IdDVT";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_DVTDTO
                {
                    IdDVT = Int32.Parse(row["IdDVT"].ToString()),
                    TenDVT = row["TenDVT"].ToString(),
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

                using (var command = new SqlCommand("select ROW_NUMBER() OVER (ORDER BY TenDVT) AS STT," +
                    " TenDVT as N'Tên' from DM_DVT where TenDVT LIKE N'%" + whereStr + "%'" + 
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
                                    if (i == 0)
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
