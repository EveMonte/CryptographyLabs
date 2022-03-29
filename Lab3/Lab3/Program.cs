using Aspose.Cells;
using Aspose.Cells.Charts;
using System.Collections;
using System.Diagnostics;

class Program
{
    public static void Main()
    {
        int cols;
        int rows;
        do
        {
            Console.Write("Введите число строк: ");
        }
        while (!int.TryParse(Console.ReadLine(), out rows));

        do
        {
            Console.Write("Введите число столбцов: ");
        }
        while (!int.TryParse(Console.ReadLine(), out cols));

        Encryption encryption = new Encryption(rows, cols);

        Stopwatch sw = new Stopwatch();
        sw.Start(); 
        Decryption decryption = new Decryption(encryption.Route());
        sw.Stop();
        Console.WriteLine($"Шифрование маршрутной перестановкой заняло {sw.ElapsedMilliseconds/1000.0f}с");
        sw.Restart();
        decryption.Route();
        sw.Stop();
        Console.WriteLine($"Дешифрование маршрутной перестановкой заняло {sw.ElapsedMilliseconds / 1000.0f}с");

        string name;
        string secondName;

        Console.Write("Введите, пожалуйста, ваше имя: ");
        name = Console.ReadLine();
        name = new string(name.ToLower().Distinct().ToArray());

        Console.Write("Введите, пожалуйста, вашу фамилию: ");
        secondName = Console.ReadLine();
        secondName = new string(secondName.ToLower().Distinct().ToArray());
        
        sw.Restart();
        decryption.RouteMatrix = encryption.Multiple(name, secondName);
        sw.Stop();
        Console.WriteLine($"Шифрование множественной перестановкой заняло {sw.ElapsedMilliseconds / 1000.0f}с");

        sw.Restart();
        decryption.Multiple(name, secondName);
        sw.Stop();
        Console.WriteLine($"Дешифрование множественной перестановкой заняло {sw.ElapsedMilliseconds / 1000.0f}с");


        encryption.Gistogram();
    }
}

class Encryption : Cipher
{
    private int rows;
    private int cols;
    public Encryption(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
    }
    public override char[,] Route()
    {
        int counter = 0;
        using(StreamReader sr = new StreamReader("input.txt"))
        {
            input = sr.ReadToEnd();
        }

        routeMatrix = new char[rows, cols];
        CreateMatrix(input);

        for(int j = 0; j < cols; j++)
        {
            for(int i = 0; i < rows; i++)
            {
                encodedText += routeMatrix[i, j];
            }
        }

        using(StreamWriter sw = new StreamWriter("outputRoute.txt"))
        {
            sw.WriteLine(encodedText);
        }

        return routeMatrix;
    }

    public override char[,] Multiple(string name, string secondName)
    {
        using(StreamReader sr = new StreamReader("input2.txt"))
        {
            input = sr.ReadToEnd();
        }
        CreateMatrix(name, secondName);
        SortCols();
        SortRows();
        for(int i = 2; i < routeMatrix.GetLength(0); i++)
        {
            for(int j = 2; j < routeMatrix.GetLength(1); j++)
            {
                encodedMultiple += routeMatrix[i, j];  
            }
        }
        using(StreamWriter sw = new StreamWriter("encodedMultiple.txt"))
        {
            sw.WriteLine(encodedMultiple);
        }
        return routeMatrix;
    }
}

class Decryption : Cipher
{

    public char[,] RouteMatrix
    {
        get { return routeMatrix; }
        set { routeMatrix = value; }
    }


    public Decryption(char[,] matrix)
    {
        routeMatrix = matrix;
    }
    public override char[,] Route()
    {
        string decodedText = "";
        using (StreamReader sr = new StreamReader("outputRoute.txt"))
        {
            encodedText = sr.ReadToEnd();
        }

        CreateMatrix(encodedText);

        for (int j = 0; j < routeMatrix.GetLength(1); j++)
        {
            for (int i = 0; i < routeMatrix.GetLength(0); i++)
            {
                decodedText += routeMatrix[i, j];
            }
        }

        using (StreamWriter sw = new StreamWriter("decodedRoute.txt"))
        {
            sw.WriteLine(decodedText);
        }

        return null;
    }

    public override char[,] Multiple(string name, string secondName)
    {
        SortCols(secondName);
        SortRows(name);
        
        for (int i = 2; i < routeMatrix.GetLength(0); i++)
        {
            for (int j = 2; j < routeMatrix.GetLength(1); j++)
            {
                decodedText += routeMatrix[i, j];
            }
        }
        using(StreamWriter sw = new StreamWriter("decodedMultiple.txt"))
        {
            sw.WriteLine(decodedText);
        }

        return null;
    }
}

abstract class Cipher
{
    public abstract char[,] Route();
    public abstract char[,] Multiple(string name, string secondName);
    protected string input = "";
    protected string encodedText = "";
    protected string decodedText = "";
    protected string encodedMultiple = "";
    protected char[,] routeMatrix;
    const string ALPHABET = "abcdefghijklmnopqrstuvwxyz?!.,- ";


    public int CountLetters(string text)
    {
        int count = 0;
        for (int i = 0; i < ALPHABET.Length; i++)
        {
            count += text.Count(n => n == ALPHABET[i]);
        }

        return count;

    }

    public void Gistogram()
    {
        Workbook workbook = new Workbook();
        Worksheet worksheet = workbook.Worksheets[0];
        float countRoute = CountLetters(encodedText);
        float countBefore = CountLetters(input);
        float countMultiple = CountLetters(encodedMultiple);
        char cellCoord = 'B';
        string coords = "";
        string lastCoord = "";
        worksheet.Cells["A1"].PutValue("Символы");
        worksheet.Cells["A2"].PutValue("Частота появления символов исходного сообщения");
        worksheet.Cells["A3"].PutValue("Частота появления символов зашифрованного сообщения (маршрутная перестановка)");
        worksheet.Cells["A4"].PutValue("Частота появления символов зашифрованного сообщения (множественная перестановка)");
        bool isOutOfRange = false;

        for (int i = 1; i <= ALPHABET.Length; i++)
        {

            if (i >= 26 && !isOutOfRange)
            {
                cellCoord = 'A';
                isOutOfRange = true;
            }
            coords = cellCoord.ToString() + 1;
            if (isOutOfRange)
            {
                coords = coords.Insert(0, "A");
            }

            worksheet.Cells[coords].PutValue(ALPHABET[i - 1].ToString());
            cellCoord = (char)(cellCoord + 1);
            lastCoord = coords.Substring(0,2);
        }

        cellCoord = 'B';
        isOutOfRange = false;
        for (int i = 1; i <= ALPHABET.Length; i++)
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


            worksheet.Cells[coords + 2].PutValue(input.Count(n => n == ALPHABET[i - 1]) / countBefore);
            worksheet.Cells[coords + 3].PutValue(encodedText.Count(n => n == ALPHABET[i - 1]) / countRoute);
            worksheet.Cells[coords + 4].PutValue(encodedMultiple.Count(n => n == ALPHABET[i - 1]) / countMultiple);
            cellCoord = (char)(cellCoord + 1);
        }
        cellCoord = 'B';

        worksheet.AutoFitColumn(0, 0, 4);

        int chartIndex = worksheet.Charts.Add(ChartType.Column, 5, 1, 29, 15);
        int chartIndexPorta = worksheet.Charts.Add(ChartType.Column, 5, 1, 29, 15);

        Chart chart = worksheet.Charts[chartIndex];
        Chart chartPorta = worksheet.Charts[chartIndexPorta];

        chart.NSeries.Add("A2:" + lastCoord + "2", false);
        chart.NSeries.Add("A3:" + lastCoord + "3", false);
        chartPorta.NSeries.Add("A2:" + lastCoord + "2", false);
        chartPorta.NSeries.Add("A4:" + lastCoord + "4", false);

        chart.NSeries.CategoryData = "A1:" + lastCoord + "1";
        chartPorta.NSeries.CategoryData = "A1:" + lastCoord + "1";

        chart.NSeries[0].Name = "Вероятности до шифрования";
        chartPorta.NSeries[0].Name = "Вероятности до шифрования";
        chart.NSeries[1].Name = "Вероятности после шифрования";
        chartPorta.NSeries[1].Name = "Вероятности после шифрования";
        chart.Title.Text = "Вероятности появления символов при шифровании Маршрутной перестановкой";
        chart.Title.Font.Size = 14;
        chartPorta.Title.Text = "Вероятности появления символов при шифровании Множественной перестановкой";
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


    protected void PrintMatrix()
    {
        for (int i = 0; i < routeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < routeMatrix.GetLength(1); j++)
            {
                Console.Write(routeMatrix[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }

    protected void CreateMatrix(string source)
    {
        int counter = 0;
        for (int i = 0; i < routeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < routeMatrix.GetLength(1); j++)
            {
                routeMatrix[i, j] = source[counter];
                counter++;
            }
        }
    }

    protected void CreateMatrix(string name, string secondName)
    {
        routeMatrix = new char[name.Length + 2, secondName.Length + 2];
        int counter = 0;
        int number;
        Dictionary<char, int> keyValuePairs = new Dictionary<char, int>();
        for(char ch = 'a'; ch != 'A'; ch++)
        {
            if (name.Contains(ch))
            {
                keyValuePairs.Add(ch, counter);
                counter++;
            }
        }
        counter = 0;
        for(int i = 2;i < routeMatrix.GetLength(0); i++)
        {
            keyValuePairs.TryGetValue(name[counter], out number);
            routeMatrix[i, 0] = name[counter++];
            routeMatrix[i, 1] = char.Parse(number.ToString());
        }

        keyValuePairs.Clear();
        counter = 0;
        for (char ch = 'a'; ch != 'A'; ch++)
        {
            if (secondName.Contains(ch))
            {
                keyValuePairs.Add(ch, counter);
                counter++;
            }
        }
        counter = 0;
        for (int i = 2; i < routeMatrix.GetLength(1); i++)
        {
            keyValuePairs.TryGetValue(secondName[counter], out number);
            routeMatrix[0, i] = secondName[counter++];
            routeMatrix[1, i] = char.Parse(number.ToString());
        }

        counter = 0;
        for (int i = 2; i < routeMatrix.GetLength(0); i++)
        {
            for(int j = 2;j < routeMatrix.GetLength(1); j++)
            {
                routeMatrix[i, j] = input[counter++];
            }
        }
        PrintMatrix();
    }

    protected void SortCols()
    {
        char[] temp = new char[routeMatrix.GetLength(0)];
        int currentOrder = 0;
        for(int j = 2; j < routeMatrix.GetLength(1); j++)
        {
            for (int k = 2; k < routeMatrix.GetLength(1); k++)
            {
                if (routeMatrix[1, k] == char.Parse((j - 2).ToString()))
                {
                    currentOrder = int.Parse((k - 2).ToString());
                    break;
                }
            }
            if (currentOrder != j - 2) 
            {
                for (int i = 0; i < routeMatrix.GetLength(0); i++)
                {
                    temp[i] = routeMatrix[i, currentOrder + 2];
                    routeMatrix[i, currentOrder + 2] = routeMatrix[i, j];
                    routeMatrix[i, j] = temp[i];
                } 
            }
        }
        Console.WriteLine("\nОтсортированные столбцы:");
        PrintMatrix();
    }

    protected void SortCols(string columnsKey)
    {
        char[] temp = new char[routeMatrix.GetLength(0)];
        int currentOrder = 0;

        for (int j = 2; j < routeMatrix.GetLength(1); j++)
        {
            if (routeMatrix[0, j] != columnsKey[j - 2])
            {
                for(int k = 2; k < routeMatrix.GetLength(1); k++)
                {
                    if(routeMatrix[0, k] == columnsKey[j - 2])
                    {
                        currentOrder = int.Parse(routeMatrix[1, k].ToString());
                        routeMatrix[1, k] = char.Parse((j - 2).ToString());
                        routeMatrix[1, j] = char.Parse(currentOrder.ToString());
                        break;
                    }
                }

                for (int i = 0; i < routeMatrix.GetLength(0); i++)
                {
                    temp[i] = routeMatrix[i, currentOrder + 2];
                    routeMatrix[i, currentOrder + 2] = routeMatrix[i, j];
                    routeMatrix[i, j] = temp[i];
                }
            }
        }
        Console.WriteLine("\nОтсортированные столбцы (исходное расположение стобцов):");
        PrintMatrix();
    }


    protected void SortRows()
    {
        char[] temp = new char[routeMatrix.GetLength(1)];
        int currentOrder = 0;
        for (int i = 2; i < routeMatrix.GetLength(0); i++)
        {
            for (int k = 2; k < routeMatrix.GetLength(1); k++)
            {
                if (routeMatrix[k, 1] == char.Parse((i - 2).ToString()))
                {
                    currentOrder = int.Parse((k - 2).ToString());
                    break;
                }
            }

            if (currentOrder != i - 2)
            {
                for (int j = 0; j < routeMatrix.GetLength(1); j++)
                {
                    temp[j] = routeMatrix[currentOrder + 2, j];
                    routeMatrix[currentOrder + 2, j] = routeMatrix[i, j];
                    routeMatrix[i, j] = temp[j];
                }
            }
        }
        Console.WriteLine("\nОтсортированные строки:");
        PrintMatrix();
    }

    protected void SortRows(string rowsKey)
    {
        char[] temp = new char[routeMatrix.GetLength(1)];
        int currentOrder = 0;
        int counter = 0;
        for (int i = 2; i < routeMatrix.GetLength(0); i++)
        {
            if (routeMatrix[i, 0] != rowsKey[i - 2])
            {
                for (int k = 2; k < routeMatrix.GetLength(0); k++)
                {
                    if (routeMatrix[k, 0] == rowsKey[i - 2])
                    {
                        currentOrder = int.Parse(routeMatrix[k, 1].ToString());
                        routeMatrix[k, 1] = char.Parse((i - 2).ToString());
                        routeMatrix[i, 1] = char.Parse(currentOrder.ToString());
                        break;
                    }
                }
                for (int j = 0; j < routeMatrix.GetLength(1); j++)
                {
                    temp[j] = routeMatrix[currentOrder + 2, j];
                    routeMatrix[currentOrder + 2, j] = routeMatrix[i, j];
                    routeMatrix[i, j] = temp[j];
                }
            }
        }
        Console.WriteLine("\nОтсортированные строки (исходное расположение строк):");
        PrintMatrix();
    }
}