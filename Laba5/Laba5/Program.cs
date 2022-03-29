using System.Diagnostics;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        string keyWord;
        Console.WriteLine("Введите ключ (8 символов): ");
        keyWord = Console.ReadLine();
        DES des = new DES(keyWord);
        Stopwatch watch = new Stopwatch();
        watch.Start();
        des.Encrypt();
        watch.Stop();
        Console.WriteLine($"Время для зашифрования: {watch.ElapsedMilliseconds} мс");
        watch.Restart();
        des.Decrypt();
        watch.Stop();
        Console.WriteLine($"Время для расшифрования: {watch.ElapsedMilliseconds} мс");

    }
}
public class DES
{
    private const int sizeOfBlock = 128; //в DES размер блока 64 бит, но поскольку в unicode символ в два раза длинее, то увеличим блок тоже в два раза
    private const int sizeOfChar = 16; //размер одного символа (in Unicode 16 bit)

    private const int shiftKey = 2; //сдвиг ключа 

    private const int quantityOfRounds = 16; //количество раундов

    private string encodedKeyWord;
    private string decodedKeyWord;

    string[] Blocks; //сами блоки в двоичном формате

    public DES(string keyWord)
    {
        this.encodedKeyWord = keyWord;
    }

    //зашифровать
    public void Encrypt()
    {
        if (encodedKeyWord.Length > 0)
        {
            string s = "";

            string key = encodedKeyWord;

            StreamReader sr = new StreamReader("in.txt");

            while (!sr.EndOfStream)
            {
                s += sr.ReadLine();
            }

            sr.Close();

            s = StringToRightLength(s);

            CutStringIntoBlocks(s);

            key = CorrectKeyWord(key, s.Length / (2 * Blocks.Length));
            key = StringToBinaryFormat(key);

            for (int j = 0; j < quantityOfRounds; j++)
            {
                for (int i = 0; i < Blocks.Length; i++)
                    Blocks[i] = EncodeDES_One_Round(Blocks[i], key);

                key = KeyToNextRound(key);
            }

            key = KeyToPrevRound(key);

            decodedKeyWord = StringFromBinaryToNormalFormat(key);

            string result = "";

            for (int i = 0; i < Blocks.Length; i++)
                result += Blocks[i];

            FileStream fs = new FileStream("out1.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);

            sw.WriteLine(StringFromBinaryToNormalFormat(result));
            sw.Close();

        }
    }

    //расшифровать
    public void Decrypt()
    {
        if (decodedKeyWord.Length > 0)
        {
            string s = "";

            string key = StringToBinaryFormat(decodedKeyWord);
            FileStream fs = new FileStream("out1.txt", FileMode.OpenOrCreate);

            StreamReader sr = new StreamReader(fs, Encoding.Unicode);

            while (!sr.EndOfStream)
            {
                s += sr.ReadLine();
            }

            sr.Close();

            s = StringToBinaryFormat(s);

            CutBinaryStringIntoBlocks(s);

            for (int j = 0; j < quantityOfRounds; j++)
            {
                for (int i = 0; i < Blocks.Length; i++)
                    Blocks[i] = DecodeDES_One_Round(Blocks[i], key);

                key = KeyToPrevRound(key);
            }

            key = KeyToNextRound(key);

            encodedKeyWord = StringFromBinaryToNormalFormat(key);

            string result = "";

            for (int i = 0; i < Blocks.Length; i++)
                result += Blocks[i];
            FileStream fs2 = new FileStream("out2.txt", FileMode.OpenOrCreate);

            StreamWriter sw = new StreamWriter(fs2, Encoding.Unicode);
            sw.WriteLine(StringFromBinaryToNormalFormat(result));
            sw.Close();

        }
    }

    //доводим строку до размера, чтобы делилась на sizeOfBlock
    private string StringToRightLength(string input)
    {
        while (((input.Length * sizeOfChar) % sizeOfBlock) != 0)
            input += "#";

        return input;
    }

    //разбиение обычной строки на блоки
    private void CutStringIntoBlocks(string input)
    {
        Blocks = new string[(input.Length * sizeOfChar) / sizeOfBlock];

        int lengthOfBlock = input.Length / Blocks.Length;

        for (int i = 0; i < Blocks.Length; i++)
        {
            Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
            Blocks[i] = StringToBinaryFormat(Blocks[i]);
        }
    }

    //разбиение двоичной строки на блоки
    private void CutBinaryStringIntoBlocks(string input)
    {
        Blocks = new string[input.Length / sizeOfBlock];

        int lengthOfBlock = input.Length / Blocks.Length;

        for (int i = 0; i < Blocks.Length; i++)
            Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
    }

    //перевод строки в двоичный формат
    private string StringToBinaryFormat(string input)
    {
        string output = "";

        for (int i = 0; i < input.Length; i++)
        {
            string char_binary = Convert.ToString(input[i], 2);

            while (char_binary.Length < sizeOfChar)
                char_binary = "0" + char_binary;

            output += char_binary;
        }

        return output;
    }

    //доводим длину ключа до нужной
    private string CorrectKeyWord(string input, int lengthKey)
    {
        if (input.Length > lengthKey)
            input = input.Substring(0, lengthKey);
        else
            while (input.Length < lengthKey)
                input = "0" + input;

        return input;
    }

    //шифрование DES один раунд
    private string EncodeDES_One_Round(string input, string key)
    {
        string L = input.Substring(0, input.Length / 2);
        string R = input.Substring(input.Length / 2, input.Length / 2);

        return (R + XOR(L, f(R, key)));
    }

    //расшифровка DES один раунд
    private string DecodeDES_One_Round(string input, string key)
    {
        string L = input.Substring(0, input.Length / 2);
        string R = input.Substring(input.Length / 2, input.Length / 2);

        return (XOR(f(L, key), R) + L);
    }

    //XOR двух строк с двоичными данными
    private string XOR(string s1, string s2)
    {
        string result = "";

        for (int i = 0; i < s1.Length; i++)
        {
            bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
            bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

            if (a ^ b)
                result += "1";
            else
                result += "0";
        }
        return result;
    }

    //шифрующая функция f. в данном случае это XOR
    private string f(string s1, string s2)
    {
        return XOR(s1, s2);
    }

    //вычисление ключа для следующего раунда шифрования. циклический сдвиг >> 2
    private string KeyToNextRound(string key)
    {
        for (int i = 0; i < shiftKey; i++)
        {
            key = key[key.Length - 1] + key;
            key = key.Remove(key.Length - 1);
        }

        return key;
    }

    //вычисление ключа для следующего раунда расшифровки. циклический сдвиг << 2
    private string KeyToPrevRound(string key)
    {
        for (int i = 0; i < shiftKey; i++)
        {
            key = key + key[0];
            key = key.Remove(0, 1);
        }

        return key;
    }

    //переводим строку с двоичными данными в символьный формат
    private string StringFromBinaryToNormalFormat(string input)
    {
        string output = "";

        while (input.Length > 0)
        {
            string char_binary = input.Substring(0, sizeOfChar);
            input = input.Remove(0, sizeOfChar);

            int a = 0;
            int degree = char_binary.Length - 1;

            foreach (char c in char_binary)
                a += Convert.ToInt32(c.ToString()) * (int)Math.Pow(2, degree--);

            output += ((char)a).ToString();
        }

        return output;
    }
}
