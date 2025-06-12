using System.Collections;

namespace Collection
{
    class Collection
    {
        public static void MainTest(string[] args)
        {
            ArrayList a = new ArrayList();
            a.Add("Hello");
            a.Add(1);

            foreach (var o in a)
            {
                Console.Write(o + " ");
            }

            List<int> list1 = new List<int>(); //Can understand List as Vector in CPP
            list1.Add(1);
            list1.Add(2);

            List<int> list2 = new List<int>();
            list2.Add(3);
            list2.Add(4);

            list1.AddRange(list2);
            Console.WriteLine();
            foreach (var x in list1)
            {
                Console.Write(x + " ");
            }
            Console.WriteLine();

            Console.WriteLine("Capacity " + list1.Capacity);
            Console.WriteLine("Count " + list1.Count);

            list1.Add(5);

            Console.WriteLine("Capacity " + list1.Capacity);
            Console.WriteLine("Count " + list1.Count);

            list1.Insert(0, 90);  //index,element
            list1[0] = 67; //Allows Random Update

            list1.RemoveAt(1);
            list1.Remove(90);
            if (list1.Contains(67))
            {
                Console.WriteLine("Contains");
            }
            else
            {
                Console.WriteLine("Doesn't Contain");
            }
            Console.WriteLine();
            foreach (var x in list1)
            {
                Console.Write(x + " ");
            }
            list1.Clear();



            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(10, "Ten");
            dict.Add(11, "Eleven");
            dict[12] = "Twelve";
            dict.Remove(10); //key

            Console.WriteLine();
            if (dict.ContainsKey(11))
            {
                Console.WriteLine("Contains");
            }
            else
            {
                Console.WriteLine("Doesn't Contain"); 
            }
            foreach (var x in dict)
                {
                    Console.Write(x + " ");
                }
        }
    }
}