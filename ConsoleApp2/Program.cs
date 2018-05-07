using System;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var date = DateTime.UtcNow;
            var mili =(long) date.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds;

            var date2 = new DateTime();

        }
    }
}
