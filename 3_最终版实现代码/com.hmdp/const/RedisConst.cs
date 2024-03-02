namespace com.hmdp.Const;

public class RedisConst
{
    public const string LOGIN_CODE_KEY = "login:code:";
    public const double LOGIN_CODE_TTL = 2;
    public const string LOGIN_USER_KEY = "login:token:";
    public const double LOGIN_USER_TTL = 30;
    public const string CACHE_SHOP_KEY = "cache:shop:";
    public const string CACHE_NULL_VALUE = "selfnull";
    public const string USER_SIGN_KEY = "sign:";
    public const string USER_BLOG_LIKE = "blog:like:";
    public const string FEED_KEY = "feed:";
    public const string FOLLOWS_KEY = "follows:";
    public const string SHOP_GEO_KEY = "shop:geo:";
    public const string SECKILL_STOCK_KEY = "seckill:stock:";
    public const string SECKILL_ORDER_KEY = "seckill:order:";
    public const string LOCK_ORDER = "lock:order:";
    public const string ORDER = "order";
    public const string MESSAGE_STREAM_KEY = "streamOrders";
}