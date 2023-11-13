using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Web_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            // xác thực tài khoản
            if (credential.UserName == "admin" && credential.Password == "admin")
            {
                // tạo security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "any"),
                    new Claim("Manager", "any"),
                    new Claim("EmploymentDate", "2023-07-29")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt,
                });
            }

            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");
            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey") ?? "");

            //Generate the JWT
            var jwt = new JwtSecurityToken(
                // JWT bao gồm ba phần:
                // Header: Chứa các thông tin về loại token và thuật toán mã hóa được sử dụng.
                // Payload (Nội dung): Chứa các thông tin mà bạn muốn truyền.
                // Signature (Chữ ký): Được tạo bằng cách sử dụng header, payload, một chuỗi bí mật và thuật toán đã được chỉ định trong header

                //issuer: "your_issuer",        // Issuer (người phát hành)
                //audience: "your_audience",    // Audience (đối tượng nhận)
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireAt,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public class Credential
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
