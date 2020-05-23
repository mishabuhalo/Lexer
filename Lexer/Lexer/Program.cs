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
        public static string input = "function(var s, var i) var testVariable \n{ for(testVariable) testVariable sdfsdkf a+testVariable+ 24214 +saf [\"testAttribute\"]; TestObject[\"attribute\"]  asfaf[\"fssa\"]; } -124+ var 7 432+34-2fdsgf while = 24do 124;" +
            " asd[\"asd\"]; testVariable";


        static void Main(string[] args)
        {
            int option;

            Console.WriteLine("This is input string = " + input);

            Console.WriteLine("Please imput option for output:\n1)sequence of tokens\n2)tokens sorted by type\n");
            option = int.Parse(Console.ReadLine());
            Lexer.init(input, option);

            Console.ReadLine();

        }
    }
}
