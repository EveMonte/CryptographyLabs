package com.example.sha512;

/**
 * Методы для работы с памятью
 */
public class mem {
    /**
     * Копирует целое число в массив байтов, начиная с отступа
     * Переворачивает порядок байтов
     * dst       массив, который получает целое число
     * offset    индекс массива, куда записывается первый байт
     * src       число для операции
     */
    public static void copy(byte[] dst, int offset, int src) {
        dst[offset + 0] = (byte) (src       );
        dst[offset + 1] = (byte) (src >>>  8);
        dst[offset + 2] = (byte) (src >>> 16);
        dst[offset + 3] = (byte) (src >>> 24);
    }

    /**
     * Меняет порядок little-endian на big-endian
     * x число для переворота
     */
    public static long flip(final long x) {
        return    (x & 0x00000000000000FFL)  << 56
                | (x & 0x000000000000FF00L)  << 40
                | (x & 0x0000000000FF0000L)  << 24
                | (x & 0x00000000FF000000L)  <<  8
                | (x & 0x000000FF00000000L) >>>  8
                | (x & 0x0000FF0000000000L) >>> 24
                | (x & 0x00FF000000000000L) >>> 40
                | (x & 0xFF00000000000000L) >>> 56
                ;
    }
}
