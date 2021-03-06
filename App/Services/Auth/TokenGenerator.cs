#region using

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Services.Db.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace App.Services.Auth
{
    public class TokenGenerator
    {
        private readonly AppSettings _appSettings;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="options"></param>
        public TokenGenerator(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        /// <summary>
        ///     Generate JWT.
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <returns></returns>
        public string GenerateToken(ApplicationUser applicationUser)
        {
            var claims = GenerateClaims(applicationUser);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_appSettings.Jwt.Expiry);
            var token = new JwtSecurityToken(
                _appSettings.Jwt.Issuer,
                _appSettings.Jwt.Audience,
                claims,
                expires: expires,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        ///     Generate claims.
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <returns></returns>
        private static IEnumerable<Claim> GenerateClaims(ApplicationUser applicationUser)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id)
            };
        }
    }
}