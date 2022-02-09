class Program
{
    public static void Main()
    {
        bool whileActive = true;
        int n = 1;
        int m;
        while (whileActive)
        {
            Console.WriteLine("Для скольких чисел считать НОД? (либо 2, либо 3, 0 - для выхода)");
            int key = int.Parse(Console.ReadLine());
            switch (key)
            {
                case 0: 
                    whileActive = false; 
                    break;
                case 2:
                    Console.Write("Введите n: ");
                    n = int.Parse(Console.ReadLine());
                    Console.Write("Введите m: ");
                    m = int.Parse(Console.ReadLine());

                    Console.WriteLine($"НОД двух чисел: {Calculations.NOD((uint)m, (uint)n)}");
                    break;
                case 3:
                    Console.Write("Введите n: ");
                    n = int.Parse(Console.ReadLine());
                    Console.Write("Введите m: ");
                    m = int.Parse(Console.ReadLine());
                    Console.Write("Введите l: ");
                    int l = int.Parse(Console.ReadLine());
                    Console.WriteLine($"НОД трех чисел: {Calculations.NOD((uint)m, (uint)n, (uint)l)}");
                    break;
                default:
                    Console.WriteLine("Введите корректное значение");
                    break;
            }
        }

        List<uint> numbers = Calculations.SieveEratosthenes((uint)n);
        Console.WriteLine($"Все простые числа от 2 до {n}:");
        foreach (uint number in numbers)
        {
            Console.Write(number + " ");
        }

    }
}
public class Calculations
{
    public static uint NOD(uint firstNumber, uint secondNumber)
    {
        if(firstNumber < secondNumber)
        {
            firstNumber ^= secondNumber;
            secondNumber ^= firstNumber;
            firstNumber ^= secondNumber;
        }

        uint reminder = 1;
        reminder = firstNumber % secondNumber;
        uint previousReminder = secondNumber;


        while (reminder != 0)
        {
            firstNumber = secondNumber;
            secondNumber = reminder;
            previousReminder = reminder;
            reminder = firstNumber % secondNumber;
        }
        return previousReminder;
    }
    public static uint NOD(uint firstNumber, uint secondNumber, uint thirdNumber)
    {
        return NOD(thirdNumber, NOD(firstNumber, secondNumber));
    }

    public static List<uint> SieveEratosthenes(uint n)
    {
        var numbers = new List<uint>();
        for (var i = 2u; i < n; i++)
        {
            numbers.Add(i);
        }

        for (var i = 0; i < numbers.Count; i++)
        {
            for (var j = 2u; j < n; j++)
            {
                numbers.Remove(numbers[i] * j);
            }
        }

        return numbers;
    }
}
