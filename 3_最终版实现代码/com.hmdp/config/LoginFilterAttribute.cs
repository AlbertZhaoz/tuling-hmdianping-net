using com.hmdp.dto;
using com.hmdp.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace com.hmdp.config;

public class LoginFilterAttribute:Attribute,IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.Any(item => item is IAllowAnonymous))
        {
            return;
        }
        
        if(UserHolder.GetUser() == null)
        {
            context.Result = new JsonResult(new
            {
                Code = 401,
                Info = "未登录"
            });
        }
    }
}