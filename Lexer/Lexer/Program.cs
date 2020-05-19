using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Lexer
{

    class Program
    {
        public static string input = "function () { for sdfsdkf a+b23+ 24214 +saf} -124+ var 7 432+34-2fdsgf while = 24do 124; ";


        static void Main(string[] args)
        {
            string temp = "-124+ 7";


            string numbersinExpression = @"\W*\d+\W*";

            Console.WriteLine("This is input string = " + input);
            //Console.WriteLine("Temp = " + Regex.IsMatch(temp, numbersinExpression));

            Lexer.init(input);

            Console.ReadLine();

        }
    }
}
