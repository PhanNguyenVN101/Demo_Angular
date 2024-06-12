using DpsLibs.Data;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.DM_XuatXu;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using System;
using JeeBeginner.Classes;
using System.Linq;

namespace JeeBeginner.Reponsitories.DM_XuatXu
{
    public class DM_XuatXuRepository:IDM_XuatXuRepository
    {
        private readonly string _connectionString;

        public DM_XuatXuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        #region Danh sách xuất xứ

        public async Task<IEnumerable<DM_XuatXuDTO>> DM_XuatXu_List(string whereStr)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            string sql = $@"select IdXuatXu,TenXuatXu from DM_XuatXu where 1=1 
                        {(string.IsNullOrEmpty(whereStr) ? "" : "AND " + whereStr)}
                       and DeletedDate is null";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_XuatXuDTO
                {
                    IdXuatXu = Int32.Parse(row["IdXuatXu"].ToString()),
                    TenXuatXu = row["TenXuatXu"].ToString(),
                });
                return await Task.FromResult(result);

            }

        }
        #endregion

        private Hashtable InitDataDM_XuatXu(DM_XuatXuDTO dmnh, long CreatedBy, bool isUpdate = false)
        {
            Hashtable val = new Hashtable();
            val.Add("TenXuatXu", dmnh.TenXuatXu);
            val.Add("isDel", 0);
            if (!isUpdate)
            {
                val.Add("CreatedDate", DateTime.UtcNow);
                val.Add("CreatedBy", CreatedBy);
            }
            return val;
        }

        #region Thêm mới xuất xứ
        public async Task<ReturnSqlModel> DM_XuatXu_Insert(DM_XuatXuDTO model, long CreatedBy)
        {
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    var val = InitDataDM_XuatXu(model, CreatedBy);
                    int x = cnn.Insert(val, "DM_XuatXu");
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

        #region Sửa xuất xứ
        public async Task<ReturnSqlModel> UpdateXuatXu(DM_XuatXuDTO model, long CreatedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdXuatXu", model.IdXuatXu);
                    val = InitDataDM_XuatXu(model, CreatedBy, true);
                    int x = cnn.Update(val, conds, "DM_XuatXu");
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

        #region Xóa xuất xứ
        public async Task<ReturnSqlModel> DeleteXuatXu(DM_XuatXuDTO model, long DeletedBy)
        {
            Hashtable val = new Hashtable();
            SqlConditions conds = new SqlConditions();
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                try
                {
                    conds.Add("IdXuatXu", model.IdXuatXu);
                    val.Add("isDel", 1);
                    val.Add("DeletedBy", DeletedBy);
                    val.Add("DeletedDate", DateTime.UtcNow);
                    int x = cnn.Update(val, conds, "DM_XuatXu");
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
        #region Xóa nhiều xuất xứ
        public async Task<ReturnSqlModel> DeleteXuatXus(decimal[] ids, long DeleteBy)
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
                        if (cnn.Update(_item, new SqlConditions { { "IdXuatXu", _Id } }, "DM_XuatXu") != 1)
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
        #region Tìm kiếm xuất xứ
        public async Task<IEnumerable<DM_XuatXuDTO>> SearchXuatXu(string TenXuatXu)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("TenXuatXu", TenXuatXu);
            string sql = @"select IdXuatXu,TenXuatXu from DM_XuatXu where TenXuatXu like N'%'+ @TenXuatXu +'%' and DeletedDate is null";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_XuatXuDTO
                {
                    IdXuatXu = Int32.Parse(row["IdXuatXu"].ToString()),
                    TenXuatXu = row["TenXuatXu"].ToString(),
                });
                return await Task.FromResult(result);
            }
        }
        #endregion


        #region Tìm mã xuất xứ
        public async Task<DM_XuatXuDTO> GetXuatXuID(int IdXuatXu)
        {
            DataTable dt = new DataTable();
            SqlConditions Conds = new SqlConditions();
            Conds.Add("IdXuatXu", IdXuatXu);
            string sql = @"select IdXuatXu,TenXuatXu from DM_XuatXu where IdXuatXu = @IdXuatXu";
            using (DpsConnection cnn = new DpsConnection(_connectionString))
            {
                dt = await cnn.CreateDataTableAsync(sql, Conds);
                var result = dt.AsEnumerable().Select(row => new DM_XuatXuDTO
                {
                    IdXuatXu = Int32.Parse(row["IdXuatXu"].ToString()),
                    TenXuatXu = row["TenXuatXu"].ToString(),
                }).SingleOrDefault();
                return await Task.FromResult(result);
            }
        }
        #endregion
    }
}
