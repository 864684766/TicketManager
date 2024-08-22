using log4net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketManager.Model.User;
using TicketManagerServer.Controllers;

namespace TicketManagerServer.Unit
{

    public class JwtHelper
    {
        private readonly ILog _logger;
        private readonly IConfiguration _configuration;
        public JwtHelper(ILog logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string CreateToken(UserInfo user) 
        {
            var claims = new[]
            {
               new Claim(ClaimTypes.Name,user.Name!),
               new Claim(ClaimTypes.MobilePhone,user.Phone!)
            };

            // 2. 从 appsettings.json 中读取IssuerSigningKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTInfo:IssuerSigningKey"]!));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);
            var TokenTimespan = double.Parse(_configuration.GetSection("TokenTimespan").Value!);
            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["JWTInfo:ValidIssuer"],     //Issuer
                _configuration["JWTInfo:ValidAudience"],   //Audience
                claims,                          //Claims,
                DateTime.Now,                    //notBefore
                DateTime.Now.AddHours(TokenTimespan),    //expires
                signingCredentials               //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
