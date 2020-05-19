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

       
        public static string input = "124 dsf 23dsfs , 3; 65";

        static void Main(string[] args)
        {
            List<string> matches = new List<string>();
            Regex regex= new Regex(@"(\s|^)\d+(\s|;|$)");

            List<Tokens> result = new List<Tokens>();

            MatchCollection match = regex.Matches(input);

            if(match.Count > 0)
            {
                foreach(Match temp in match)
                {
                    matches.Add(temp.Value);
                    
                    Console.WriteLine(temp.Value);
                }

                Console.WriteLine("Redaction");

                for (int j = 0; j < matches.Count(); j++)
                {
                    for (int i = 0; i < matches[j].Length; i++)
                    {
                        if (!Char.IsDigit(matches[j][i]))
                        {
                            matches[j]= matches[j].Remove(i, 1);
                        }
                    }
                    Console.WriteLine(matches[j]);
                    result.Add(new Tokens(TokensNames.Numbers, matches[j]));
                }
                Console.WriteLine("Tokens");
                for(int i = 0; i < result.Count(); i ++)
                {
                    result[i].printToken();
                }
            }

            Console.ReadLine();

        }
    }
}
