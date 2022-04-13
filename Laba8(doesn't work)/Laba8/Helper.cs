using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba8
{
    public class Helper
    {
        public static long GetPRoot(long p)
        {
            for (long i = 0; i < p; i++)
                if (IsPRoot(p, i))
                    return i;
            return 0;
        }

        static bool IsPRoot(long p, long a)
        {
            if (a == 0 || a == 1)
                return false;
            long last = 1;
            var set = new HashSet<long>();
            for (long i = 0; i < p - 1; i++)
            {
                last = (last * a) % p;
                if (set.Contains(last)) // Если повтор
                    return false;
                set.Add(last);
            }
            return true;
        }

        public static long EulersFunction(long p, long q)
        {
            return (p - 1) * (q - 1);
        }

        public static bool IsPrime(long number)
        {
            bool isPrime = true;
            for (long i = 2; i < number / 2; i++)
            {
                if (number % i == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            return isPrime;
        }

        public static long Gcd(double v, long firstNumber, long secondNumber)
        {
            if (firstNumber < secondNumber)
            {
                firstNumber ^= secondNumber;
                secondNumber ^= firstNumber;
                firstNumber ^= secondNumber;
            }

            long reminder = 1;
            reminder = firstNumber % secondNumber;
            long previousReminder = secondNumber;


            while (reminder != 0)
            {
                firstNumber = secondNumber;
                secondNumber = reminder;
                previousReminder = reminder;
                reminder = firstNumber % secondNumber;
            }
            return previousReminder;
        }
        public static long gcd(long a, long b, out long lastx, out long lasty)
        {
            if (b > a)
            {
                long f;
                f = a;
                a = b;
                b = f;
            }
            long x = 0;
            long y = 1;
            lastx = 1;
            lasty = 0;
            long quo;
            long temp;
            while (b != 0)
            {
                quo = a / b;
                temp = b;
                b = a % b;
                a = temp;
                temp = x;
                x = lastx - quo * x;
                lastx = temp;
                temp = y;
                y = lasty - quo * y;
                lasty = temp;
            }
            return a;

        }
        //public static double Gcd(double a, double b, out double x, out double y) // greatest common divider, extended Euclid's algorithm
        //{
        //    if (b < a)
        //    {
        //        double f;
        //        f = a;
        //        a = b;
        //        b = f;
        //    }

        //    if (a == 0)
        //    {
        //        x = 0;
        //        y = 1;
        //        return b;
        //    }

        //    double gcd = Gcd(b % a, a, out x, out y);

        //    double newY = x;
        //    double newX = y - (b / a) * x;

        //    x = newX;
        //    y = newY;
        //    return gcd;
        //}

        public static long Reminder(long b, long exp, long m)
        {
            long origin = b;
            b %= m;
            for (long i = 1; i < exp; i++)
            {
                b *= origin;
                b %= m;
            }
            return b;
        }
    }
}
