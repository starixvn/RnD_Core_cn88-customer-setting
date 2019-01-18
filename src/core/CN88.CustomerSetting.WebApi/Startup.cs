namespace CN88.CustomerSetting.WebApi
{
    using System;
    using System.Linq;
    using CN88.CustomerSetting.DataAccesses;
    using CN88.CustomerSetting.Services;
    using CN88.CustomerSetting.WebApi.Configurations;
    using CN88.CustomerSetting.WebApi.Handlers;
    using CN88.CustomerSetting.WebApi.Middlewares;
    using Fanex.Caching;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.RemotingRef;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SharpRaven;
    using Swashbuckle.AspNetCore.Swagger;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            InitLogging(services);

            InitFanexData();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            var customerSettingApiConfigs = new CustomerSettingApiConfigs();
            Configuration.Bind("CustomerSettingApi", customerSettingApiConfigs);


            services
                .AddSingleton(customerSettingApiConfigs)
                .AddSingleton<IDynamicRepository, DynamicRepository>()
                .AddSingleton<ICacheService, CacheService>()
                .AddSingleton<ValidationService, ValidationServiceImpl>()
                .AddSingleton<UpdateSettingService, UpdateSettingServiceImpl>()
                .AddSingleton<CreateCustomerService, CreateCustomerServiceImpl>()
                .AddSingleton<ResourceService, ResourceServiceImpl>()
                .AddSingleton<ProductService, ProductServiceImpl>()
                .AddSingleton<QuerySettingService, QuerySettingServiceImpl>()
                .AddSingleton<SettingTypeServiceFactory, SettingTypeServiceFactoryImpl>()
                .AddSingleton<CustomerDataAccess, CustomerDataAccessImpl>()
                .AddSingleton<IRemotingRefService, RemotingRefService>()
                .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "CN88 Customer Setting", Version = "v1" });
            });

            services.AddMemoryCache();
            services.AddAuthentication(o => o.AddScheme("api", a => a.HandlerType = typeof(HmacAuthenticationHandler)));
            services.AddMvcCore().AddJsonFormatters().AddAuthorization();

            InitRemotingRef();
        }

        private void InitFanexData()
        {
            var connections = Configuration
               .GetSection("ConnectionStrings")
               .GetChildren()
               .ToDictionary(connection => connection.Key, connection => connection.Value, StringComparer.OrdinalIgnoreCase);

            DbSettingProviderManager
              .StartNewSession()
              .UseConnectionStrings(connections)
              .UseDefaultDbSettingProvider(Configuration["StoredProcedurePath"])
              .Run();
        }

        private void InitLogging(IServiceCollection services)
        {
            LogManager
                    .SetDefaultLogCategory(Configuration["Fanex.Logging:DefaultCategory"])
                    .Use(new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(Configuration["Fanex.Logging:SentryUrl"])
                    }));

            services.AddSingleton(Logger.Log);

            ExceptionHandler
                // To use custom config
                .UseConfig(new LoggingConfiguration()
                .GetHttpHeaderInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo));
        }

        private void InitRemotingRef()
        {
            RefDataTypeContainer
                .ConnectTo(new Uri(Configuration["RemotingRef:Url"]))
                .FromApplication(Configuration["RemotingRef:SiteName"])
                .Register(RefDataType.BetTypeName, new RefDataTypeHandlerOptions { SlidingTimeout = TimeSpan.MaxValue })
                .Register(RefDataType.SportName, new RefDataTypeHandlerOptions { SlidingTimeout = TimeSpan.MaxValue })
                .Register(RefDataType.RefResource, new RefDataTypeHandlerOptions { SlidingTimeout = TimeSpan.MaxValue });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger()
               .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"../swagger/v1/swagger.json", "CN88 Customer Setting Docs");
                })
                .UseMiddleware(typeof(LocalizationMiddleware))
                .UseMiddleware(typeof(ErrorHandlingMiddleware))
                .UseMvc(
                    routes =>
                    {
                        routes.MapRoute(
                            name: "default",
                            template: "{controller=Home}/{action=Index}");
                    });
        }
    }
}