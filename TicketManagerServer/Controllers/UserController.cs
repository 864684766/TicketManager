using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using TicketManager.IServer;
using TicketManager.Model.User;
using TicketManager.QueryModel.User;
using TicketManagerServer.Model;
using TicketManagerServer.Unit;

namespace TicketManagerServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILog _logger;
        private readonly IUserServer _userServer;

        private readonly JwtHelper _jwtHelper;

        public UserController(IUserServer userServer, JwtHelper jwtHelper)
        {
            _userServer = userServer;
            _logger = LogManager.GetLogger(typeof(UserController));
            _jwtHelper = jwtHelper;
        }

        [HttpPost("userLogin")]
        public async Task<IActionResult> userLogin(UserQuery user)
        {
            var _result = new ResultData();
            try
            {
                var results = await _userServer.userLogin(user);

                if (results == null)
                {
                    _result.status = (int)HttpStatusCode.OK;
                    _result.msg = "未找到用户信息";
                    return BadRequest(_result);
                }
                var token = _jwtHelper.CreateToken(results);
                _result.status = (int)HttpStatusCode.OK;
                _result.msg = "success";
                _result.resultData = token;
                return Ok(_result);
            }
            catch (Exception ex)
            {
                _logger.Error($"userLogin error:{ex}");
                _result.status = (int)HttpStatusCode.InternalServerError;
                _result.msg = ex.Message;
                _result.resultData = "";
                return BadRequest(_result);
            }
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(UserInfo user)
        {
            var _result = new ResultData();
            try
            {
                var results = await _userServer.RegisterUser(user);
                _result.status = (int)HttpStatusCode.OK;
                _result.msg = "success";
                _result.resultData = results;
                return Ok(_result);
            }
            catch (Exception ex)
            {
                _logger.Error($"userLogin error:{ex}");
                _result.status = (int)HttpStatusCode.InternalServerError;
                _result.msg = ex.Message;
                _result.resultData = "";
                return BadRequest(_result);
            }
        }

        [Authorize]
        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(UserInfo user)
        {
            var _result = new ResultData();
            try
            {
                var userJson = JsonConvert.SerializeObject(user);
                _logger.Info($"UpdateUserInfo params:{userJson}");
                var results = await _userServer.UpdateUserInfo(user);
                _result.status = (int)HttpStatusCode.OK;
                _result.msg = "success";
                _result.resultData = results;
                return Ok(_result);
            }
            catch (Exception ex)
            {
                _logger.Error($"userLogin error:{ex}");
                _result.status = (int)HttpStatusCode.InternalServerError;
                _result.msg = ex.Message;
                _result.resultData = "";
                return BadRequest(_result);
            }
        }

        [HttpPost("DelUserInfo")]
        public async Task<IActionResult> DelUserInfo(UserInfo user)
        {
            var _result = new ResultData();
            try
            {
                var results = await _userServer.DelUserInfo(user);
                _result.status = (int)HttpStatusCode.OK;
                _result.msg = "success";
                _result.resultData = results;
                return Ok(_result);
            }
            catch (Exception ex)
            {
                _logger.Error($"userLogin error:{ex}");
                _result.status = (int)HttpStatusCode.InternalServerError;
                _result.msg = ex.Message;
                _result.resultData = "";
                return BadRequest(_result);
            }
        }
    }
}
