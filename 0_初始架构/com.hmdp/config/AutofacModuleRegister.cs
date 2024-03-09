using Autofac;
using com.hmdp.attribute;
using com.hmdp.controller;

namespace com.hmdp.config;

public class AutofacModuleRegister: Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 1. 批量注册所有服务层服务
        builder.RegisterAssemblyTypes(typeof(AutofacModuleRegister).Assembly)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .PropertiesAutowired(new PropertySelector());
        
        // 2. 注册每一个控制器和抽象之间的关系
        var controllerBaseType = typeof(BaseController);
        builder.RegisterAssemblyTypes(typeof(AutofacModuleRegister).Assembly)
            .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            // 支持属性注入
            .PropertiesAutowired(new PropertySelector());
        
        #region 没有接口层的服务层注入
        //因为没有接口层，所以不能实现解耦，只能用 Load 方法。
        //注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
        //var assemblysServicesNoInterfaces = Assembly.Load("Blog.Core.Services");
        //builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);
        // // 1. 注册所有服务层服务
        // builder.RegisterGeneric(typeof(BaseService<>))
        //     .As(typeof(IBaseService<>))
        //     .InstancePerDependency(); 
        //
        // builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(AutofacModuleRegister)))
        //     .AsImplementedInterfaces()
        //     .InstancePerDependency()
        //     .PropertiesAutowired(); //允许将拦截器服务的列表分配给注册。
        // builder.RegisterType<ShopService>().As<IShopService>()
        //     .AsImplementedInterfaces()
        //     .InstancePerDependency();

        // 2. 注册 ShopService 类作为 IShopService 接口的实现
        // 获取 Service.dll 程序集服务，并注册
        // var assemblysServices = Assembly.LoadFrom(servicesDllFile);
        // builder.RegisterAssemblyTypes(assemblysServices)
        //     .AsImplementedInterfaces()
        //     .InstancePerDependency()
        //     .PropertiesAutowired()
        //     .EnableInterfaceInterceptors()       //引用Autofac.Extras.DynamicProxy;
        //     .InterceptedBy(cacheType.ToArray()); //允许将拦截器服务的列表分配给注册。
        #endregion

        #region 没有接口的单独类，启用class代理拦截
        //只能注入该类中的虚方法，且必须是public
        //这里仅仅是一个单独类无接口测试，不用过多追问
        // builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Love)))
        //     .EnableClassInterceptors()
        //     .InterceptedBy(cacheType.ToArray());
        #endregion

        #region 单独注册一个含有接口的类，启用interface代理拦截
        //不用虚方法
        //builder.RegisterType<AopService>().As<IAopService>()
        //   .AsImplementedInterfaces()
        //   .EnableInterfaceInterceptors()
        //   .InterceptedBy(typeof(BlogCacheAOP));
        #endregion
    }
}