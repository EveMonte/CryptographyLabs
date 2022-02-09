class Program
{
    public static void Main()
    {
        Console.Write("Введите n: ");
        int n = int.Parse(Console.ReadLine());
        Console.Write("Введите m: ");
        int m = int.Parse(Console.ReadLine());

        Console.WriteLine($"НОД: {Calculations.NOD((uint)m, (uint)n)}");
        
        List<uint> numbers = Calculations.SieveEratosthenes((uint)n);
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
        uint previousReminder = 1;
        reminder = firstNumber % secondNumber;

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
