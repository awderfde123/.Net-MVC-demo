using demo.Models;
using demo.Services;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly JwtService _jwt;
    private readonly LoginService _service;
    public AccountController(JwtService jwt, LoginService service)
    {
        _jwt = jwt;
        _service = service;
    }

    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        var user = _service.GetLoginByUserName(username);
        if (user != null && user?.Password == password)
        {
            var accessToken = _jwt.GenerateToken(username, user.Role, TimeSpan.FromMinutes(15));
            var refreshToken = _jwt.GenerateToken(username, user.Role, TimeSpan.FromHours(1));
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict // Strict不能第三方登入
            }); 
            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict // Strict不能第三方登入
            });

            return RedirectToAction("Index", "Product");
        }

        TempData["Message"] = "帳號或密碼錯誤";
        return RedirectToAction("Index", "Account");
    }

    [HttpGet]
    public ActionResult<IEnumerable<Account>> Logout()
    {
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return RedirectToAction("Index", "Account");
    }
}
