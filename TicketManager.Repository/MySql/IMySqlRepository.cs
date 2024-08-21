using Dapper;

namespace TicketManager.Repository.MySql
{
    public interface IMySqlRepository
    {
        /// <summary>
        /// 获取单条信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dyparams"></param>
        /// <returns>查询结果</returns>
        public Task<T> GetInfo<T>(string sql, DynamicParameters? dyparams = null) where T : class;

        /// <summary>
        /// 插入单条信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <returns>查询结果</returns>
        public Task<int> AddInfo<T>(string sql, T obj) where T : class;

        /// <summary>
        /// 更新单条信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dyparams"></param>
        /// <returns>查询结果</returns>
        public Task<bool> UpdateInfo<T>(string sql, T obj) where T : class;

        /// <summary>
        /// 删除单条信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dyparams"></param>
        /// <returns>查询结果</returns>
        public Task<bool> DeleteInfo(string sql, DynamicParameters? dyparams = null);

        /// <summary>
        /// 获取信息列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="dyparams"></param>
        /// <returns>查询结果</returns>
        public Task<IEnumerable<T>> GetAllList<T>(string sql, DynamicParameters? dyparams = null) where T : class;
    }
}
