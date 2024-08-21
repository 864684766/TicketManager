using log4net;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketManager.Emuns;
using TicketManager.IServer;
using TicketManager.Model.Common;
using TicketManager.Repository.MySql;
using TicketManager.Util;
using Tourism.Repository.MongoDb;

namespace TicketManager.Server
{
    public class CommentServer: ICommentServer
    {
        private const string CommentDocument = "comment_list";
        private const string DbName = "comment";
        private readonly IMongoRepository _mongodb;
        private readonly ILog _log;

        public CommentServer(IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString(DbNameEnum.CommonDb.ToString())!;
            _mongodb = new MongoRepository(connStr, DbName);
            _log = LogManager.GetLogger(typeof(CommentServer));
        }

        public async Task<int> AddComment(UserComment userComment) {
            try
            {
                var result = await _mongodb.AddAsync(userComment, CommentDocument);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error("AddComment method error:" + ex);
                throw;
            }
        }
    }
}
