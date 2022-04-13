
using Laba8;

class Program
{
    public static void Main(string[] args)
    {
        RSA rsa = new RSA();
        rsa.Encrypt();
        rsa.Decrypt();

        ElGamal elGamal = new ElGamal();
        elGamal.Encrypt();
        elGamal.Decrypt();
    }
}
public interface Cipher
{
    public bool Encrypt();
    public bool Decrypt();
}

public class RSA : Cipher
{
    private long p;
    private long q;   
    private long n;
    private long e;
    private long d;
    private string originalTextFile = "text.txt";
    private string encryptedTextFile = "encryptedRSA.txt";
    private string decryptedTextFile = "decryptedRSA.txt";
    public RSA()
    {
        Random random = new Random();
        do
        {
            p = random.Next(10, 99);
        } while(!Helper.IsPrime(p));
        do
        {
            q = random.Next(10, 99);
        } while (!(p != q && Helper.IsPrime(q)));
        n = p * q;

        long gcd, y;
        do
        {
            e = random.Next(2, 100);
            gcd = Helper.gcd(e, Helper.EulersFunction(p, q), out y, out _);
            if (y < 0)
            {
                d = n + y;
            }
            else
            {
                d = y;
            }

        } while (gcd != 1 && !Helper.IsPrime(e));

        PrlongParameters();
    }

    private void PrlongParameters()
    {
        Console.WriteLine($"p = {p}; q = {q}; n = {n}; e = {e}; d = {d}");
    }

    public bool Decrypt()
    {
        string text;
        string decryptedText = "";
        using (StreamReader sr = new StreamReader(encryptedTextFile))
        {
            text = sr.ReadToEnd();
        }
        string[] enc = text.Split(' ');

        foreach (string letter in enc)
        {
            if (!letter.Contains("\r\n"))
            {
                decryptedText += (char)Helper.Reminder(long.Parse(letter), d, n);

            }
        }
        using (StreamWriter sw = new StreamWriter(decryptedTextFile))
        {
            sw.WriteLine(decryptedText);
        }
        return true;
    }

    public bool Encrypt()
    {
        string text;
        string encryptedText = "";
        using(StreamReader sr = new StreamReader(originalTextFile))
        {
            text = sr.ReadToEnd();
        }
        foreach(char c in text)
        {
            encryptedText += Helper.Reminder((long)c, e, n).ToString() + " ";
        }
        using(StreamWriter sw = new StreamWriter(encryptedTextFile))
        {
            sw.WriteLine(encryptedText);
        }

        return true;
    }
}

public class ElGamal : Cipher
{
    private long p;
    private long g;
    private long k;
    private long x;
    private long y;
    private long a;
    private long b;

    private string originalTextFile = "text.txt";
    private string decryptedTextFile = "decryptedElGamal.txt";
    private string encryptedTextFile = "encryptedElGamal.txt";
    public ElGamal()
    {
        Random random = new Random();
        do
        {
            p = random.Next(10, 99);
        } while (!Helper.IsPrime(p));

        g = Helper.GetPRoot(p);
        x = random.Next(2, 10);
        y = Helper.Reminder(g, x, p);
        PrlongParameters();
    }

    private void PrlongParameters()
    {
        Console.WriteLine($"p = {p}; g = {g}; x = {x}; y = {y}");
    }


    public bool Decrypt()
    {
        string text;
        long rev;
        string decryptedText = "";
        using (StreamReader sr = new StreamReader(encryptedTextFile))
        {
            text = sr.ReadToEnd();
        }
        string[] vs = text.Split(' ');
        for(long i = 0; i < vs.Length; i+=2)
        {
            if (!vs[i].Contains("\r\n") || !vs[i + 1].Contains("\r\n"))
            {
                a = long.Parse(vs[i]);
                b = long.Parse(vs[i + 1]);
                Helper.gcd((long)Math.Pow(a, x), p, out rev, out _);
                decryptedText += (char)(rev * b % p);
            }
        }
        using(StreamWriter streamWriter = new StreamWriter(decryptedTextFile))
        {
            streamWriter.WriteLine(decryptedText);
        }
        return true;

    }

    public bool Encrypt()
    {
        Random random = new Random();
        string text;
        string encryptedText = "";
        using(StreamReader sr = new StreamReader(originalTextFile))
        {
            text = sr.ReadToEnd();
        }
        foreach(char c in text)
        {
            k = random.Next(100);
            encryptedText += Helper.Reminder(g, k, p).ToString() + " " + (Helper.Reminder(g, k, p) * (long)c % p).ToString() + " ";
        }

        using(StreamWriter sw = new StreamWriter(encryptedTextFile))
        {
            sw.Write(encryptedText);
        }

        return true;
    }
}