using System;
using System.IO;
using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Sql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace AspNetCoreDashboardCustomPropertiesSample {
    public class Startup {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment) {
            string dataDirectoryPath = Path.Combine(hostingEnvironment.ContentRootPath, "Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectoryPath);

            Configuration = configuration;
            FileProvider = hostingEnvironment.ContentRootFileProvider;
            DashboardExportSettings.CompatibilityMode = DashboardExportCompatibilityMode.Restricted;
        }

        public IFileProvider FileProvider { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services
                .AddCors(options => {
                    options.AddPolicy("AllowAnyOrigin", builder => {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
                })
                .AddMvc()
                .AddDefaultDashboardController((configurator, serviceProvider)  => {
                    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(Configuration));

                    //DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(FileProvider.GetFileInfo("Data/Dashboards").PhysicalPath);
                    //configurator.SetDashboardStorage(dashboardFileStorage);

                    configurator.SetDashboardStorage(serviceProvider.GetService<SessionDashboardStorage>());

                    DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

                    // Registers an SQL data source.
                    DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
                    sqlDataSource.DataProcessingMode = DataProcessingMode.Client;
                    SelectQuery query = SelectQueryFluentBuilder
                        .AddTable("Categories")
                        .Join("Products", "CategoryID")
                        .SelectAllColumns()
                        .Build("Products_Categories");
                    sqlDataSource.Queries.Add(query);
                    dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());

                    // Registers an Object data source.
                    DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
                    dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());

                    // Registers an Excel data source.
                    DashboardExcelDataSource excelDataSource = new DashboardExcelDataSource("Excel Data Source");
                    excelDataSource.FileName = FileProvider.GetFileInfo("Data/Sales.xlsx").PhysicalPath;
                    excelDataSource.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Sheet1"));
                    dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml());

                    configurator.SetDataSourceStorage(dataSourceStorage);

                    configurator.DataLoading += (s, e) => {
                        if(e.DataSourceName == "Object Data Source") {
                            e.Data = Invoices.CreateData();
                        }
                    };
                });

            services.AddDevExpressControls();

            services
                .AddDistributedMemoryCache()
                .AddSession();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<SessionDashboardStorage>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            app.UseCors("AllowAnyOrigin");
            app.UseDevExpressControls();
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes => {
                routes.MapDashboardRoute("api");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}