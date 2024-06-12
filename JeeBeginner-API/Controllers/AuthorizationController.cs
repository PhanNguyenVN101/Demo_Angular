using JeeAccount.Classes;
using JeeBeginner.Classes;
using JeeBeginner.Models.Common;
using JeeBeginner.Models.User;
using JeeBeginner.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JeeBeginner.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly ICustomAuthorizationService _service;
        private readonly string _jwtSecret;

        public AuthorizationController(ICustomAuthorizationService service, IConfiguration configuration)
        {
            _service = service;
            _jwtSecret = configuration.GetValue<string>("JWT:Secret");
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel model)
        {
            try
            {
                var user = await _service.GetUser(model.Username, model.Password);

                if (user == null)
                    return Unauthorized(new { message = "Username or password invalid" });

                if (user.IsLock)
                    return Unauthorized(new { message = "Username is locked" });

                if (user.Id == -1)
                    return Unauthorized(new { message = "Partner is locked" });

                var token = _service.CreateToken(user);

                return Ok(new
                {
                    user = user,
                    token = token.Result
                });
            }
            catch (KhongCoDuLieuException ex)
            {
                return Unauthorized(MessageReturnHelper.Custom(ex.Message + " không hợp lệ"));
            }
            catch (Exception ex)
            {
                return BadRequest(MessageReturnHelper.Exception(ex));
            }
        }

        [HttpGet]
        [Route("updateLastlogin")]
        public async Task<ActionResult<dynamic>> UpdateLastLogin()
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user == null) return StatusCode(401);

                var data = await _service.UpdateLastLogin(user.Id);
                if (!data.Susscess)
                {
                    return StatusCode(400);
                }

                return StatusCode(200);
            }
            catch (KhongCoDuLieuException ex)
            {
                return Unauthorized(MessageReturnHelper.Custom(ex.Message + " không hợp lệ"));
            }
            catch (Exception ex)
            {
                return BadRequest(MessageReturnHelper.Exception(ex));
            }
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<ActionResult<dynamic>> changePassword([FromBody] ChangePasswordModel model)
        {
            try
            {
                var user = Ulities.GetUserByHeader(HttpContext.Request.Headers, _jwtSecret);
                if (user == null) return StatusCode(401);
                if (string.IsNullOrEmpty(model.Username))
                {
                    model.Username = user.Username;
                }
                var getUSer = await _service.GetUser(model.Username, model.PasswordOld);
                if (getUSer == null) return (400, "Tài khoản hoặc mật khẩu cũ không hợp lệ");
                _service.ChangePassword(model);
                return StatusCode(200, new { message = "Thành công" });
            }
            catch (KhongCoDuLieuException ex)
            {
                return Unauthorized(MessageReturnHelper.Custom(ex.Message + " không hợp lệ"));
            }
            catch (Exception ex)
            {
                return BadRequest(MessageReturnHelper.Exception(ex));
            }
        }

        [HttpGet]
        [Route("api/authorization/getrules")]
        public async Task<IActionResult> GetRules(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is required.");

            try
            {
                var userRules = _service.GetRules(username);

                if (userRules == null)
                    return NotFound();

                return Ok(userRules);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("renewToken")]
        public IActionResult RenewToken()
        {
            // Lấy thông tin token từ yêu cầu
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
            {
                return BadRequest("Token is missing");
            }

            // Kiểm tra xác thực token và lấy thông tin người dùng
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            try
            {
                var tokenValidationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Hủy bỏ độ trễ mặc định
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                // Kiểm tra xem token có hết hạn không
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    // Tạo một token mới
                    var newToken = GenerateToken(principal.Claims);

                    // Trả về token mới
                    return Ok(new { token = newToken });
                }
                else
                {
                    // Token vẫn còn hiệu lực, không cần làm mới
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return BadRequest("Invalid token");
            }
        }

        // Phương thức này để tạo token mới
        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15), // Thời gian hết hạn của token mới
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}