package com.example.sha512;

/**
 * Представляет губку и реализует общие методы
 */
public abstract class Sponge {
    static final long[] blake2b_IV = {
            0x6a09e667f3bcc908L, 0xbb67ae8584caa73bL,
            0x3c6ef372fe94f82bL, 0xa54ff53a5f1d36f1L,
            0x510e527fade682d1L, 0x9b05688c2b3e6c1fL,
            0x1f83d9abfb41bd6bL, 0x5be0cd19137e2179L
    };

    public long[] state;

    public final int N_COLS;

    public final int FULL_ROUNDS;
    public final int HALF_ROUNDS;

    public final int BLOCK_LEN_INT64;
    public final int BLOCK_LEN_BYTES;

    /**
     * Строит губку и инициализирует ее состояние
     * Инициализирует первую половину состояния нулями, а вторую с
     * помощью blake2b_IV. Это подразумевает, что состояние имеет 16 * 8 = 128 байтов
     */
    public Sponge(LyraParams params) {
        // initialize the sponge state:
        state = new long[16];
        // first 8 words are zeroed out
        for (int i = 0; i != 8; ++i) {
            state[i] = 0;
        }

        // second 8 words are blake2b_IV's
        for (int i = 0; i != 8; ++i) {
            state[8 + i] = mem.flip(blake2b_IV[i]);
        }

        this.N_COLS = params.N_COLS;
        this.FULL_ROUNDS = params.FULL_ROUNDS;
        this.HALF_ROUNDS = params.HALF_ROUNDS;
        this.BLOCK_LEN_INT64 = params.BLOCK_LEN_INT64;
        this.BLOCK_LEN_BYTES = params.BLOCK_LEN_BYTES;
    }

    /**
     * Поглощает слова в губку
     * src    исходный массив слов для поглощения
     * len    количество слов для поглощения из src
     * offset индекс элемента из src, с которого начинать
     */
    public void absorb(final long[] src, final int len, final int offset) {
        for (int i = 0; i != len; ++i) {
            state[i] ^= src[offset + i];
        }

        sponge_lyra();
    }

    /**
     * Выжимает байты из губки
     * dst целевой массив для выжимки
     * len количество байт для выжимания в dst
     */
    public void squeeze(byte[] dst, final int len) {
        final int div = len / BLOCK_LEN_BYTES;
        final int mod = len % BLOCK_LEN_BYTES;

        // Assume block size is a multiple of 8 bytes
        for (int i = 0; i != div; ++i) {
            final int offset0 = i * BLOCK_LEN_BYTES;

            for (int j = 0; j != BLOCK_LEN_INT64; ++j) {
                final int offset1 = offset0 + 8 * j;

                byte[] bytes = pack.bytes(state[j]);

                for (int k = 0; k != 8; ++k) {
                    dst[offset1 + k] = bytes[k];
                }
            }

            sponge_lyra();
        }

        final int div8 = mod / 8;
        final int mod8 = mod % 8;

        final int offset0 = div * BLOCK_LEN_BYTES;

        for (int i = 0; i != div8; ++i) {
            final int offset1 = offset0 + 8 * i;

            final byte[] bytes = pack.bytes(state[i]);

            for (int j = 0; j != 8; ++j) {
                dst[offset1 + j] = bytes[j];
            }
        }

        final int offset1 = offset0 + 8 * div8;

        for (int i = 0; i != mod8; ++i) {
            final byte[] bytes = pack.bytes(state[div8]);

            for (int j = 0; j != mod8; ++j) {
                dst[offset1 + j] = bytes[j];
            }
        }
    }

    /**
     * Свдиг слов на определенное количество битов вправо
     * Rotate a word by several bits to the right.
     * word слово для сдвига
     * b    величина сдвига
     * a    результат сдвига
     */
    public static long rotr64(final long word, final int b) {
        return (word << (64 - b)) | (word >>> b);
    }

    /**
     * Сдвиг слов на определенное количество битов влево
     * word слово для свдига
     * b    величина сдвига
     * a    результат сдвига
     */
    public static long rotl64(final long word, final int b) {
        return (word << b) | (word >>> (64 - b));
    }

    public abstract void G(final int a, final int b, final int c, final int d);

    /**
     * Обноавляет состояние губки
     * rounds количество перестановок
     */
    public void sponge_lyra(final int rounds) {
        for (int round = 0; round != rounds; ++round) {
            G(0, 4,  8, 12);
            G(1, 5,  9, 13);
            G(2, 6, 10, 14);
            G(3, 7, 11, 15);
            G(0, 5, 10, 15);
            G(1, 6, 11, 12);
            G(2, 7,  8, 13);
            G(3, 4,  9, 14);
        }
    }

    /**
     * Обновляет состояние губки, запускает полное количество раундов по умолчанию.
     */
    public void sponge_lyra() {
        sponge_lyra(FULL_ROUNDS);
    }

    public void reduced_squeeze_row0(long[] dst, final int offset) {
        int word = (N_COLS - 1) * BLOCK_LEN_INT64;

        for (int i = 0; i != N_COLS; ++i) {
            System.arraycopy(state, 0, dst, offset + word, BLOCK_LEN_INT64);

            word -= BLOCK_LEN_INT64;

            sponge_lyra(HALF_ROUNDS);
        }
    }

    public void reduced_duplex_row1_and_row2(long[] dst, final int offset1, final int offset2) {
        int word1 = 0;
        int word2 = (N_COLS - 1) * BLOCK_LEN_INT64;

        for (int i = 0; i != N_COLS; ++i) {
            for (int j = 0; j != BLOCK_LEN_INT64; ++j) {
                state[j] ^= dst[offset1 + word1 + j];
            }

            sponge_lyra(HALF_ROUNDS);

            for (int j = 0; j != BLOCK_LEN_INT64; ++j) {
                dst[offset2 + word2 + j] = dst[offset1 + word1 + j] ^ state[j];
            }

            word1 += BLOCK_LEN_INT64;
            word2 -= BLOCK_LEN_INT64;
        }
    }

    /**
     * Операция дублирования
     * Все смещения указывают на матрицу dst и обозначают начало некоторых строк байт
     * All of the offsets point into {@code dst} and denote a start of some *row* of bytes.
     * dst     матрица, которая предоставляет и получает байты
     * offset0 строка, которая предоставляет и получает байты
     * offset1 строка, которая предоставляет байты (последняя проинициализированная строка)
     * offset2 строка, которая предоставляет байты (последняя повторно посещенная и обновленная строка)
     * offset3 строка,которая получает байты
     */
    public void reduced_duplex_row_filling(long dst[], final int offset0, final int offset1, final int offset2, final int offset3) {
        int word0 = offset0;
        int word1 = offset1;
        int word2 = offset2;
        int word3 = offset3 + (N_COLS - 1) * BLOCK_LEN_INT64;

        for (int i = 0; i != N_COLS; ++i) {
            for (int j = 0; j != BLOCK_LEN_INT64; ++j) {
                state[j] ^= mem.flip(
                      mem.flip(dst[word0 + j])
                    + mem.flip(dst[word1 + j])
                    + mem.flip(dst[word2 + j])
                );
            }

            sponge_lyra(HALF_ROUNDS);

            for (int j = 0; j != BLOCK_LEN_INT64; ++j) {
                dst[word3 + j] = dst[word1 + j] ^ state[j];
            }

            for (int j = 0; j != BLOCK_LEN_INT64 - 2; ++j) {
                dst[word0 + j] ^= state[j + 2];
            }
            dst[word0 + BLOCK_LEN_INT64 - 2] ^= state[0];
            dst[word0 + BLOCK_LEN_INT64 - 1] ^= state[1];

            word0 += BLOCK_LEN_INT64;
            word1 += BLOCK_LEN_INT64;
            word2 += BLOCK_LEN_INT64;
            word3 -= BLOCK_LEN_INT64;
        }
    }

    /**
     * Операция дублирования
     * Все смещения указывают на dst и обозначают начало некоторой строки байт
     * dst     матрица, котоаря предоставляет и получает байты
     * offset0 строка, которая предоставляет  и получает байты
     * offset1 строка, котоаря предоставляет и получает байты после сдвига
     * offset2 строка, которая предоставляет байты
     * offset3 строка, которая предоставляет байты
     */
    public void reduced_duplex_row_wandering(long[] dst, final int offset0, final int offset1, final int offset2, final int offset3) {
        int word0 = offset0;
        int word1 = offset1;

        for (int i = 0; i != N_COLS; ++i) {
            final int rndcol0 = (int) Long.remainderUnsigned(mem.flip(state[4]), N_COLS) * BLOCK_LEN_INT64;
            final int rndcol1 = (int) Long.remainderUnsigned(mem.flip(state[6]), N_COLS) * BLOCK_LEN_INT64;

            final int word2 = offset2 + rndcol0;
            final int word3 = offset3 + rndcol1;

            for (int j = 0; j != BLOCK_LEN_INT64; ++j) {
                state[j] ^= mem.flip(
                      mem.flip(dst[word0 + j])
                    + mem.flip(dst[word1 + j])
                    + mem.flip(dst[word2 + j])
                    + mem.flip(dst[word3 + j])
                );
            }

            sponge_lyra(HALF_ROUNDS);

            for (int j = 0; j != BLOCK_LEN_INT64; ++j) {
                dst[word0 + j] ^= state[j];
            }

            for (int j = 0; j != BLOCK_LEN_INT64 - 2; ++j) {
                dst[word1 + j] ^= state[j + 2];
            }
            dst[word1 + BLOCK_LEN_INT64 - 2] ^= state[0];
            dst[word1 + BLOCK_LEN_INT64 - 1] ^= state[1];

            word0 += BLOCK_LEN_INT64;
            word1 += BLOCK_LEN_INT64;
        }
    }
}
