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
using System.Collections;
using JeeBeginner.Classes;
using System.Security.Cryptography;
using JeeBeginner.Models.DM_NhanHieu;
using Microsoft.AspNetCore.Http;
using static JeeBeginner.Models.Common.Panigator;

namespace JeeBeginner.Reponsitories.DM_NhanHieu
{
    public class DM_NhanHieuRepository : IDM_NhanHieuRepository
    {
        private readonly string _connectionString;

        public DM_NhanHieuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        #region Danh sách nhãn hiệu
        
        public async Task<IEnumerable<DM_NhanHieuDTO>> DM_NhanHieu_List(string whereStr)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string sql = $@"select IdNhanHieu,TenNhanHieu from DM_NhanHieu where 1=1 
                        {(string.IsNullOrEmpty(whereStr) ? "" : "AND " + whereStr)}
                       and DeletedDate is null";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_NhanHieuDTO
                {
                    IdNhanHieu = Int32.Parse(row["IdNhanHieu"].ToString()),
                    TenNhanHieu = row["TenNhanHieu"].ToString(),
                });
                return await Task.FromResult(result);

            }

        }
        #endregion
        

        private Hashtable InitDataDM_NhanHieu(DM_NhanHieuDTO dmnh, long CreatedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();
            val.Add("TenNhanHieu", dmnh.TenNhanHieu);
            val.Add("isDel", 0);
            if (!isUpdate)
            {
                val.Add("CreatedDate", DateTime.UtcNow);
                val.Add("CreatedBy", CreatedBy);
            }
            return val;
        }

        #region Thêm mới nhãn hiệu
        public async Task<ReturnSqlModel> DM_NhanHieu_Insert(DM_NhanHieuDTO model, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDM_NhanHieu(model, CreatedBy);
                    int x = cnn.Insert(val, "DM_NhanHieu");
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

        #region Sửa nhãn hiệu
        public async Task<ReturnSqlModel> UpdateNhanHieu(DM_NhanHieuDTO model, long CreatedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdNhanHieu", model.IdNhanHieu);
                    val = InitDataDM_NhanHieu(model, CreatedBy, true);
                    int x = cnn.Update(val, conds, "DM_NhanHieu");
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

        #region Xóa nhãn hiệu
        public async Task<ReturnSqlModel> DeleteNhanHieu(DM_NhanHieuDTO model, long DeletedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdNhanHieu", model.IdNhanHieu);
                    val.Add("isDel", 1);
                    val.Add("DeletedBy", DeletedBy);
                    val.Add("DeletedDate", DateTime.UtcNow);
                    int x = cnn.Update(val, conds, "DM_NhanHieu");
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
        #region Xóa nhiều nhãn hiệu
        public async Task<ReturnSqlModel> DeleteNhanHieus(decimal[] ids, long DeleteBy)
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
                        if (cnn.Update(_item, new SqlConditions { { "IdNhanHieu", _Id } }, "DM_NhanHieu") != 1)
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
        #region Tìm kiếm nhãn hiệu
        public async Task<IEnumerable<DM_NhanHieuDTO>> SearchNhanHieu(string TenNhanHieu)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("TenNhanHieu", TenNhanHieu);
            string sql = @"select IdNhanHieu,TenNhanHieu from DM_NhanHieu where TenNhanHieu like N'%'+ @TenNhanHieu +'%' and DeletedDate is null";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_NhanHieuDTO
                {
                    IdNhanHieu = Int32.Parse(row["IdNhanHieu"].ToString()),
                    TenNhanHieu = row["TenNhanHieu"].ToString(),
                });
                return await Task.FromResult(result);
            }
        }
        #endregion
       
        
        #region Tìm mã nhãn hiệu
        public async Task<DM_NhanHieuDTO> GetNhanHieuID(int IdNhanHieu)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("IdNhanHieu", IdNhanHieu);
            string sql = @"select IdNhanHieu,TenNhanHieu from DM_NhanHieu where IdNhanHieu = @IdNhanHieu";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_NhanHieuDTO
                {
                    IdNhanHieu = Int32.Parse(row["IdNhanHieu"].ToString()),
                    TenNhanHieu = row["TenNhanHieu"].ToString(),
                }).SingleOrDefault();
                return await Task.FromResult(result);
            }
        }
        #endregion
        
    }
}
