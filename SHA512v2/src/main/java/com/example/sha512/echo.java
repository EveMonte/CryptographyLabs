package com.example.sha512;

public class echo {

    public static String bytes(byte[] bytes, int n, int m, int s) {
        int div = n / m;
        int mod = n % m;
        String hash = "";
        if (s + n > bytes.length) {
            System.out.println("You ask to echo " + (s + n) + " byte(s)");
            System.out.println("Buffer only has " + bytes.length + " byte(s)");
            return null;
        }
        for (int i = 0; i != div; ++i) {
            for (int j = 0; j != m; ++j) {
                System.out.printf("%02X ", bytes[s + i * m + j]);
                hash += String.format("%02X", bytes[s + i * m + j]);

            } System.out.println();
        }

        for (int i = 0; i != mod; ++i) {
            System.out.printf("%02X ", bytes[s + div * m + i]);
            hash+=String.format("%02X", bytes[s + div * m + i]);

        } System.out.println();
        return hash;
    }

    public static String bytes(byte[] bytes, int n) {
        return echo.bytes(bytes, n, 16, 0);
    }

    public static String bytes(long[] longs, int n, int m, int s) {
        return bytes(pack.bytes(longs), n, m, s);
    }

    public static String bytes(long[] longs, int n) {
        return echo.bytes(longs, n, 16, 0);
    }

    public static void params(LyraParams params) {
        System.out.println("Echo LyraParams:");
        System.out.println("klen: " + params.klen);
        System.out.println("tcost: " + params.t_cost);
        System.out.println("mcost: " + params.m_cost);
        System.out.println("sponge: " + params.SPONGE);
        System.out.println("FULL_ROUNDS: " + params.FULL_ROUNDS);
        System.out.println("HALF_ROUNDS: " + params.HALF_ROUNDS);
        System.out.println("N_COLS: " + params.N_COLS);
        System.out.println("BLOCKS: " + params.BLOCK_LEN_INT64);
    }
}
