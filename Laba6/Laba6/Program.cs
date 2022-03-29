using System.Diagnostics;
using System.Text;

public class LCG
{
    private int a;
    private int c;
    private int n;
    private int currentNumber;


    public LCG(int a, int c, int n)
    {
        this.a = a;
        this.c = c;  
        this.n = n;
        currentNumber = 0;
    }

    public int GenerateNumber()
    {
        return currentNumber = (currentNumber * a + c) % n;
    }
}

public class RC4
{
    private byte n;
    private byte[] key;
    private byte[] S;

    int i = 0;
    int j = 0;
    int t;
    int K;

    public RC4(byte n, byte key)
    {
        this.n = n;
        this.key = BitConverter.GetBytes(key);
        S = new byte[(int)Math.Pow(2, n)];
        InitializeSBlock();

    }

    public void InitializeSBlock()
    {
        for (int i = 0; i < Math.Pow(2, n); i++)
        {
            S[i] = (byte)i;
        }
        int j = 0;
        for (int i = 0; i < Math.Pow(2, n); i++)
        {
            j = (int)((j + S[i] + key[i % key.Length]) % Math.Pow(2, n));
            Swap(S, i, j);
        }
    }

    private int PRGA()
    {
        i = (int)((i + 1) % Math.Pow(2, n));
        j = (int)((j + S[i]) % Math.Pow(2, n));
        Swap(S, i, j);
        t = (int)((S[i] + S[j]) % Math.Pow(2, n));
        return S[t];
    }
    public void Swap(byte[] array, int index1, int index2)
    {
        byte temp = array[index1];
        array[index1] = array[index2];
        array[index2] = temp;
    }

    public byte[] Encryption(byte[] text, int size)
    {
        byte[] data = text.Take(size).ToArray();

        byte[] cipher = new byte[data.Length];

        for (int m = 0; m < data.Length; m++)
        {
            cipher[m] = (byte)(data[m] ^ PRGA());
        }

        return cipher;

    }

    public byte[] Decryption(byte[] text, int size)
    {
        return Encryption(text, size);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Последовательность псевдослучайных чисел для a=421, c=1663, n=7875:");
        LCG generator = new LCG(a: 421, c: 1663, n: 7875);
        for(int i = 0; i < 20; i++)
        {
            Console.Write(generator.GenerateNumber() + "; ");
        }
        List<byte> keys = new List<byte>();
        keys.Add(13);
        keys.Add(19);
        keys.Add(90);
        keys.Add(92);
        keys.Add(240);

        Console.WriteLine("\nВведите строку для шифрования: ");
        string testString = Console.ReadLine();
        Stopwatch sw = new Stopwatch();
        foreach(byte key in keys)
        {
            RC4 rc4En = new RC4(8, key);

            sw.Restart();
            byte[] testBytes = ASCIIEncoding.ASCII.GetBytes(testString);
            byte[] result = rc4En.Encryption(testBytes, testBytes.Length);
            sw.Stop();

            Console.WriteLine("Зашифрованное сообщение: " + ASCIIEncoding.ASCII.GetString(result));
            Console.WriteLine($"Время, потребовавшееся для зашифрования сообщения: {sw.ElapsedMilliseconds} мс\n");

            RC4 rc4De = new RC4(8, key);

            sw.Restart();
            byte[] decryptedBytes = rc4De.Decryption(result, result.Length);
            string decryptedString = ASCIIEncoding.ASCII.GetString(decryptedBytes);
            sw.Stop();

            Console.WriteLine("Расшифрованное сообщение: " + decryptedString);
            Console.WriteLine($"Время, потребовавшееся для расшифрования сообщения: {sw.ElapsedMilliseconds} мс\n");

        }
    }
}
