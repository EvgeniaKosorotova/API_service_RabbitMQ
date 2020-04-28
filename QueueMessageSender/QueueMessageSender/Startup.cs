using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Models;
using System.Text;

namespace QueueMessageSender
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddDataAnnotations();
            services.AddControllers();
            services.AddSingleton<IQueueMessageSender, RMQMessageSender>();
            services.AddSingleton<AuthenticationJWT>();
            services.AddSingleton<HashGenerator>();
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration.GetValue<string>("Settings:JWT:Issuer"),
                        ValidAudience = Configuration.GetValue<string>("Settings:JWT:Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Settings:JWT:SecurityKey")))
                    };
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Queue Message Sender", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "QueueMessageSender V1");
                c.RoutePrefix = string.Empty;
            });
            if (env.IsDevelopment()) 
            {
                app.UseDeveloperExceptionPage();
            }
            else 
            {
                app.UseExceptionHandler(a => a.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;

                    var result = JsonConvert.SerializeObject(new ErrorModel { Error = exception.Message });
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(result);
                }));
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
