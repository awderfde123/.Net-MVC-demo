using demo.Models;
using demo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace demo.Middlewares
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtService _jwtService;

        public JwtCookieMiddleware(RequestDelegate next, JwtService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context, JwtService jwt)
        {
            context.Request.Cookies.TryGetValue("access_token", out var accessToken);
            context.Request.Cookies.TryGetValue("refresh_token", out var refreshToken);

            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                // access_token 有效
                if (!string.IsNullOrEmpty(accessToken) && jwt.ValidateToken(accessToken) != null)
                {
                    context.Request.Headers.Append("Authorization", $"Bearer {accessToken}");
                }
                // access token 無效，refresh token 有效 續發新的 access_token
                else if (!string.IsNullOrEmpty(refreshToken) && jwt.ValidateToken(refreshToken) is ClaimsPrincipal refreshPrincipal)
                {
                    var userId = refreshPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var roleClaim = refreshPrincipal.FindFirst(ClaimTypes.Role)?.Value;

                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(roleClaim))
                    {
                        if (Enum.TryParse<Role>(roleClaim, out var role))
                        {
                            var newAccessToken = jwt.GenerateToken(userId, role, TimeSpan.FromMinutes(15));

                            context.Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                            });

                            context.Request.Headers.Append("Authorization", $"Bearer {newAccessToken}");
                        }
                    }
                }

            }

            await _next(context);
        }

    }
}
