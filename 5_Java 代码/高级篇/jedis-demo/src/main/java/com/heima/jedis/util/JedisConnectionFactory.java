package com.heima.jedis.util;

import redis.clients.jedis.*;

public class JedisConnectionFactory {

    private static JedisPool jedisPool;
    private static final JedisCluster jedisCluster;

    static {
        // 配置连接池
        JedisPoolConfig poolConfig = new JedisPoolConfig();
        poolConfig.setMaxTotal(8);
        poolConfig.setMaxIdle(8);
        poolConfig.setMinIdle(0);
        poolConfig.setMaxWaitMillis(1000);
        // 创建连接池对象
        // jedisPool = new JedisPool(poolConfig,
        //         "192.168.150.101", 6379, 1000, "123321");
        jedisCluster = new JedisCluster(
                new HostAndPort("192.168.150.101", 7001), poolConfig);
    }

    public static Jedis getJedis(){
        return jedisPool.getResource();
    }

    public static JedisCluster getJedisCluster(){
        return jedisCluster;
    }
}
