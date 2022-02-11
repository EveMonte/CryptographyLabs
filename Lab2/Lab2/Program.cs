using Aspose.Cells;
using Aspose.Cells.Charts;
using System.Diagnostics;
using System.Linq;

class Program
{
    public static void Main()
    {
        Decryption decryption = new Decryption();
        Encryption encryption = new Encryption();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        encryption.Caesar();
        sw.Stop();
        Console.WriteLine($"Время, затраченное на зашифрование шифром Цезаря с системой аффинной подстановкой: {sw.ElapsedMilliseconds/1000.0f}с");
        sw.Restart();
        decryption.Caesar();
        sw.Stop();
        Console.WriteLine($"Время, затраченное на расшифрование шифром Цезаря с системой аффинной подстановкой: {sw.ElapsedMilliseconds/1000.0f}с");
        sw.Restart();
        encryption.Porta();
        sw.Stop();
        Console.WriteLine($"Время, затраченное на зашифрование алгоритмом Порта: {sw.ElapsedMilliseconds/1000.0f}с");
        encryption.Gistogram();
        sw.Restart();
        decryption.Porta();
        sw.Stop();
        Console.WriteLine($"Время, затраченное на расшифрование алгоритмом Порта: {sw.ElapsedMilliseconds/1000.0f}с");

    }
}
class Encryption : Base
{

    public override void Caesar()
    {
        string text;
        string alphabet;
        using (StreamReader sr = new StreamReader("inputEng.txt"))
        {
            text = sr.ReadToEnd().ToLower();
        }
        using (StreamReader sr = new StreamReader("PolishAlphabet.txt"))
        {
            alphabet = sr.ReadToEnd();
        }
        int index = 0;
        foreach (char c in text)
        {
            if (alphabet.Contains(text[index]))
            {
                text = text.Remove(index, 1).Insert(index, alphabet[((alphabet.IndexOf(c)) * a + b) % alphabet.Length].ToString());
            }
            index++;
        }
        using (StreamWriter sw = new StreamWriter("outputCaesarEnryption.txt"))
        {
            sw.WriteLine(text);
        }
        textCaesar = text;
    }

    public override void Porta()
    {
        textPorta = "";
        using (StreamReader sr = new StreamReader("inputEng.txt"))
        {
            textBeforeEncryption = sr.ReadToEnd().ToLower();
        }
        using (StreamReader sr = new StreamReader("PolishAlphabet.txt"))
        {
            alphabet = sr.ReadToEnd();
        }
        textBeforeEncryption = textBeforeEncryption.Replace(" ", "");
        foreach (char c in textBeforeEncryption)
        {
            if(!alphabet.Contains(c))
                textBeforeEncryption = textBeforeEncryption.Replace(c.ToString(), "");
        }
        while(textBeforeEncryption.Length % 4 != 0)
        {
            textBeforeEncryption += "a";
        }
        for(int i = 0; i < textBeforeEncryption.Length; i += 2)
        {
            if (!alphabet.Contains(textBeforeEncryption[i]))
            {
                textBeforeEncryption.Remove(i, 1).Insert(i, alphabet[0].ToString());
            }

            if (!alphabet.Contains(textBeforeEncryption[i + 1]))
            {
                textBeforeEncryption.Remove(i + 1, 1).Insert(i + 1, alphabet[0].ToString());
            }

            textPorta += (alphabet.IndexOf(textBeforeEncryption[i + 1]) + alphabet.Length * alphabet.IndexOf(textBeforeEncryption[i])).ToString("D3");
        }
        textPorta = textPorta.Replace("-", "");

        using (StreamWriter sw = new StreamWriter("outputPortaEnryption.txt"))
        {
            sw.WriteLine(textPorta);
        }

    }

    public void Gistogram()
    {
        Workbook workbook = new Workbook();
        Worksheet worksheet = workbook.Worksheets[0];
        float countCaesar = CountLetters(textCaesar);
        float countBefore = CountLetters(textBeforeEncryption);
        float countPorta = CountLetters(textPorta);
        char cellCoord = 'B';
        string coords = "";
        string lastCoord = "";
        worksheet.Cells["A1"].PutValue("Символы");
        worksheet.Cells["A2"].PutValue("Частота появления символов исходного сообщения");
        worksheet.Cells["A3"].PutValue("Частота появления символов зашифрованного сообщения");
        worksheet.Cells["A32"].PutValue("Символы");
        worksheet.Cells["A33"].PutValue("Частота появления символов зашифрованного сообщения");
        bool isOutOfRange = false;

        for (int i = 1; i <= alphabet.Length; i++)
        {

            if(i >= 26 && !isOutOfRange)
            {
                cellCoord = 'A';
                isOutOfRange = true;
            }
            coords = cellCoord.ToString() + 1;
            if (isOutOfRange)
            {
                coords = coords.Insert(0, "A");
            }

            worksheet.Cells[coords].PutValue(alphabet[i-1].ToString());
            cellCoord = (char)(cellCoord + 1);
            lastCoord = coords.Substring(0, 2);
        }
        
        cellCoord = 'B';
        isOutOfRange = false;
        for (int i = 1; i <= alphabet.Length; i++)
        {
            if (i >= 26 && !isOutOfRange)
            {
                cellCoord = 'A';
                isOutOfRange = true;
            }

            coords = cellCoord.ToString();
            if (isOutOfRange)
            {
                coords = coords.Insert(0, "A");
            }


            worksheet.Cells[coords + 2].PutValue(textBeforeEncryption.Count(n => n == alphabet[i-1]) / countBefore);
            worksheet.Cells[coords + 3].PutValue(textCaesar.Count(n => n == alphabet[i-1]) / countCaesar);
            cellCoord = (char)(cellCoord + 1);
        }
        cellCoord = 'B';

        for (int i = 0; i < 10; i++)
        {
            coords = cellCoord.ToString();

            worksheet.Cells[coords+32].PutValue(i);
            worksheet.Cells[coords+33].PutValue(textPorta.Count(n => n.ToString() == i.ToString()) / (float)textPorta.Length);
            cellCoord = (char)(cellCoord + 1);

        }

        worksheet.AutoFitColumn(0, 0, 4);

        int chartIndex = worksheet.Charts.Add(ChartType.Column, 5, 1, 29, 15);
        int chartIndexPorta = worksheet.Charts.Add(ChartType.Column, 5, 1, 29, 15);

        Chart chart = worksheet.Charts[chartIndex];
        Chart chartPorta = worksheet.Charts[chartIndexPorta];
        chart.NSeries.Add("A2:" + lastCoord + "2", false);
        chartPorta.NSeries.Add("A33:K33", false);
        chart.NSeries.CategoryData = "A1:" + lastCoord + "1";
        chartPorta.NSeries.CategoryData = "A32:K32";
        chart.NSeries.Add("A3:" + lastCoord + "3", false);
        chart.NSeries.CategoryData = "A1:Z1";
        chart.NSeries[0].Name = "Вероятности до шифрования";
        chartPorta.NSeries[0].Name = "Вероятности до шифрования";
        chart.NSeries[1].Name = "Вероятности после шифрования";
        chart.Title.Text = "Вероятности появления символов при шифровании Цезаря с системой аффинных подстановок";
        chart.Title.Font.Size = 14;
        chartPorta.Title.Text = "Вероятности появления символов при шифровании алгоритмом Порта";
        chartPorta.Title.Font.Size = 14;
        chartPorta.ChartObject.Y = chart.ChartObject.Y;
        chartPorta.ChartObject.X = chart.ChartObject.X + chart.ChartObject.Width;

        workbook.Save("Charts.xls");

        new Process
        {
            StartInfo = new ProcessStartInfo("Charts.xls")
            {
                UseShellExecute = true
            }
        }.Start();

    }
    public int CountLetters(string input)
    {
        int count = 0;
        for (int i = 0; i < alphabet.Length; i++)
        {
            count += input.Count(n => n == alphabet[i]);
        }
        return count;
    }

}

class Euclid
{
    public static int Gcd(int a, int b, out int x, out int y)
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
}

class Decryption : Base
{

    public override void Caesar()
    {
        int c, d, nod;
        nod = Euclid.Gcd(a, 26, out c, out d);
        float reciprocal = c / nod;
        using (StreamReader sr = new StreamReader("outputCaesarEnryption.txt"))
        {
            textCaesar = sr.ReadToEnd().ToLower();
        }
        using (StreamReader sr = new StreamReader("PolishAlphabet.txt"))
        {
            alphabet = sr.ReadToEnd();
        }
        int index = 0;
        foreach (char ch in textCaesar)
        {
            if (alphabet.Contains(textCaesar[index]))
            {
                textCaesar = textCaesar.Remove(index, 1).Insert(index, alphabet[(int)Math.Round(reciprocal * (alphabet.IndexOf(ch) + alphabet.Length - b)) % alphabet.Length].ToString());
            }
            index++;
        }
        using (StreamWriter sw = new StreamWriter("outputCaesarDecryption.txt"))
        {
            sw.WriteLine(textCaesar);
        }
    }

    public override void Porta()
    {
        using (StreamReader sr = new StreamReader("outputPortaEnryption.txt"))
        {
            textPorta = sr.ReadToEnd().ToLower();
        }
        using (StreamReader sr = new StreamReader("PolishAlphabet.txt"))
        {
            alphabet = sr.ReadToEnd();
        }
        string newString = "";
        for (int i = 0; i < textPorta.Length; i += 3)
        {
            if(textPorta.Length - i < 3)
            {
                break;
            }
            string tempStr = textPorta.Substring(i, 3);
            int tempInt = Convert.ToInt32(tempStr);
            int tempI = 0;
            int tempJ = 0;
            for(int j = 0; j < alphabet.Length; j++)
            {
                if(tempInt < alphabet.Length)
                {
                    tempJ = tempInt;
                    tempI = 0;
                    break;
                }
                if((tempInt - j) % alphabet.Length == 0)
                {
                    tempJ = j;
                    tempI = (tempInt - j) / alphabet.Length;
                    break;
                }
            }
            newString += alphabet[tempI].ToString() + alphabet[tempJ].ToString();
        }

        using (StreamWriter sw = new StreamWriter("outputPortaDecryption.txt"))
        {
            sw.WriteLine(newString);
        }
    }
}

abstract class Base
{
    protected string alphabet = "";
    protected string textCaesar = "";
    protected string textPorta = "";
    protected string textBeforeEncryption = "";


    protected static int a = 3;
    protected static int b = 7;
    abstract public void Caesar();
    abstract public void Porta();
}
