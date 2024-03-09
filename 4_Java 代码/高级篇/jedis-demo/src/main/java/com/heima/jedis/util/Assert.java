package com.heima.jedis.util;

public class Assert {
    public static void notNull(Object obj, String msg){
        if (obj == null) {
            throw new RuntimeException(msg);
        }
    }
    public static void hasText(String str, String msg){
        if (str == null) {
            throw new RuntimeException(msg);
        }
        if (str.trim().isEmpty()) {
            throw new RuntimeException(msg);
        }
    }
}
