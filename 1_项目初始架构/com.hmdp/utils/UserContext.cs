namespace com.hmdp.utils;

public class UserContext
{
    private static HttpContext _context;

    public static void Configure(HttpContext context)
    {
        _context = context;
    }
    
    public static UserContext Current
    {
        get
        {
            return _context.RequestServices.GetService(typeof(UserContext)) as UserContext;
        }
    }
}