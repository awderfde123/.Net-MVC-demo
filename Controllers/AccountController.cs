using demo.Migrations;
using demo.Models;
using demo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Role = demo.Models.Role;

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
        var hasher = new PasswordHasher<Account>();

        if (user == null)
        {
            TempData["Message"] = "帳號或密碼錯誤";
            return RedirectToAction("Index");
        }

        var result = hasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Success)
        {
            var accessToken = _jwt.GenerateToken(username, user.Role, TimeSpan.FromMinutes(15));
            var refreshToken = _jwt.GenerateToken(username, user.Role, TimeSpan.FromHours(1));

            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction("Index", "Product");
        }

        TempData["Message"] = "帳號或密碼錯誤";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult GoogleLogin(string returnUrl = "/Product/Index")
    {
        var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", new { returnUrl }) };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<ActionResult> GoogleResponse(string returnUrl = "/Product/Index")
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!result.Succeeded)
        {
            TempData["Message"] = "Google 登入失敗";
            return RedirectToAction("Index");
        }

        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (email == null)
        {
            TempData["Message"] = "無法取得 Google 帳號資訊";
            return RedirectToAction("Index");
        }

        // 檢查是否已有帳號，若無則新增
        var user = _service.GetLoginByUserName(email);
        if (user == null)
        {
            user = new Account
            {
                UserName = email,
                Password = "", // 第三方登入無密碼
                Role = Role.User
            };
            _service.AddAccount(user);
        }

        //Cookie
        var appClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

        var claimsIdentity = new ClaimsIdentity(appClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties { IsPersistent = true });

        //JWT
        var accessToken = _jwt.GenerateToken(user.UserName, user.Role, TimeSpan.FromMinutes(15));
        var refreshToken = _jwt.GenerateToken(user.UserName, user.Role, TimeSpan.FromHours(1));

        Response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

        //清除Cookie
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return LocalRedirect(returnUrl);
    }


    [HttpGet]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Register(string username, string password, string confirmPassword)
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

        var hasher = new PasswordHasher<Account>();
        _service.AddAccount(new Account
        {
            UserName = username,
            Password = hasher.HashPassword(new Account(), password),
            Role = Role.User
        });

        TempData["Message"] = "註冊成功，請登入";
        return RedirectToAction("Index");
    }
}
