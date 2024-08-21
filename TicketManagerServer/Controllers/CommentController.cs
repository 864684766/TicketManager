using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TicketManager.IServer;
using TicketManager.Model.Common;
using TicketManager.QueryModel.User;
using TicketManagerServer.Model;
using TicketManagerServer.Unit;

namespace TicketManagerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ILog _logger;
        private readonly ICommentServer _commonServer;

        public CommentController(ICommentServer commonServer) {
            _commonServer = commonServer;
            _logger = LogManager.GetLogger(typeof(CommentController));
        }

        [Authorize]
        [HttpPost("addComment")]
        public async Task<IActionResult> AddComment(UserComment userComm)
        {
            var _result = new ResultData();
            try
            {
                var results = await _commonServer.AddComment(userComm);

                _result.status = (int)HttpStatusCode.OK;
                _result.msg = "success";
                _result.resultData = results;
                return Ok(_result);
            }
            catch (Exception ex)
            {
                _logger.Error($"addComment error:{ex}");
                _result.status = (int)HttpStatusCode.InternalServerError;
                _result.msg = ex.Message;
                _result.resultData = "";
                return BadRequest(_result);
            }
        }
    }
}
