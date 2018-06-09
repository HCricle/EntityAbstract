using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedEA.Core.Model.DbContext;
using SharedEA.Core.Model.DbModels;
using SharedEA.Core.Web.Extensions;
using SharedEA.Core.WebApi.JWT;
namespace EntityAbstract.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SecretKey=Configuration["SecretKey"];
        }

        public IConfiguration Configuration { get; }
        private readonly string SecretKey;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJsonCore();
            services.ConfigureWebApiSettings(new WebApiSettings(Configuration[nameof(WebApiSettings.Host)], SecretKey));
            var settingOptions = Configuration.GetSection("JwtIssuerOptions");
            var issuer = settingOptions[nameof(TokenProviderOptions.Issuer)];
            var audience = settingOptions[nameof(TokenProviderOptions.Audience)];

            services.AddWebApi(options =>
            {
                options.SecretKey = SecretKey;
                options.Issuer = issuer;
                options.Audience = audience;
                options.ValidIssuer = "EntityAbstractIssuer";
                options.ValidAudience = "EntityAbstractTokenPrivoter";
            });

            services.AddEaDbContext<EaDbContext,EaUser>();
            services.AddServices();
            services.AddRepositories();
            services.AddMvc(options => options.AddAuthorizeFilter());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
