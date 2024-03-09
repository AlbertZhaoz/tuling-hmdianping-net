using com.hmdp.config;
using com.hmdp.utils;

namespace com.hmdp.extension;

public static class AutoMapperExtension
{
    public static void AddutoMapperSetup(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperConfig));
    }
}