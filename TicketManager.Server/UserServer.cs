using Dapper;
using log4net;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using TicketManager.Emuns;
using TicketManager.IServer;
using TicketManager.Model.User;
using TicketManager.QueryModel.User;
using TicketManager.Repository.MySql;
using TicketManager.Util;

namespace TicketManager.Server
{

#pragma warning disable CS8604
    public class UserServer : IUserServer
    {
        private readonly IMySqlRepository _mysqldb;
        private readonly ILog _log;
        static UserServer()
        {
            LazySingleton<ILog>.Initialize(() => LogManager.GetLogger(typeof(UserServer)));
        }

        public UserServer(IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString(DbNameEnum.UserManager.ToString());
            _mysqldb = new MySqlRepository(connStr);
            _log = LazySingleton<ILog>.GetInstance();
        }

        public async Task<UserInfo> userLogin(UserQuery query)
        {
            try
            {
                string sql = $"SELECT * FROM user WHERE name = @Name AND userPwd = @userPwd";
                var parameters = new DynamicParameters();
                parameters.Add("@Name", query.name);
                parameters.Add("@userPwd", query.userPwd);
                var result = await _mysqldb.GetInfo<UserInfo>(sql, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error($"userLogin error:{ex}");
                throw;
            }
        }

        public async Task<int> RegisterUser(UserInfo info)
        {
            try
            {
                var properties = info.GetType().GetProperties();
                var keyList = new List<string>();
                var fileList = new List<string>();
                DynamicParameters parameters = new DynamicParameters();
                foreach (var property in properties)
                {
                    keyList.Add($"@{property.Name}");
                    var value = property.GetValue(info);

                    if (property.Name == "userPwd")
                    {
                        value = PwdHandler.MD5EncryptTo32(value!.ToString());
                    }
                    parameters.Add($"@{property.Name}", value);
                }
                fileList = keyList.Select(item => item.Replace("@", "")).ToList();
                string sql = $"INSERT INTO user ({string.Join(",", fileList)}) VALUES ({string.Join(",", keyList)})";
                var result = await _mysqldb.AddInfo(sql, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error($"RegisterUser error:{ex}");
                throw;
            }
        }


        public async Task<bool> UpdateUserInfo(UserInfo info)
        {
            try
            {
                List<string> updates;
                DynamicParameters parameters;
                buildUpdateParams(info, out updates, out parameters);
                if (updates.Count == 0)
                {
                    return false;
                }
                string sql = @$"UPDATE user 
                                    SET {string.Join(",", updates)}
                                    WHERE id=@Id";

                parameters.Add("@Id", info.Id);
                var result = await _mysqldb.UpdateInfo(sql, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error($"UpdateUserInfo error:{ex}");
                throw;
            }
        }

        private void buildUpdateParams(UserInfo info, out List<string> updates, out DynamicParameters parameters)
        {
            var userInfo = info.GetType().GetProperties();
            updates = new List<string>();
            parameters = new DynamicParameters();
            foreach (var property in userInfo)
            {
                if (property.Name.ToLower() == "id")
                {
                    continue;
                }
                var value = property.GetValue(info);
                if (!string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    updates.Add(@$"{property.Name}=@{property.Name}");
                    parameters.Add($@"{property.Name}", value);
                }
            }
        }

        private object checkParams<T>(T info) where T : class
        {
            try
            {
                var updates = new List<string>();
                var parameters = new DynamicParameters();
                var userInfo = info.GetType().GetProperties();
                var missingRequiredFields = new List<string>();

                foreach (var property in userInfo)
                {
                    var value = property.GetValue(info);
                    var isRequired = Attribute.IsDefined(property, typeof(RequiredAttribute));

                    // 只需检查必填字段
                    if (isRequired && (value == null || (property.PropertyType == typeof(string) && string.IsNullOrWhiteSpace(value.ToString()))))
                    {
                        missingRequiredFields.Add(property.Name); // 收集缺失的必填字段
                    }
                }

                if (missingRequiredFields.Count > 0)
                {
                    return new
                    {
                        status = "fail",
                        msg = $"Missing required fields: {string.Join(", ", missingRequiredFields)}"
                    };
                }
                return new
                {
                    status = "success",
                    msg = $"check pass"
                };
            }
            catch (Exception ex)
            {
                _log.Error($"checkParams error:{ex}");
                throw;
            }
        }

        public async Task<bool> DelUserInfo(UserInfo info)
        {
            try
            {
                string sql = @$"DELETE FROM user
                                WHERE id=@Id";
                var parameters = new DynamicParameters();
                parameters.Add("@Id", info.Id);
                var result = await _mysqldb.DeleteInfo(sql, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error($"DelUserInfo error:{ex}");
                throw;
            }
        }
    }
}
