using log4net;
using MongoDB.Driver;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Tourism.Repository.MongoDb
{
    public class MongoRepository : IMongoRepository
    {
        private readonly MongoClient _client; // MongoClient 作为单例
        private readonly ILog _log;
        private readonly string _dbName;

        public MongoRepository(string connectionString, string dbName)
        {
            _client = new MongoClient(connectionString); // 创建 MongoClient 实例
            _dbName = dbName;
            _log = LogManager.GetLogger(typeof(MongoRepository)); // 直接初始化 _log
        }

        /// <summary>
        /// 获取指定集合
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="document">集合名称</param>
        /// <returns></returns>
        public IMongoDatabase GetMongoDatabase()
        {
            return _client.GetDatabase(_dbName); // 直接使用 _dbName
        }

        /// <summary>
        /// 新增一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info">数据源</param>
        /// <returns></returns>
        public async Task<int> AddAsync<T>(T info, string document) where T : class
        {
            try
            {
                var database = GetMongoDatabase();
                var collection = database.GetCollection<T>(document);
                await collection.InsertOneAsync(info);
                return 1;
            }
            catch (Exception ex)
            {
                _log.Error("AddAsync method error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">数据列表</param>
        /// <returns></returns>
        public async Task<int> BatchAddAsync<T>(List<T> list, string document) where T : class
        {
            try
            {
                var database = GetMongoDatabase();
                var collection = database.GetCollection<T>(document);
                await collection.InsertManyAsync(list);
                return 1;
            }
            catch (Exception ex)
            {
                _log.Error("BatchAddAsync method error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="express">删除条件</param>
        /// <returns></returns>
        public async Task<int> DelOneAsync<T>(Expression<Func<T, bool>> express, string document) where T : class
        {
            try
            {
                var database = GetMongoDatabase();
                var collection = database.GetCollection<T>(document);
                await collection.DeleteOneAsync(express);

                return 1;
            }
            catch (Exception ex)
            {
                _log.Error("DelOneAsync method error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// 删除所有符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="express">删除条件</param>
        /// <returns></returns>
        public async Task<int> DelManayAsync<T>(Expression<Func<T, bool>> express, string document) where T : class
        {
            try
            {
                var database = GetMongoDatabase();
                var collection = database.GetCollection<T>(document);
                await collection.DeleteManyAsync(express);
                return 1;
            }
            catch (Exception ex)
            {
                _log.Error("DelManayAsync method error:" + ex);
                throw;
            }
        }

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
        public async Task<(IFindFluent<T, T> linq, long totalCount)> PageListByQuery<T>(Dictionary<string, string> sortQuery, Expression<Func<T, bool>> query, long totalCount, string document, int pageIndex = 0, int pageSize = 0, bool showCount = false) where T : class
        {
            try
            {
                totalCount = 0;
                if (sortQuery.Count < 0)
                {
                    throw new Exception("sortQuery must fill in");
                }
                var sort = Builders<T>.Sort;
                SortDefinition<T>? sortimp = null;
                foreach (var item in sortQuery)
                {
                    if (item.Value.ToLower() == "desc")
                    {
                        sortimp = sort.Descending(item.Key);
                    }
                    else
                    {
                        sortimp = sort.Ascending(item.Key);
                    }
                }
                var database = GetMongoDatabase();
                var collection = database.GetCollection<T>(document);
                var result = collection.Find<T>(query).Sort(sortimp).Skip((pageIndex - 1) * pageSize).Limit(pageSize);

                if (showCount)
                {
                    totalCount = await result.CountDocumentsAsync();
                }

                return (result, totalCount);
            }
            catch (Exception ex)
            {
                _log.Error("PageListByQuery method error:" + ex);
                throw;
            }
        }


        /// <summary>
        /// 查询（不分页）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public async Task<IAsyncCursor<T>> QueryListAsync<T>(Expression<Func<T, bool>> query, string document) where T : class
        {
            try
            {
                var database = GetMongoDatabase();
                var collection = database.GetCollection<T>(document);
                return await collection.FindAsync<T>(query);
            }
            catch (Exception ex)
            {
                _log.Error("QueryListAsync method error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// 更新所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateInfo">更新查询条件</param>
        /// <param name="filter">更新条件</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<UpdateResult> UpdateManyAsync<T>(T updateInfo, Expression<Func<T, bool>> filter, string document, UpdateOptions? options = null) where T : class
        {
            try
            {
                List<UpdateDefinition<T>> updateList = BuildUpdateDefinition<T>(updateInfo, null);
                var database = GetMongoDatabase();
                var collection = database.GetCollection<T>(document);
                return await collection.UpdateManyAsync(filter, Builders<T>.Update.Combine(updateList), options);
            }
            catch (Exception ex)
            {
                _log.Error("UpdateManyAsync method error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateInfo">更新查询条件</param>
        /// <param name="filter">更新条件</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<UpdateResult> UpdateOneAsync<T>(T updateInfo, Expression<Func<T, bool>> filter, string document, UpdateOptions? options = null) where T : class
        {
            try
            {
                List<UpdateDefinition<T>> updateList = BuildUpdateDefinition<T>(updateInfo, null);
                var database = GetMongoDatabase();
                var collection = database.GetCollection<T>(document);
                return await collection.UpdateOneAsync(filter, Builders<T>.Update.Combine(updateList), options);
            }
            catch (Exception ex)
            {
                _log.Error("UpdateOneAsync method error:" + ex);
                throw;
            }
        }

        /// <summary>
        /// 建立更新条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private List<UpdateDefinition<T>> BuildUpdateDefinition<T>(object doc, string? parent)
        {
#pragma warning disable CS8602
            var updateList = new List<UpdateDefinition<T>>();
            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var key = parent == null ? property.Name : string.Format("{0}.{1}", parent, property.Name);
                //非空的复杂类型
                if ((property.PropertyType.IsClass || property.PropertyType.IsInterface) && property.PropertyType != typeof(string) && property.GetValue(doc) != null)
                {
                    if (typeof(IList).IsAssignableFrom(property.PropertyType))
                    {
                        #region 集合类型
                        int i = 0;
                        var subObj = property.GetValue(doc);
                        foreach (var item in subObj as IList)
                        {
                            if (item.GetType().IsClass || item.GetType().IsInterface)
                            {
                                if (key.ToLower() != "id")
                                {
                                    updateList.AddRange(BuildUpdateDefinition<T>(doc, string.Format("{0}.{1}", key, i)));
                                }
                            }
                            else
                            {
                                if (key.ToLower() != "id")
                                {
                                    updateList.Add(Builders<T>.Update.Set(string.Format("{0}.{1}", key, i), item));
                                }
                            }
                            i++;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 实体类型
                        //复杂类型，导航属性，类对象和集合对象
                        var subObj = property.GetValue(doc);
                        foreach (var sub in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                        {
                            if (sub.GetValue(subObj).ToString() != string.Empty && key.ToLower() != "id")
                            {
                                updateList.Add(Builders<T>.Update.Set(string.Format("{0}.{1}", key, sub.Name), sub.GetValue(subObj)));
                            }

                        }
                        #endregion
                    }
                }
                else //简单类型
                {
                    //这里更新不能带ID字段，常见错误：
                    //After applying the update to the document {_id:}, the (immutable) field '_id' was found to have been altered to _id: ObjectId('000000000000000000000000')
                    if (property.GetValue(doc).ToString() != string.Empty && key.ToLower() != "id")
                    {
                        updateList.Add(Builders<T>.Update.Set(key, property.GetValue(doc)));
                    }
                }
            }

            return updateList;
        }

        #region 分页使用方法
        //    // 定义查询条件
        //    Expression<Func<YourEntityType, bool>> query = x => x.SomeProperty == "SomeValue";

        //    // 定义排序条件
        //    var sortQuery = new Dictionary<string, string>
        //{
        //    { "SomeField", "asc" }, // 或 "desc"
        //};

        //    // 调用分页方法
        //    var(result, totalCount) = await mongoRepository.PageListByQuery(
        //        sortQuery,
        //        query,
        //        0, // totalCount 初始值
        //    "yourCollectionName",
        //        pageIndex: 1, // 页码
        //        pageSize: 10, // 每页条数
        //        showCount: true // 是否显示总数
        //);
        //    // 处理结果
        //    var items = await result.ToListAsync(); // 获取查询结果
        //    Console.WriteLine($"Total Count: {totalCount}");
        #endregion
    }
}