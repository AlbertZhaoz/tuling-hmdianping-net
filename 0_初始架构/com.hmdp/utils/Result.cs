using NetTaste;

namespace com.hmdp.dto;

/// <summary>
/// 通用信息返回类
/// </summary>
public class Result
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int status { get; set; } = 200;
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool success { get; set; } = false;
    /// <summary>
    /// 返回信息
    /// </summary>
    public string? errorMsg { get; set; } = "";
    /// <summary>
    /// 返回数据集合
    /// </summary>
    public object data { get; set; }

    public long? total { get; set; } = 1L;
    
    /// <summary>
    /// 返回消息
    /// </summary>
    /// <param name="success">失败/成功</param>
    /// <param name="msg">消息</param>
    /// <param name="response">数据</param>
    /// <returns></returns>
    public static Result Message(bool success,string errorMsg, object data, long? total = 1L)
    {
        return new Result() { errorMsg = errorMsg, data = data, success = success, total = total};
    }
    
    /// <summary>
    /// 返回成功
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    public static Result Success(string msg = "") {
        return Message(true, msg, default, default);
    }
    
    /// <summary>
    /// 返回成功
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    public static Result Success(string msg,object data) {
        return Message(true, msg, data, default);
    }
    
    
    /// <summary>
    /// 返回成功
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    public static Result Success(object data) {
        return Message(true, "", data, default);
    }
    /// <summary>
    /// 返回失败
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    public static Result Fail(string msg)
    {
        return Message(false, msg, default);
    }
    /// <summary>
    /// 返回失败
    /// </summary>
    /// <param name="msg">消息</param>
    /// <param name="response">数据</param>
    /// <returns></returns>
    public static Result Fail(string msg, object response)
    {
        return Message(false, msg, response);
    }
}