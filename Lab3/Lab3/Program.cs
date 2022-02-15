using System.Collections;
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

        Decryption decryption = new Decryption(encryption.Route());
        decryption.Route();

        string name;
        string secondName;

        Console.Write("Введите, пожалуйста, ваше имя: ");
        name = Console.ReadLine();
        name = new string(name.Distinct().ToArray());

        Console.Write("Введите, пожалуйста, вашу фамилию: ");
        secondName = Console.ReadLine();
        secondName = new string(secondName.Distinct().ToArray());
        
        decryption.RouteMatrix = encryption.Multiple(name, secondName);
        decryption.Multiple(name, secondName);

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
        using(StreamReader sr = new StreamReader("input.txt"))
        {
            input = sr.ReadToEnd();
        }
        CreateMatrix(name, secondName);
        SortCols();
        SortRows();
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
    protected char[,] routeMatrix;

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
        Console.WriteLine("Отсортированные строки:");
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
        Console.WriteLine("Отсортированные строки (исходное расположение строк):");
        PrintMatrix();
    }
}