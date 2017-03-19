using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collections.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            foreach (var item in Collections.Tasks.Task.GenerateAllPermutations(new int[] { 1, 2, 3, 4 }, 2))
            {
                foreach (var i in item)
                    Console.Write(i);
                Console.Write("  ");
            }
             
        }
    }
}
