using AspNetCoreRateLimit;
using HotelLstWebApi.Data;
using HotelLstWebApi.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelLstWebApi
{
    public static class ServiceExtentions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUsers>(q => q.User.RequireUniqueEmail = true);
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();            
        }
        public static void ConfigureJWT(this IServiceCollection services,IConfiguration Configuration )
        {
            var jwtSetting = Configuration.GetSection("Jwt");
            var key = Environment.GetEnvironmentVariable("key");// this is a environment variable set from dos command by command setx key_name "key value" /M
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o=>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer=true,
                    ValidateLifetime=true,
                    ValidateAudience=false,
                    ValidateIssuerSigningKey=true,
                    ValidIssuer=jwtSetting.GetSection("Issuer").Value,
                    IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }
        public static void UseExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error=> {
                error.Run(async context => {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error($"Something went wrong in the {contextFeature.Error}");
                        await context.Response.WriteAsync(new Error
                        {
                            StatusCode=context.Response.StatusCode,
                            Message= "Internal server error. Please try later."
                        }.ToString());
                    }

                });
            });
        }
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt=>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion= new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(
                (expirationOpt)=>
                {
                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                    },
                (validationOpt) =>
                {
                    validationOpt.MustRevalidate = true;
                }
             );
        }
        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            var rateLimitRate = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint="*",
                    Limit=1,
                    Period="1s"
                }
            };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRate;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        }
    }
}
