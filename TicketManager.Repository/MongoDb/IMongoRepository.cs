using log4net;
using MongoDB.Driver;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tourism.Repository.MongoDb
{
    public interface IMongoRepository
    {
        /// <summary>
        /// 新增一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info">数据源</param>
        /// <returns></returns>
        public Task<int> AddAsync<T>(T info, string document) where T : class;

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">数据列表</param>
        /// <returns></returns>
        public Task<int> BatchAddAsync<T>(List<T> list, string document) where T : class;

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="express">删除条件</param>
        /// <returns></returns>
        public Task<int> DelOneAsync<T>(Expression<Func<T, bool>> express, string document) where T : class;

        /// <summary>
        /// 删除所有符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="express">删除条件</param>
        /// <returns></returns>
        public Task<int> DelManayAsync<T>(Expression<Func<T, bool>> express, string document) where T : class;

        /// <summary>
        /// 根据条件分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortQuery">排序条件</param>
        /// <param name="query">查询条件</param>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">页数据条数</param>
        /// <param name="showCount">是否显示总数</param>
        /// <returns></returns>
        public Task<(IFindFluent<T, T> linq, long totalCount)> PageListByQuery<T>(Dictionary<string, string> sortQuery, Expression<Func<T, bool>> query, long totalCount, string document, int pageIndex = 0, int pageSize = 0, bool showCount = false) where T : class;


        /// <summary>
        /// 查询（不分页）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public Task<IAsyncCursor<T>> QueryListAsync<T>(Expression<Func<T, bool>> query, string document) where T : class;

        /// <summary>
        /// 更新所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateInfo">更新查询条件</param>
        /// <param name="filter">更新条件</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<UpdateResult> UpdateManyAsync<T>(T updateInfo, Expression<Func<T, bool>> filter, string document, UpdateOptions? options = null) where T : class;

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateInfo">更新查询条件</param>
        /// <param name="filter">更新条件</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<UpdateResult> UpdateOneAsync<T>(T updateInfo, Expression<Func<T, bool>> filter, string document, UpdateOptions? options = null) where T : class;
    }
}
