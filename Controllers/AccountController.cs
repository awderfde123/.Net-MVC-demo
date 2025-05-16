using demo.Models;
using demo.Services;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly JwtService _jwt;
    private readonly AccountService _service;
    public AccountController(JwtService jwt, AccountService service)
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

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            TempData["Message"] = "帳號與密碼不能為空";
            return View();
        }

        if (password != confirmPassword)
        {
            TempData["Message"] = "密碼與確認密碼不一致";
            return View();
        }

        var user = _service.GetLoginByUserName(username);
        if (user != null)
        {
            TempData["Message"] = "帳號已存在";
            return View();
        }
        _service.AddAccount(new Account { UserName=username, Password = password, Role = Role.User });
        TempData["Message"] = "註冊成功，請登入";
        return RedirectToAction("Index");
    }
}
