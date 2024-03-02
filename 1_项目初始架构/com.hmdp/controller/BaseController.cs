using com.hmdp.dto;
using com.hmdp.repo;
using Microsoft.AspNetCore.Mvc;

namespace com.hmdp.controller;

public class BaseController:ControllerBase
{
    [NonAction]
    public Result SuccessPage<T>(int page, int dataCount, int pageSize, List<T> data,
        int pageCount, string msg = "获取成功")
    {
        return new Result()
        {
            success = true,
            errorMsg = msg,
            data = new PageModel<T>(page, dataCount, pageSize, data)
        };
    }

    [NonAction]
    public Result SuccessPage<T>(PageModel<T> pageModel, string msg = "获取成功")
    {
        return new Result()
        {
            success = true,
            errorMsg = msg,
            data = pageModel
        };
    }
}