using System.ComponentModel;
namespace Delegate {
    public delegate int del(int a, int b);
    
public class Calculator
{
    public static int Add(int x, int y) => x + y;
    public static int Multiply(int x, int y) => x * y;
}

class Program
{

        public static void Main(string[] args)
        {
            del del1 = Add;
            ExecutingClass exe1 = new ExecutingClass();
            exe1.execute(del1);

            Func<int, int, int> sub = (int a, int b) => a - b; //can also be assigned to fn1, fn1 matching signature (two int params and int return)
            Console.WriteLine(sub(12, 9));
            Action<int> print = (int a) => Console.WriteLine(a); //can also be assigned to fn1, where fn1 takes int as param and return void
            print(90);
        }
    static private int Add(int a, int b)
    {
        return a + b;
    }
}

class ExecutingClass
{
        public void execute(del del1)
        {
            Console.WriteLine(del1(3, 4));
    } 
}
}
