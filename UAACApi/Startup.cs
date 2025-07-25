using Lib;
using Lib.Api;
using Lib.Api.Configs;
using Lib.Api.Middlewares;
using Lib.Api.ModelBinders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Models;
using Repositorys;
using Services;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace UAACApi
{
    public class Startup
    {
        public const string CychCorsPolicy = "CychCorsPolicy";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // �ϸ� Api �i�� big5
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);

            services
                .AddControllers(options =>
                {
                    options.AllowEmptyInputInBodyModelBinding = true;
                    options.ModelBinderProviders.Insert(0, new StringBinderProvider());
                })
                .AddJsonOptions(options =>
                {
                    // System.Text.Json: �x����N Newtonsoft Json.NET ���ѨM���

                    // ���\�򥻩ԤB�^��Τ�������r������r��
                    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

                    // Use the default property casing
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = null;

                    options.JsonSerializerOptions.WriteIndented = true;

                    //options.JsonSerializerOptions.IgnoreNullValues = true;
                });
            //.AddNewtonsoftJson(options =>
            //{
            //    // Newtonsoft Json.NET: �w�]�Y�i�� Response �����줤�夺�e

            //    // Use the default property casing
            //    //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //    options.UseMemberCasing();

            //    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //});

            var appSettings = Configuration.Get<AppSettings>();
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearerConfig(appSettings);

            services.AddCors(options =>
            {
                options.AddPolicy(CychCorsPolicy, builder =>
                {
                    builder
                        // �]�w���\��쪺�ӷ��A���h�Ӫ��ܥi�� `,` �j�}
                        //.WithOrigins("http://*.cych.org.tw", "https://*.cych.org.tw", "*")
                        //.SetIsOriginAllowedToAllowWildcardSubdomains()
                        .SetIsOriginAllowed(origin => true) // allow any origin
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "UAACApi", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSingleton<ApiUtilLocator>();
            services.AddSingleton<UtilLocator>();
            services.AddScoped<DBContext>();
            services.AddScoped<AuthService>();
            services.AddScoped<PermissionService>();
            services.AddScoped<BatchService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // �i�d�I���U�Ӥ����n�餤�Y�^�����B�z�ҥ~���p�C
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", "UAACApi v1"));

            app.UseLoggerMiddleware();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(CychCorsPolicy);

            app.UseAuthentication(); // �����ҡB�A���v

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
