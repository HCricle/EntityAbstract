using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.Service;
using SharedEA.Core.Service.Extensions;
using SharedEA.Core.WebApi.Helpers;
using SharedEA.Core.Web.Extensions;
using Microsoft.AspNetCore.Http;
using SharedEA.Core.DbModel.RepositoryModel;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EntityAbstract.Api.Controllers
{
    [Area("api")]
    [Route("api/v1/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<EaUser> _userManager;
        private readonly SignInManager<EaUser> _signInManager;
        private readonly IOptions<WebApiSettings> _settings;
        private readonly EaDbContext _dbContext;
        private readonly IEmailSenderService _emailSender;
        public AccountController(UserManager<EaUser> userManager,
            SignInManager<EaUser> signInManager,
            EaDbContext dbContext,
            IEmailSenderService emailSender,
            IOptions<WebApiSettings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _emailSender = emailSender;
            _settings = settings;
        }
        [HttpGet("gum/uid={uid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserMoney(string uid)
        {
            var user = await _userManager.FindByIdAsync(uid);
            return Json(user?.Money);
        }
        [HttpGet("gubn/name={name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            EaUser u =new EaUser();
            if (user != null)
            {
                u.Id = user.Id;
                u.UserName = user.UserName;
                u.Type = user.Type;
                u.Email = user.Email;
            }
            return Json(u);
        }
        [HttpGet("gubi/id={id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            EaUser u = new EaUser();
            if (user != null)
            {
                u.Id = user.Id;
                u.UserName = user.UserName;
                u.Type = user.Type;
                u.Email = user.Email;
            }
            return Json(u);
        }
        [HttpGet("GetId/name={name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetId(string name)
        {
            return Ok((await _userManager.FindByNameAsync(name)).Id);
        }
        [HttpGet("gn/id={id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetName(string id)
        {
            return Ok((await _userManager.FindByIdAsync(id)).UserName);
        }
        // 查看是否登陆
        [HttpGet("isl")]
        [AllowAnonymous]
        public IActionResult IsLoad()
        {
            return Ok(GetToken() != null);
        }
        [HttpGet("gu/id={id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var token = GetToken();
            if (token == null)
            {
                return Unauthorized();
            }
            var user = (await _userManager.FindByIdAsync(id));
            if (user.UserName!=token.aud)
            {
                return Unauthorized();
            }
            return Ok(user);
        }
        // POST api/v1/account/login
        [HttpPost("Login")]
        //[ValidateAntiForgeryToken]//???
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] ApiLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Name, model.Pwd, true, false);
            if (result.Succeeded)
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.SecretKey));
                var options = new TokenProviderOptions()
                {
                    Issuer = "EntityAbstract",
                    Audience = "EntityAbstractAudience",
                    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                };
                var tpm = new TokenProvider(options);
                var token = await tpm.GenerateToken(model.Name, model.Pwd);
                var user = await _userManager.FindByNameAsync(model.Name);
                return token != null ? (IActionResult)Json(token) : BadRequest();
            }

            if (result.IsLockedOut)
            {
                return BadRequest("Lockout");
            }
            return BadRequest("Invalid login attempt");
        }
        [HttpPost("Register")]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] ApiRegisterModel model)
        {
            if (model==null||model.GetType().GetProperties().Any(p=>string.IsNullOrEmpty(p.GetValue(model).ToString())))
            {
                return BadRequest("模型验证失败");
            }
            if (_dbContext.Users.Where(u=>u.Email==model.Email).Any())
            {
                return BadRequest("邮箱已注册");
            }
            if (_dbContext.Users.Where(u=>u.UserName==model.UserName).Any())
            {
                return BadRequest("用户名已注册");
            }
            var user = new EaUser { UserName = model.UserName, Email = model.Email, CreateTime = DateTime.Now, Type = "default"};
            var result = await _userManager.CreateAsync(user, model.Pwd);
            if (result.Succeeded)
            {
                //_logger.LogInformation("User created a new account with password.");
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                var res = await _emailSender.SendEmailConfirmationAsync(model.Email, "");
                return Ok();
            }
            return BadRequest(result.Errors);

        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Confirmation(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("无法找到该用户");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return RedirectToAction(nameof(AccountController.Confirmation), "Account", result.Succeeded ? "ConfirmEmail" : "Error");   
        }
        // 忘记密码
        [HttpPost("fp")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword()
        {
            var token = GetToken();
            if (token==null)
            {
                return Unauthorized();
            }
            var user = await _userManager.FindByNameAsync(token.aud);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return BadRequest("用户不存在或此用户没通过邮箱验证");
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);
            return Ok();
        }

        // POST api/v1/accountapi/LogOff
        [HttpPost("LogOff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        [NonAction]
        public TokenDec GetToken()
        {
            return this.GetTokenDec(_settings.Value.SecretKey);
        }

    }
}
