using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedEA.Core.DataService.Services;
using SharedEA.Core.Service;
using SharedEA.Core.Web.Options;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Data.Repositories;
using System;
using System.Linq;
using System.Text;

namespace SharedEA.Core.Web.Extensions
{
    public static class IServiceCollectionExtensions
    {
        private static SymmetricSecurityKey GetSigningKey(string secretKey) => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        /// <summary>
        /// 加入数据库模型响应包含
        /// <list type="table">
        ///     <item>EmailSenderService</item>
        ///     <item>GroupRepository</item>
        ///     <item>ContentRepository</item>
        ///     <item>CommentRepository</item>
        ///     <item>LikeRepository</item>
        ///     <item>MsgRespository</item>
        ///     <item>ContentFileRepository</item>
        /// </list>
        /// </summary>
        /// <param name="services"></param>
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.AddTransient<GroupRepository>();
            services.AddTransient<ContentRepository>();
            services.AddTransient<CommentRepository>();
            services.AddTransient<LikeRepository>();
            services.AddTransient<MsgRepository>();
            services.AddTransient<MsgDetailRepository>();
            services.AddTransient<ContentFileRepository>();
            services.AddTransient<PreLoadRepository>();
            services.AddTransient<ShopRepository>();
            services.AddTransient<ShopItemRepository>();
            services.AddTransient<ACmdRepository>();
            services.AddTransient<BuyRepository>();
            services.AddTransient<FriendRepository>();
        }
        public static void AddEaDbContext<TDbContext,TUser>(this IServiceCollection services)
            where TDbContext:DbContext
            where TUser:IdentityUser
        {
            services.AddDbContext<TDbContext>();

            services.AddIdentity<TUser, IdentityRole>(opetions =>
            {
                opetions.Password.RequiredLength = 6;
                opetions.Password.RequireNonAlphanumeric = false;
                opetions.Password.RequireDigit = false;
#if !DEBUG
                opetions.SignIn.RequireConfirmedEmail = true;
#endif
            })
                .AddEntityFrameworkStores<TDbContext>()
                .AddDefaultTokenProviders();
        }
        /// <summary>
        /// 加入Microsoft外部登陆
        /// </summary>
        /// <param name="services"></param>
        public static void AddMicrosoftAccount(this IServiceCollection services)
        {
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = "";
                microsoftOptions.ClientSecret = "";
            });
        }
        /// <summary>
        /// 加入gzip压缩
        /// </summary>
        /// <param name="services"></param>
        public static void AddGzipCompress(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml", "text/html", "image/png", "image/jpg", });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });
        }
        /// <summary>
        /// 加入服务，包含
        /// <list type="bullet">
        ///     <item>EmailSenderService</item>
        ///     <item>XmlDataService</item>
        /// </list>
        /// </summary>
        /// <param name="services"></param>
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<EmailSenderService>();
            services.AddTransient<XmlDataService>();
        }
        /// <summary>
        /// 加入jsonformatters
        /// </summary>
        /// <param name="services"></param>
        public static void AddJsonCore(this IServiceCollection services)
        {
            services.AddCors();
            services.AddMvcCore()
                .AddJsonFormatters();
            services.AddOptions();
        }
        /// <summary>
        /// 加入WebApi设置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="webApiSettings"></param>
        public static void ConfigureWebApiSettings(this IServiceCollection services,WebApiSettings webApiSettings)
        {
            services.Configure<WebApiSettings>(settings => settings.Host = webApiSettings.Host);
            services.Configure<WebApiSettings>(settings => settings.SecretKey = webApiSettings.SecretKey);
        }
        /// <summary>
        /// 添加webapi,会将api token 提供者注册好的
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public static void AddWebApi(this IServiceCollection services,Action<WebApiOptions> action)
        {
            var apiOptions = new WebApiOptions();
            action?.Invoke(apiOptions);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSigningKey(apiOptions.SecretKey),
                ValidateIssuer = true,
                ValidIssuer = apiOptions.ValidIssuer,//EntityAbstract
                ValidateAudience = true,
                ValidAudience = apiOptions.ValidAudience,//EntityAbstractAudience
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
            services.Configure<TokenProviderOptions>(options =>
            {
                options.Issuer = apiOptions.Issuer;
                options.Audience = apiOptions.Audience;
                options.SigningCredentials = new SigningCredentials(GetSigningKey(apiOptions.SecretKey), SecurityAlgorithms.HmacSha256);
            })
            .AddAuthorization(options =>
            {
                apiOptions.PolicyBuilder(options);
            })
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = apiOptions.UseHttps;//不使用https
                options.Audience = apiOptions.Audience;//??
                options.ClaimsIssuer = apiOptions.Issuer;
                options.TokenValidationParameters = tokenValidationParameters;
                options.SaveToken = true;
            });

        }
    }
}
