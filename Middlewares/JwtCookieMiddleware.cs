using demo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;
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

        public async Task InvokeAsync(HttpContext context)
        {
            // 如果 Authorization header 不存在，嘗試從 cookie 裡讀取 token
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                if (context.Request.Cookies.TryGetValue("access_token", out var token))
                {
                    // 加上 Bearer 前綴給後續中介軟體用
                    context.Request.Headers.Add("Authorization", $"Bearer {token}");
                }
            }

            await _next(context);
        }
    }

}
