package com.example.sha512;

/**
 * Хранит все параметры в одном месте
 */
public class LyraParams {
    /**
     * Длина результирующего хеша в байтах
     */
    public final int klen;

    /**
     * Затраты по времени [1, +Inf]
     * Приблизительно пропорционально числу операций во время стадии Wandering
     */
    public final int t_cost;

    /**
     * Затртаты по памяти
     * Приблизительно пропорциональны количеству итераций во время стадии Wandering
     * Приблизительно пропорционально размеру матрицы, используемой во всем процессе
     */
    public final int m_cost;

    /**
     * Количество столбцов в матрице
     * Каждый столбец содержит BLOCK_LEN_INT64 8-байтных блоков
     */
    public final int N_COLS;

    /**
     * Тип используемой губки: Blake2b, BlaMka или BlaMka с половиной раундов
     */
    public final String SPONGE;

    /**
     * Количество раундов во время стадии перестановок (обычно 12)
     */
    public final int FULL_ROUNDS;

    /**
     * Уменьшенное количество раундов во время перестановок (обычно 12)
     */
    public final int HALF_ROUNDS;

    /**
     * Размер одного блока в 8-байтной части
     */
    public final int BLOCK_LEN_INT64;

    /**
     * Размер одного блока в байтах, равный 8 * BLOCK_LEN_INT64
     */
    public final int BLOCK_LEN_BYTES;

    /**
     * Размер одной строки матрицы из 8-байтных частей
     */
    public final int ROW_LEN_INT64;

    /**
     * Размер одной строки матрицы в байтах
     */
    public final int ROW_LEN_BYTES;

    public final int SIZEOF_INT   = 4;
    public final int SIZEOF_INT64 = 8;

    public final int BLOCK_LEN_BLAKE2_SAFE_INT64 = 8;
    public final int BLOCK_LEN_BLAKE2_SAFE_BYTES = 64;

    public LyraParams(
            int klen, int t_cost, int m_cost,
            int N_COLS, String SPONGE,
            int FULL_ROUNDS, int HALF_ROUNDS, int BLOCK_LEN_INT64
    ) {
        this.klen = klen;

        this.t_cost = t_cost;
        this.m_cost = m_cost;

        this.N_COLS = N_COLS;

        this.SPONGE = SPONGE;
        this.FULL_ROUNDS = FULL_ROUNDS;
        this.HALF_ROUNDS = HALF_ROUNDS;

        this.BLOCK_LEN_INT64 =     BLOCK_LEN_INT64;
        this.BLOCK_LEN_BYTES = 8 * BLOCK_LEN_INT64;

        this.ROW_LEN_INT64 =     N_COLS * BLOCK_LEN_INT64;
        this.ROW_LEN_BYTES = 8 * N_COLS * BLOCK_LEN_INT64;
    }
}
