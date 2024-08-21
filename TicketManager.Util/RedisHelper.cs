using log4net;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Configuration;
using TicketManager.Emuns;

namespace TicketManager.Util
{
#pragma warning disable CS8603
#pragma warning disable CS8618
    public class RedisHelper
    {

        private static ILog _log;
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redisdb;

        static RedisHelper()
        {
            // 只在类加载时初始化一次
            LazySingleton<ILog>.Initialize(() => LogManager.GetLogger(typeof(RedisHelper)));
        }
        public RedisHelper(IConfiguration configuration)
        {
            _log = LazySingleton<ILog>.GetInstance();
            if (_redisdb == null) {
                var redis_addr = configuration.GetConnectionString(DbNameEnum.RedisDbAddress.ToString())?.ToString() ?? "";
                _redisdb = ConnectionMultiplexer.Connect(redis_addr);
            }
            _db = _redisdb.GetDatabase();
        }

        #region String Func
        public async Task<bool> ExistsAsync(RedisKey key)
        {
            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _log?.Error("ExistsAsync method error:" + ex);
                throw;
            }
        }

        public async Task<bool> StringSetAsync(string key, string val, TimeSpan expireTime)
        {
            try
            {
                var result = await _db.StringSetAsync(key, val, expireTime);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error("Set method error:" + ex);
                throw;
            }
        }

        public async Task<string> GetAsync(string key)
        {
            try
            {
                return await _db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("GetAsync method error:" + ex);
                throw;
            }
        }

        public async Task<bool> DelAsync(RedisKey key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("DelAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long> IncrAsync(string key, long value = 1)
        {
            try
            {
                return await _db.StringIncrementAsync(key, value);
            }
            catch (Exception ex)
            {
                _log.Error("IncrAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long> DecrAsync(string key, long value = 1)
        {
            try
            {
                return await _db.StringDecrementAsync(key, value);
            }
            catch (Exception ex)
            {
                _log.Error("DecrAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long> AppendAsync(string key, long value = 1)
        {
            try
            {
                return await _db.StringAppendAsync(key, value);
            }
            catch (Exception ex)
            {
                _log.Error("AppendAsync method error:" + ex);
                throw;
            }
        }

        public async Task<bool> MSetAsync(KeyValuePair<RedisKey, RedisValue>[] values)
        {
            try
            {
                return await _db.StringSetAsync(values);
            }
            catch (Exception ex)
            {
                _log.Error("MSetAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> MGetAsync(RedisKey[] values)
        {
            try
            {
                return await _db.StringGetAsync(values);
            }
            catch (Exception ex)
            {
                _log.Error("MGetAsync method error:" + ex);
                throw;
            }
        }
        #endregion

        #region Hash Func
        public async void HSetAsync(string key, RedisValue hashField, RedisValue value)
        {
            try
            {
                await _db.HashSetAsync(key, hashField, value);
            }
            catch (Exception ex)
            {
                _log.Error("HSetAsync method error:" + ex);
                throw;
            }
        }

        public async void HMSetAsync(string key, HashEntry[] hashFiles)
        {
            try
            {
                await _db.HashSetAsync(key, hashFiles);
            }
            catch (Exception ex)
            {
                _log.Error("HMSetAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue> HGetAsync(RedisKey key, RedisValue hashField)
        {
            try
            {
                return await _db.HashGetAsync(key, hashField);
            }
            catch (Exception ex)
            {
                _log.Error("HGetAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> HMGetAsync(RedisKey key, RedisValue[] hashFields)
        {
            try
            {
                return await _db.HashGetAsync(key, hashFields);
            }
            catch (Exception ex)
            {
                _log.Error("HMGetAsync method error:" + ex);
                throw;
            }
        }

        public async Task<HashEntry[]> HGetAllAsync(RedisKey key)
        {
            try
            {
                return await _db.HashGetAllAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("HGetAllAsync method error:" + ex);
                throw;
            }
        }

        public async Task<bool> HExistsAsync(RedisKey key, RedisValue hashField)
        {
            try
            {
                return await _db.HashExistsAsync(key, hashField);
            }
            catch (Exception ex)
            {
                _log.Error("HExists method error:" + ex);
                throw;
            }
        }

        public async Task<long> HIncrByAsync(RedisKey key, RedisValue hashField, long value)
        {
            try
            {
                return await _db.HashIncrementAsync(key, hashField, value);
            }
            catch (Exception ex)
            {
                _log.Error("HIncrByAsync method error:" + ex);
                throw;
            }
        }

        public async Task<bool> HDelAsync(RedisKey key, RedisValue hashField)
        {
            try
            {
                return await _db.HashDeleteAsync(key, hashField);
            }
            catch (Exception ex)
            {
                _log.Error("HDelAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> HKeysAsync(RedisKey key)
        {
            try
            {
                return await _db.HashKeysAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("HKeysAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> HValsAsync(RedisKey key)
        {
            try
            {
                return await _db.HashValuesAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("HValsAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long> HLenAsync(RedisKey key)
        {
            try
            {
                return await _db.HashLengthAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("HLenAsync method error:" + ex);
                throw;
            }
        }
        #endregion

        #region List Func
        public async Task<long> LPushAsync(RedisKey key, RedisValue[] values)
        {
            try
            {
                return await _db.ListLeftPushAsync(key, values);
            }
            catch (Exception ex)
            {
                _log.Error("LPushAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long> RPushAsync(RedisKey key, RedisValue[] values)
        {
            try
            {
                return await _db.ListLeftPushAsync(key, values);
            }
            catch (Exception ex)
            {
                _log.Error("RPushAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue> LPopAsync(RedisKey key)
        {
            try
            {
                return await _db.ListLeftPopAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("LPopAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue> RPopAsync(RedisKey key)
        {
            try
            {
                return await _db.ListRightPopAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("RPopAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long> LLenAsync(RedisKey key)
        {
            try
            {
                return await _db.ListLengthAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("LLenAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> LRangeAsync(RedisKey key, long start, long stop)
        {
            try
            {
                return await _db.ListRangeAsync(key, start, stop);
            }
            catch (Exception ex)
            {
                _log.Error("LRangeAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue> LIndexAsync(RedisKey key, long index)
        {
            try
            {
                return await _db.ListGetByIndexAsync(key, index);
            }
            catch (Exception ex)
            {
                _log.Error("LIndexAsync method error:" + ex);
                throw;
            }
        }

        public async void LSetAsync(RedisKey key, long index, RedisValue value)
        {
            try
            {
                await _db.ListSetByIndexAsync(key, index, value);
            }
            catch (Exception ex)
            {
                _log.Error("LSetAsync method error:" + ex);
                throw;
            }
        }

        public async void LTrimAsync(RedisKey key, long start, long stop)
        {
            try
            {
                await _db.ListTrimAsync(key, start, stop);
            }
            catch (Exception ex)
            {
                _log.Error("LTrimAsync method error:" + ex);
                throw;
            }
        }

        public async void LInsertAsync(RedisKey key, string position, RedisValue pivot, RedisValue value)
        {
            try
            {
                if (position.ToLower() == "after")
                {
                    await _db.ListInsertAfterAsync(key, pivot, value);
                }
                else if (position.ToLower() == "before")
                {
                    await _db.ListInsertBeforeAsync(key, pivot, value);
                }
            }
            catch (Exception ex)
            {
                _log.Error("LInsertAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue> RPopLPushAsync(RedisKey source, RedisKey destination)
        {
            try
            {
                return await _db.ListRightPopLeftPushAsync(source, destination);
            }
            catch (Exception ex)
            {
                _log.Error("RPopLPushAsync method error:" + ex);
                throw;
            }
        }
        #endregion

        #region Set Func
        public async Task<long> SAddAsync(RedisKey key, RedisValue[] values)
        {
            try
            {
                return await _db.SetAddAsync(key, values);
            }
            catch (Exception ex)
            {
                _log.Error("SAddAsync method error:" + ex);
                throw;
            }
        }

        public async Task<bool> SAddAsync(RedisKey key, RedisValue value)
        {
            try
            {
                return await _db.SetAddAsync(key, value);
            }
            catch (Exception ex)
            {
                _log.Error("SAddAsync method error:" + ex);
                throw;
            }
        }

        public async Task<bool> SRemAsync(RedisKey key, RedisValue value)
        {
            try
            {
                return await _db.SetRemoveAsync(key, value);
            }
            catch (Exception ex)
            {
                _log.Error("SRemAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long> SRemAsync(RedisKey key, RedisValue[] values)
        {
            try
            {
                return await _db.SetRemoveAsync(key, values);
            }
            catch (Exception ex)
            {
                _log.Error("SRemAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> SMembersAsync(RedisKey key)
        {
            try
            {
                return await _db.SetMembersAsync(key);
            }
            catch (Exception ex)
            {
                _log.Error("SMembersAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> SRandMemberAsync(RedisKey key, long count = 1)
        {
            try
            {
                return await _db.SetRandomMembersAsync(key, count);
            }
            catch (Exception ex)
            {
                _log.Error("SRandMemberAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> SPopAsync(RedisKey key, long count = 1)
        {
            try
            {
                return await _db.SetPopAsync(key, count);
            }
            catch (Exception ex)
            {
                _log.Error("SPopAsync method error:" + ex);
                throw;
            }
        }
        #endregion

        #region Sorted Set Func

        public async Task<bool> ZAddAsync(RedisKey key, RedisValue member, double score)
        {
            try
            {
                return await _db.SortedSetAddAsync(key, member, score);
            }
            catch (Exception ex)
            {
                _log.Error("ZAddAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long> ZAddAsync(RedisKey key, SortedSetEntry[] values)
        {
            try
            {
                return await _db.SortedSetAddAsync(key, values);
            }
            catch (Exception ex)
            {
                _log.Error("ZAddAsync method error:" + ex);
                throw;
            }
        }

        public async Task<double?> ZScoreAsync(RedisKey key, RedisValue member)
        {
            try
            {
                return await _db.SortedSetScoreAsync(key, member);
            }
            catch (Exception ex)
            {
                _log.Error("ZScoreAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> ZRangeAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            try
            {
                return await _db.SortedSetRangeByRankAsync(key, start, stop, order);
            }
            catch (Exception ex)
            {
                _log.Error("ZRangeAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> ZRangeAsync(RedisKey key, long start = 0, long stop = -1)
        {
            try
            {
                return await _db.SortedSetRangeByRankAsync(key, start, stop);
            }
            catch (Exception ex)
            {
                _log.Error("ZRangeAsync method error:" + ex);
                throw;
            }
        }

        public async Task<SortedSetEntry[]> ZRangeWithScoreAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            try
            {
                return await _db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order);
            }
            catch (Exception ex)
            {
                _log.Error("ZRangeWithScoreAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> ZRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
        {
            try
            {
                return await _db.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take);
            }
            catch (Exception ex)
            {
                _log.Error("ZRangeByScoreAsync method error:" + ex);
                throw;
            }
        }

        public async Task<double> ZIncrByAsync(RedisKey key, RedisValue value, double score)
        {
            try
            {
                return await _db.SortedSetIncrementAsync(key, value, score);
            }
            catch (Exception ex)
            {
                _log.Error("ZIncrByAsync method error:" + ex);
                throw;
            }
        }

        public async Task<double> ZCountAsync(RedisKey key, double min, double max)
        {
            try
            {
                return await _db.SortedSetLengthAsync(key, min, max);
            }
            catch (Exception ex)
            {
                _log.Error("ZCountAsync method error:" + ex);
                throw;
            }
        }

        public async Task<bool> ZRemAsync(RedisKey key, RedisValue member)
        {
            try
            {
                return await _db.SortedSetRemoveAsync(key, member);
            }
            catch (Exception ex)
            {
                _log.Error("ZRemAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> ZRemRangByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            try
            {
                return await _db.SortedSetRangeByRankAsync(key, start, stop, order);
            }
            catch (Exception ex)
            {
                _log.Error("ZRemRangByRankAsync method error:" + ex);
                throw;
            }
        }

        public async Task<RedisValue[]> ZRemRangByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
        {
            try
            {
                return await _db.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take);
            }
            catch (Exception ex)
            {
                _log.Error("ZRemRangByScoreAsync method error:" + ex);
                throw;
            }
        }

        public async Task<long?> ZRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending)
        {
            try
            {
                return await _db.SortedSetRankAsync(key, member, order);
            }
            catch (Exception ex)
            {
                _log.Error("ZRankAsync method error:" + ex);
                throw;
            }
        }

        public async Task<double> ZDecrAsync(RedisKey key, RedisValue member, double score)
        {
            try
            {
                return await _db.SortedSetDecrementAsync(key, member, score);
            }
            catch (Exception ex)
            {
                _log.Error("ZDecrAsync method error:" + ex);
                throw;
            }
        }
        #endregion
    }
}
