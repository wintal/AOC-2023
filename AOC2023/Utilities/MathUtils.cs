namespace Utilities
{
    public class MathUtils
    {
        public static  ulong  GreatestCommonDivisor(ulong a, ulong b)
        {
            while (b != 0)
            {
                ulong temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static ulong LowestCommonMultiple(ulong a, ulong b)
        {
            return a * b / GreatestCommonDivisor(a, b);
        }


    }
}