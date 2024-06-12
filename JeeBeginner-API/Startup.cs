using JeeBeginner.Reponsitories.AccountManagement;
using JeeBeginner.Reponsitories.AccountRoleManagement;
using JeeBeginner.Reponsitories.Authorization;
using JeeBeginner.Reponsitories.CustomerManagement;
using JeeBeginner.Reponsitories.DM_DVT;
using JeeBeginner.Reponsitories.DM_LoaiMatHang;
using JeeBeginner.Reponsitories.DM_LoaiTaiSan;
using JeeBeginner.Reponsitories.DM_LyDoTangGiamTS;
using JeeBeginner.Reponsitories.DM_MatHang;
using JeeBeginner.Reponsitories.DM_NhanHieu;
using JeeBeginner.Reponsitories.DM_NhomTaiSan;
using JeeBeginner.Reponsitories.DM_XuatXu;
using JeeBeginner.Reponsitories.PartnerManagement;
using JeeBeginner.Reponsitories.TaikhoanManagement;
using JeeBeginner.Services;
using JeeBeginner.Services.AccountManagement;
using JeeBeginner.Services.AccountRoleManagement;
using JeeBeginner.Services.Authorization;
using JeeBeginner.Services.CustomerManagement;
using JeeBeginner.Services.DM_DoiTacBaoHiem;
using JeeBeginner.Services.DM_DVT;
using JeeBeginner.Services.DM_LoaiMatHang;
using JeeBeginner.Services.DM_LoaiTaiSan;
using JeeBeginner.Services.DM_LyDoTangGiamTS;
using JeeBeginner.Services.DM_MatHang;
using JeeBeginner.Services.DM_NhanHieu;
using JeeBeginner.Services.DM_NhomTaiSan;
using JeeBeginner.Services.DM_XuatXu;
using JeeBeginner.Services.PartnerManagement;
using JeeBeginner.Services.TaikhoanManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Text;
using OfficeOpenXml;
using Microsoft.Extensions.FileProviders;
using Minio.DataModel;
using System.IO;

namespace JeeBeginner
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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            services.AddCors(o => o.AddPolicy("AllowOrigin", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddControllers().AddNewtonsoftJson();
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages().AddNewtonsoftJson();
            services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                //options.RequireHttpsMetadata = false;
                //options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"])),
                };
            });

            //Swagger
            services.AddSwaggerGen(c =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    //Scheme = "bearer", // must be lower case
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                        {
                                            {securityScheme, new string[] { }}
                                        });
            });

            services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddOptions();
            #region add Repository
            services.AddTransient<IAccountManagementRepository, AccountManagementRepository>();
            services.AddTransient<IDM_LoaiMatHangRepository, DM_LoaiMatHangRepository>();
            services.AddTransient<IDM_DVTRepository, DM_DVTRepository>();
            services.AddTransient<IDM_NhanHieuRepository, DM_NhanHieuRepository>();
            services.AddTransient<IDM_XuatXuRepository, DM_XuatXuRepository>();
            services.AddTransient<IDM_DoiTacBaoHiemRepository, DM_DoiTacBaoHiemRepository>();
            services.AddTransient<IDM_LoaiTaiSanRepository, DM_LoaiTaiSanRepository>();
            services.AddTransient<IDM_NhomTaiSanRepository, DM_NhomTaiSanRepository>();
            services.AddTransient<IDM_LyDoTangGiamTSRepository, DM_LyDoTangGiamTSRepository>();
            services.AddTransient<IDM_MatHangRepository, DM_MatHangRepository>();
            services.AddTransient<IPartnerManagementRepository, PartnerManagementRepository>();
            services.AddTransient<IAuthorizationRepository, AuthorizationRepository>();
            services.AddTransient<ICustomerManagementRepository, CustomerManagementRepository>();
            services.AddTransient<ITaikhoanManagementRepository, TaikhoanManagementRepository>();
            services.AddTransient<IAccountRoleManagementRepository, AccountRoleManagementRepository>();
            #endregion add Repository
            #region add service
            services.AddTransient<IPartnerManagementService, PartnerManagementService>();
            services.AddTransient<IDM_LoaiMatHangService, DM_LoaiMatHangService>();
            services.AddTransient<IDM_DVTService, DM_DVTService>();
            services.AddTransient<IDM_NhanHieuService, DM_NhanHieuService>();
            services.AddTransient<IDM_XuatXuService, DM_XuatXuService>();
            services.AddTransient<IDM_DoiTacBaoHiemService, DM_DoiTacBaoHiemService>();
            services.AddTransient<IDM_LoaiTaiSanService, DM_LoaiTaiSanService>();
            services.AddTransient<IDM_NhomTaiSanService, DM_NhomTaiSanService>();
            services.AddTransient<IDM_LyDoTangGiamTSService, DM_LyDoTangGiamTSService>();
            services.AddTransient<IDM_MatHangService, DM_MatHangService>();
            services.AddTransient<IAccountManagementService, AccountManagementService>();
            services.AddTransient<ICustomAuthorizationService, CustomAuthorizationService>();
            services.AddTransient<ICustomerManagementService, CustomerManagementService>();
            services.AddTransient<ITaikhoanManagementService, TaikhoanManagementService>();
            services.AddTransient<IAccountRoleManagementService, AccountRoleManagementService>();
            #endregion add service
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JeeBeginner v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowOrigin");

            app.UseAuthorization();

            app.UseStaticFiles();// For the wwwroot folder

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            var currentDirectory = Directory.GetCurrentDirectory();
            var uploadsFolder = Path.Combine(currentDirectory, "Images");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsFolder),
                RequestPath = "/Images"
            });
        }
    }
}