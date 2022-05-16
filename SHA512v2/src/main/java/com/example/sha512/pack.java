package com.example.sha512;

import java.util.List;

/**
 * Pack bytes as longs and longs as bytes.
 *
 * The class name is lowercase because its methods are static.
 */
public class pack {
    /**
     * Упаковывает число в массив байтов
     * Наиболее значимые биты попадают в начало
     * x число для упаковки
     */
    public static byte[] bytes(long x) {
        byte[] bytes = new byte[8];

        for (int i = 0; i != 8; ++i) {
            bytes[i] = (byte) (x >>> (56 - 8 * i));
        }

        return bytes;
    }

    /**
     * Упаковывает массив чисел в массив байт
     * Наиболее значимые биты получают наименьший индекс
     * longs массив чисел для упаковки
     */
    public static byte[] bytes(long[] longs) {
        byte[] bytes = new byte[8 * longs.length];

        for (int i = 0; i != longs.length; ++i) {
            for (int j = 0; j != 8; ++j) {
                bytes[8 * i + j] = (byte) (longs[i] >>> (56 - j * 8));
            }
        }

        return bytes;
    }

    /**
     * Упаковывает массив строк в массив байтов
     * Для каждой строки из массива берет первые 2 буквы и преобразовывает в шестнадцатеричное число
     * strings массив строк для конвертации
     */
    public static byte[] bytes(String[] strings) {
        byte[] bytes = new byte[strings.length];

        for (int i = 0; i != strings.length; ++i) {
            final byte b0 = Byte.parseByte(strings[i].substring(0, 1), 16);
            final byte b1 = Byte.parseByte(strings[i].substring(1, 2), 16);

            bytes[i] = (byte) ((b0 << 4) | b1);
        }

        return bytes;
    }

    /**
     * Упаковывает список строк в массив байт
     * Для каждой строки из списка берется первые две буквы и преобразовываются в шестнадцатеричное число
     * strings списко строк для преобразования
     */
    public static byte[] bytes(List<String> strings) {
        return pack.bytes(strings.toArray(new String[strings.size()]));
    }

    /**
     * Упаковывает массив байтов в массив чисел
     * Меньший индекс означаает более значимый бит. Если количество
     * предложенных битов не кратно 8, последнее число дополнит
     * наименее значимые биты нулями
     * bytes массив байтов для упаковки
     */
    public static long[] longs(byte[] bytes) {
        int div = bytes.length / 8;
        int mod = bytes.length % 8;

        long[] longs = new long[div + (mod == 0 ? 0 : 1)];

        for (int i = 0; i != div; ++i) {
            long l = 0L;

            for (int j = 0; j != 7; ++j, l <<= 8) {
                // Upcasting a negative value gives a negative value
                // So, mask the result of an upcast to last byte only
                l |= (bytes[i * 8 + j] & 0x00000000000000FFL);

            } l |= bytes[i * 8 + 7] & 0x00000000000000FFL;

            longs[i] = l;
        }

        if (mod != 0) {
            long l = 0;

            for (int i = 0; i != mod - 1; ++i) {
                l |= (bytes[div * 8 + i] & 0x00000000000000FFL);

                l <<= 8;
            } l |= (bytes[div * 8 + mod - 1] & 0x00000000000000FFL);

            l <<= (8 * (8 - mod));

            longs[div] = l;
        }

        return longs;
    }
}
