package com.heima;

import io.lettuce.core.TrackingArgs;
import io.lettuce.core.api.StatefulRedisConnection;
import io.lettuce.core.support.caching.CacheAccessor;
import io.lettuce.core.support.caching.CacheFrontend;
import io.lettuce.core.support.caching.ClientSideCaching;
import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.data.redis.connection.RedisConnection;
import org.springframework.data.redis.connection.RedisConnectionFactory;
import org.springframework.data.redis.connection.lettuce.LettuceConnection;

import javax.annotation.Resource;
import java.lang.reflect.Method;
import java.nio.charset.StandardCharsets;
import java.util.concurrent.ConcurrentHashMap;

@SpringBootTest
public class LettuceTest {

    @Resource
    private RedisConnectionFactory connectionFactory;

    @Test
    void testClientCache() throws Exception {
        RedisConnection connection = connectionFactory.getConnection();

        if(connection instanceof LettuceConnection){
            Method method = LettuceConnection.class.getDeclaredMethod("doGetAsyncDedicatedConnection");
            method.setAccessible(true);
            StatefulRedisConnection<byte[], byte[]> connect = (StatefulRedisConnection<byte[], byte[]>) method.invoke(connection);

            CacheFrontend<byte[], byte[]> cacheFrontend = ClientSideCaching.enable(
                    CacheAccessor.forMap(new ConcurrentHashMap<>()),
                    connect,
                    TrackingArgs.Builder.enabled()
            );

            while (true){
                byte[] bytes = cacheFrontend.get("user".getBytes(StandardCharsets.UTF_8));
                System.out.println("user = " + new String(bytes));

                Thread.sleep(3000);
            }
        }
    }
}
