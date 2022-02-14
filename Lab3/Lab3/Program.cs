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


        encryption.Multiple(name, secondName);
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

    public override void Multiple(string name, string secondName)
    {
        using(StreamReader sr = new StreamReader("input.txt"))
        {
            input = sr.ReadToEnd();
        }
        CreateMatrix(name, secondName);
        SortCols();
        SortRows();
    }
}

class Decryption : Cipher
{

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

    public override void Multiple(string name, string secondName)
    {

    }
}

abstract class Cipher
{
    public abstract char[,] Route();
    public abstract void Multiple(string name, string secondName);
    protected string input = "";
    protected string encodedText = "";
    protected char[,] routeMatrix;

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

        for (int i = 0; i < routeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < routeMatrix.GetLength(1); j++)
            {
                Console.Write(routeMatrix[i,j] + "\t");
            }
            Console.WriteLine();
        }
    }

    protected void SortCols()
    {
        char[] temp = new char[routeMatrix.GetLength(0)];
        int currentOrder;
        int counter = 0;
        for(int j = 2; j < routeMatrix.GetLength(1); j++)
        {
            currentOrder = int.Parse(routeMatrix[1, j].ToString());
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
        Console.WriteLine("Отсортированные столбцы:");

        for (int i = 0; i < routeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < routeMatrix.GetLength(1); j++)
            {
                Console.Write(routeMatrix[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }

    protected void SortRows()
    {
        char[] temp = new char[routeMatrix.GetLength(1)];
        int currentOrder;
        int counter = 0;
        for (int i = 2; i < routeMatrix.GetLength(0); i++)
        {
            currentOrder = int.Parse(routeMatrix[i, 1].ToString());
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
        for (int i = 0; i < routeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < routeMatrix.GetLength(1); j++)
            {
                Console.Write(routeMatrix[i, j] + "\t");
            }
            Console.WriteLine();
        }

    }
}