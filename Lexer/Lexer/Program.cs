using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Lexer
{

    class Program
    {
        public static string path = @"test.txt";

        static void Main(string[] args)
        {
            int option;
            Console.WriteLine("Please imput option for output:\n1)sequence of tokens\n2)tokens sorted by type\n");
            option = int.Parse(Console.ReadLine());

            Lexer.init(path, option);

            Console.ReadLine();

        }
    }
}
