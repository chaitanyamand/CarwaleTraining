using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        List<int> numbers = new() { 1, 2, 3, 4, 5 };

        var deferredQuery = numbers.Where(n =>
        {
            Console.WriteLine($"Filtering {n}"); // Will run only on enumeration
            return n > 2;
        });

        var immediateResult = numbers.Where(n =>
        {
            Console.WriteLine($"Immediate Filtering {n}");
            return n > 2;
        }).ToList();

        numbers.Add(6); // modify source

        foreach (var n in deferredQuery)
        {
            Console.WriteLine($"Deferred Result: {n}");
        }

        foreach (var n in immediateResult)
        {
            Console.WriteLine($"Immediate Result: {n}");
        }
    }
}
