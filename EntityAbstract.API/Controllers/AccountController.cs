using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedEA.Core.Model.DbContext;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Core.Model.DbModels;
using SharedEA.Core.EmailService;
using SharedEA.Core.EmailService.Extensions;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EntityAbstract.Api.Controllers
{
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
        [HttpGet("GetId/name={name}")]
        [AllowAnonymous]
        public async Task<object> GetId(string name)
        {
            return Ok((await _userManager.FindByNameAsync(name)).Id);
        }
        // POST api/v1/account/login
        [HttpPost("Login")]
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
            else
            {
                return BadRequest("Invalid login attempt");
            }
        }
        [HttpPost("Register")]
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
#warning 这里没改
                //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
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
            return View("~/Views/Confirmation", new ConfirmationModel() { Info = result.Succeeded ? "已确认邮箱" : "确认错误" });
        }
        // POST api/v1/accountapi/LogOff
        [HttpPost("LogOff")]
        [Authorize]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        
    }
}
