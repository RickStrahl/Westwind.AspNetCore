using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Westwind.AspNetCore.Security
{
    public class JwtHelper
    {

        /// <summary>
        /// Returns a Jwt Token from basic input parameters
        /// </summary>
        /// <param name="username"></param>
        /// <param name="uniqueKey"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expiration"></param>
        /// <param name="additionalClaims"></param>
        /// <returns></returns>
        public static JwtSecurityToken GetJwtToken(
            string username,
            string uniqueKey,
            string issuer,
            string audience,
            TimeSpan expiration,
            Claim[] additionalClaims = null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (additionalClaims is object)
            {
                var claimList = new List<Claim>(claims);
                claimList.AddRange(additionalClaims);
                claims = claimList.ToArray();
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(uniqueKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.Add(expiration),
                claims: claims,
                signingCredentials: creds
            );
        }

        /// <summary>
        /// Returns a token string from base claims
        /// </summary>
        /// <param name="username"></param>
        /// <param name="uniqueKey"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="expiration"></param>
        /// <param name="additionalClaims"></param>
        /// <returns></returns>
        public static string GetJwtTokenString(
            string username,
            string uniqueKey,
            string issuer,
            string audience,
            TimeSpan expiration,
            Claim[] additionalClaims = null)
        {
            var token = GetJwtToken(username, uniqueKey, issuer, audience, expiration, additionalClaims);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Converts an existing Jwt Token to a string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetJwtTokenString(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Returns an issuer key
        /// </summary>
        /// <param name="issuerKey"></param>
        /// <returns></returns>
        public static SymmetricSecurityKey GetSymetricSecurityKey(string issuerKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerKey));
        }
    }
}
