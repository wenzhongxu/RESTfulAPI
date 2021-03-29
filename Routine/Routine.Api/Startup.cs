using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Routine.Api.DbContexts;
using Routine.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpCacheHeaders(expires =>
            {
                // 过期模型设置
                expires.MaxAge = 60;
                expires.CacheLocation = CacheLocation.Private;
            }, validation =>
            {
                //验证模型设置
                validation.MustRevalidate = true;
            });

            services.AddResponseCaching(); //注册缓存服务

            services.AddControllers(setup =>
            {
                setup.ReturnHttpNotAcceptable = true; //如果请求的类型和服务器所支持的类型不一致时，返回406
                //定义缓存策略
                setup.CacheProfiles.Add("120sCacheProfile", new CacheProfile
                {
                    Duration = 120
                });
            })
                .AddNewtonsoftJson(setup =>
                {
                    setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters() //添加xml格式化器。支持输入和输出
                .ConfigureApiBehaviorOptions(setup =>
                {
                    setup.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "http://www.baidu.com",
                            Title = "有错误！",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "请看详细信息",
                            Instance = context.HttpContext.Request.Path
                        };

                        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                })
                ;

            //在全局支持这种media type
            services.Configure<MvcOptions>(config =>
            {
                var newtonSoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                newtonSoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");

            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());//注册automapper服务

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Routine.Api", Version = "v1" });
            });

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddDbContext<RoutineDbContext>(options =>
            {
                options.UseSqlite("Data Source=routine.db");
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Routine.Api v1"));
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected Error!");
                    });
                });
            }

            //app.UseResponseCaching(); //缓存

            app.UseHttpCacheHeaders(); //过期模型 验证模型 缓存

            app.UseRouting();//用来标记路由决策在请求管道里发生的位置，也就是在这里会选择端点

            app.UseAuthorization();

            //用来标记选择好的端点在请求管道的什么地方来执行
            app.UseEndpoints(endpoints =>
            {
                //基于属性的路由
                endpoints.MapControllers();
            });
        }
    }
}
