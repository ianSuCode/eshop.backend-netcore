using Eshop.Domain.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eshop.Api
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Claim> GenerateClaims(UserInfoDto userInfoDto)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfoDto.Email!),  // User.Identity.Name
                new Claim(JwtRegisteredClaimNames.Jti, userInfoDto.Id.ToString())  // JWT ID
            };
            userInfoDto.Roles!.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role.ToString()!)));
            return claims;
        }

        public string GenerateTokenByClaims(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"], // issuer
                _configuration["Jwt:Issuer"], // audience
                claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public UserInfoDto GetUserInfoFromToken(string jwtEncodedString)
        {
            var token = new JwtSecurityToken(jwtEncodedString);
            return new UserInfoDto
            {
                Id = int.Parse(token.Id),
                Email = token.Claims.FirstOrDefault(it => it.Type == JwtRegisteredClaimNames.Sub)!.Value,
                Roles = token.Claims.Where(it => it.Type == ClaimTypes.Role).Select(it => it.Value).ToList()
            };
        }
    }
}