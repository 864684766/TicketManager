using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace TicketManager.Repository.MySql
{
    public class MySqlRepository : IMySqlRepository
    {
        private readonly string _connectionString;
        public MySqlRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 获取单条信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dyparams"></param>
        /// <returns>查询结果</returns>
        public async Task<T> GetInfo<T>(string sql, DynamicParameters? dyparams = null) where T : class
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
#pragma warning disable CS8603 // 可能返回 null 引用。
                var result = await db.QueryFirstOrDefaultAsync<T>(sql, dyparams);
                return result;
            }
        }

        /// <summary>
        /// 插入单条信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>查询结果</returns>
        public async Task<int> AddInfo<T>(string sql, T data) where T : class
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                var result = await db.ExecuteAsync(sql, data);
                return result;
            }
        }

        /// <summary>
        /// 更新单条信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dyparams"></param>
        /// <returns>查询结果</returns>
        public async Task<bool> UpdateInfo<T>(string sql, T obj) where T : class
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                var rowsAffected = await db.ExecuteAsync(sql, obj);
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// 删除单条信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dyparams"></param>
        /// <returns>查询结果</returns>
        public async Task<bool> DeleteInfo(string sql, DynamicParameters? dyparams = null)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                var rowsAffected = await db.ExecuteAsync(sql, dyparams);
                return rowsAffected > 0;
            }
        }


        /// <summary>
        /// 获取信息列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dyparams"></param>
        /// <returns>查询结果</returns>
        public async Task<IEnumerable<T>> GetAllList<T>(string sql, DynamicParameters? dyparams = null) where T : class
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                var list = await db.QueryAsync<T>(sql, dyparams);
                return list;
            }
        }
    }
}
