using System.Diagnostics;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        byte[] decrypted;
        Knapsack knapsack;
        Console.WriteLine("Введите слово для зашифровки:");
        string message = Console.ReadLine();
        byte[] binary = new byte[message.Length];
        Console.WriteLine("\nВыберите кодировку: 1 - Base64; 2 - ASCII");
        int choice;
        Stopwatch sw = new Stopwatch();
        switch (int.Parse(Console.ReadLine()))
        {
            case 1:
                sw.Restart();
                knapsack = new Knapsack(6);
                byte[] bin = ASCIIEncoding.ASCII.GetBytes(message);
                string base64 = Convert.ToBase64String(bin);
                Console.WriteLine("\nСообщение после кодировки base64:");

                Console.WriteLine(base64);
                knapsack.Encrypt(Base64.Get6BitsBlock(base64));
                sw.Stop();
                Console.WriteLine($"\nВремя на зашифрование: {sw.ElapsedMilliseconds} мс");
                sw.Restart();
                decrypted = knapsack.Decrypt();
                base64 = Base64.GetTextFrom6BitsBlocks(decrypted);
                while(base64.Length * 6 % 8 != 0)
                {
                    base64 += new string('=', 1);
                }
                decrypted = Convert.FromBase64String(base64);
                base64 = Encoding.UTF8.GetString(decrypted);
                Console.WriteLine("Расшифрованное сообщение:");
                Console.WriteLine(base64);
                sw.Stop();
                Console.WriteLine($"\nВремя на расшифрование: {sw.ElapsedMilliseconds} мс");

                break;
            case 2:
                sw.Restart();
                knapsack = new Knapsack(8);
                for (int i = 0; i < binary.Length; i++)
                {
                    binary[i] = (byte)message[i];
                }
                knapsack.Encrypt(binary);
                sw.Stop();
                Console.WriteLine($"\nВремя на зашифрование: {sw.ElapsedMilliseconds} мс");
                sw.Restart();
                decrypted = knapsack.Decrypt();
                Console.WriteLine("Расшифрованное сообщение:");
                for (int i = 0; i < decrypted.Length; i++)
                {
                    Console.Write((char)decrypted[i]);
                }
                sw.Stop();
                Console.WriteLine($"\nВремя на расшифрование: {sw.ElapsedMilliseconds} мс");
                break;
            default:
                sw.Restart();
                knapsack = new Knapsack(8);
                for (int i = 0; i < binary.Length; i++)
                {
                    binary[i] = (byte)message[i];
                }

                knapsack.Encrypt(binary);
                sw.Stop();
                Console.WriteLine($"\nВремя на зашифрование: {sw.ElapsedMilliseconds} мс");

                decrypted = knapsack.Decrypt();
                sw.Restart();
                Console.WriteLine("Расшифрованное сообщение:");
                for (int i = 0; i < decrypted.Length; i++)
                {
                    Console.Write((char)decrypted[i]);
                }
                sw.Stop();
                Console.WriteLine($"\nВремя на расшифрование: {sw.ElapsedMilliseconds} мс");

                break;
        }
    }
}

class Base64
{
    private static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    public static byte[] Get6BitsBlock(string text) // getting 6-bits block from base64 encoded message via base64 alphabet
    {
        if (text.Contains('='))
        {
            text = text.Remove(text.IndexOf('='));
        }
        byte[] result = new byte[text.Length];
        for (int i = 0; i < text.Length; i++)
        {
            result[i] = (byte)alphabet.IndexOf(text[i]);
        }
        return result;
    }

    public static string GetTextFrom6BitsBlocks(byte[] bin)
    {
        string result = "";
        for (int i = 0;i < bin.Length; i++)
        {
            result += alphabet[bin[i]];
        }
        return result;
    }
}
class Knapsack
{
    private List<double> sequence;
    private List<double> openKey;
    private List<double> encryptedMessage;
    private List<double> decryptedMessage;
    private int z;
    private int a;
    private int n;
    public Knapsack(int z)
    {
        sequence = new List<double>();
        openKey = new List<double>();
        encryptedMessage = new List<double>();
        decryptedMessage = new List<double>();
        this.z = z;
        GenerateSequence();
    }
    private void GenerateSequence() // generate superincreasing sequence (every element is more than sum of all previous ones)
    {
        Random random = new Random();
        int maxValue = (random.Next((int)Math.Pow(2, 14), (int)(Math.Pow(2, 15))));
        int delta = 200;
        sequence.Add(random.Next(1, (int)(maxValue / Math.Pow(2, z))));
        Console.WriteLine("Члены сверхвозрастающей последовательности (закрытый ключ):");
        for(int i = 1; i < z; i++)
        {
            Console.Write(sequence.Last());
            Console.Write(' ');

            sequence.Add((2 * sequence.Last()) + random.Next(0, (int)delta));
        }
        Console.WriteLine(sequence.Last());

    }

    private void GenerateOpenKey() // generating open key: i * a mod n
    {
        int x, y;
        Random random = new Random();
        Console.Write("\nСумма членов сверхвозрастающей последовательности: ");
        Console.WriteLine((int)sequence.Sum());
        n = random.Next((int)sequence.Sum(), (int)sequence.Sum() + 15);
        do
        {
            a = random.Next(2, n);

        } while (Gcd(a, n, out x, out y) != 1);
        Console.WriteLine("\n\nОткрытый ключ:");
        foreach(int i in sequence)
        {
            openKey.Add(i * a % n);
            Console.Write(i * a % n);
            Console.Write(' ');
        }
    }

    public void Encrypt(byte[] binary) // encryption of english word in both formats
    {
        GenerateOpenKey();
        string bin;
        double sum = 0;
        for (int i = 0; i < binary.Length; i++)
        {
            bin = Convert.ToString(binary[i], 2);
            bin = new string('0', z - bin.Length) + bin;

            for (int j = 0; j < z; j++)
            {
                if (bin[j] == '1')
                {
                    sum += openKey[j];
                }
            }
            encryptedMessage.Add(sum);
            sum = 0;
        }
        Console.WriteLine("\n\nЗашифрованное сообщение: ");
        foreach (int i in encryptedMessage)
        {
            Console.Write(i + " ");
        }

    }

    public static int Gcd(int a, int b, out int x, out int y) // greatest common divider, extended Euclid's algorithm
    {
        if (b < a)
        {
            a ^= b;
            b ^= a;
            a ^= b;
        }

        if (a == 0)
        {
            x = 0;
            y = 1;
            return b;
        }

        int gcd = Gcd(b % a, a, out x, out y);

        int newY = x;
        int newX = y - (b / a) * x;

        x = newX;
        y = newY;
        return gcd;
    }


    public byte[] Decrypt()
    {
        Console.WriteLine();
        byte[] decrypted = new byte[encryptedMessage.Count];
        int x, y;
        Gcd(a, n, out x, out y);
        string message = "";
        x = x < 0 ? n + x : x;
        foreach(int i in encryptedMessage)
        {
            decryptedMessage.Add(i * x % n);
        }
        sequence.Reverse();
        for(int i = 0; i < encryptedMessage.Count; i++)
        {
            for (int j = 0; j < z; j++)
            {
                if (sequence[j] <= decryptedMessage[i])
                {
                    message += "1";
                    decryptedMessage[i] -= sequence[j];
                }
                else
                {
                    message += "0";
                }
            }
            message = new string(message.Reverse().ToArray());            
            decrypted[i] = (byte)Convert.ToInt32(message, 2);
            message = "";
        }
        return decrypted;
    }
}