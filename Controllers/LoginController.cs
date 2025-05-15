using demo.Models;
using demo.Services;
using Microsoft.AspNetCore.Mvc;

public class LoginController : Controller
{
    private readonly JwtService _jwt;
    private readonly LoginService _service;
    public LoginController(JwtService jwt, LoginService service)
    {
        _jwt = jwt;
        _service = service;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string username, string password)
    {
        var user = _service.GetLoginByUserName(username);
        if (user != null)
        {
            var token = _jwt.GenerateToken(username, TimeSpan.FromHours(1));
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict // Strict不能第三方登入
            });

            return RedirectToAction("Index", "Product");
        }

        ViewBag.Message = "帳號或密碼錯誤";
        return View();
    }
}
