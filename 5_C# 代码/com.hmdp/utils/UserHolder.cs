using com.hmdp.dto;

namespace com.hmdp.utils;

public class UserHolder
{
    private static AsyncLocal<tb_user_dto> t1 = new AsyncLocal<tb_user_dto>();

    public static void SaveUser(tb_user_dto userDto)
    {
        t1.Value = userDto;
    }

    public static tb_user_dto GetUser()
    {
        return t1.Value;
    }

    public static void RemoveUser()
    {
        t1.Value = null;
    }
}