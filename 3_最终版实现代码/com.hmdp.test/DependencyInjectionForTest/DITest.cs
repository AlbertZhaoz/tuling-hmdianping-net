using Autofac;
using Autofac.Extensions.DependencyInjection;
using com.hmdp.config;
using com.hmdp.extension;
using com.hmdp.utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace com.hmdp.test.DependencyInjectionForTest;

public class DITest
{
    public IContainer DICollections()
    {
        var basePath = AppContext.BaseDirectory;
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        services.AddutoMapperSetup();
        services.AddAppSettingSetup(configuration);
        AppSetting.Init(services,configuration);
        services.AddModule(configuration);
        
        //实例化 AutoFac  容器   
        var builder = new ContainerBuilder();
        builder.RegisterInstance(new LoggerFactory())
            .As<ILoggerFactory>();
        builder.RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>))
            .SingleInstance();
        builder.RegisterModule(new AutofacModuleRegister());
        
        // 将 services 中的服务填充到 AutoFac 中
        builder.Populate(services);
        
        //使用已进行的组件登记创建新容器
        var ApplicationContainer = builder.Build();

        return ApplicationContainer;
    }
}