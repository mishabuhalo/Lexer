using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Lexer
{
    static class Lexer
    {
        private static readonly string simpleNumberRegEx = @"^\d+($)";
        private static readonly string simpleWordRegEx = @"^\w+$";
        private static readonly string wordWithLiteralsRegEx = @"^\w+(\)|;|\(|,)$";
        private static readonly string eolRegEx = @"^(\d+|\w+);$";
        private static readonly string expressionFirstRegEx = @"(\=|\+|\\|\-|\*)?(\w+|\d+)(\=|\+|\\|\-|\*)";
        private static readonly string expressionSecondRegEx = @"(\=|\+|\\|\-|\*)(\w+|\d+)(\=|\+|\\|\-|\*)?";
        private static readonly string keyWordsRegEx = @"^function\(?|^for\(?|^while\(?|^var\(?|^do\(?|^try\(?|^catch\(?";
        private static readonly string operatorsRegEx = @"^(\=|\+|\/|\-|\*)$";
        private static readonly string attributesRegEx = @"^\w*\[""\w+""\]($|;)";
        private static readonly string literalsRegEx = @"^(\(|\)|\'|""|\[|\{|\}|\])$";
        private static readonly string punctualMarksRegEx = @"^(,|;)$";


        private static List<string> words = new List<string>();
        private static List<Tokens> result = new List<Tokens>();

        public static List<string> keyWordList = new List<string>() { "function", "for", "var", "while", "do", "try", "catch" };
        public static List<string> operatorsList = new List<string>() { "+", "-", "/", "*", "=" };

        public static List<Tokens> init(string input, int option)
        {
            getWords(input);

            //prindWords();

            for (int i = 0; i < words.Count(); i++)
            {
                if (Regex.IsMatch(words[i], simpleNumberRegEx))
                {
                    result.Add(new Tokens(TokensNames.Number, words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], eolRegEx))
                {
                    result.AddRange(TokenRecognition.EndOfLineTokens(words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], keyWordsRegEx))
                {
                    result.AddRange(TokenRecognition.KeyWordTokens(words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], operatorsRegEx))
                {
                    result.Add(new Tokens(TokensNames.Operator, words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], attributesRegEx, RegexOptions.IgnoreCase))
                {
                    result.AddRange(TokenRecognition.AttributeTokens(words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], expressionFirstRegEx) || Regex.IsMatch(words[i], expressionSecondRegEx))
                {
                    result.AddRange(TokenRecognition.ExpressionTokens(words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }

                if (Regex.IsMatch(words[i], literalsRegEx))
                {
                    result.Add(new Tokens(TokensNames.SybmolLiteral, words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if(Regex.IsMatch(words[i], punctualMarksRegEx))
                {
                    result.Add(new Tokens(TokensNames.PunctuationMark, words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }

                if (result.Count() > 0 && TokenRecognition.isVariable(result, words[i]))
                {
                    if (Regex.IsMatch(words[i], simpleWordRegEx))
                    {
                        result.Add(new Tokens(TokensNames.Variable, words[i]));
                        words.RemoveAt(i);
                        i--;
                        continue;
                    }

                    else if(Regex.IsMatch(words[i], wordWithLiteralsRegEx))
                    {
                        result.AddRange(TokenRecognition.VariableTokens(words[i]));
                        words.RemoveAt(i);
                        i--;
                        continue;
                    }
                    
                }
                else
                {
                    result.Add(new Tokens(TokensNames.ErrorToken, words[i]));
                    words.RemoveAt(i);
                    i--;
                }

            }
            printResult(option);

            return result;
        }

        public static void printResult(int option)
        {
            if (option == 1)
            {
                foreach (var res in result)
                    res.printToken();
            }
            else if (option == 2)
            {
                foreach (TokensNames tokens in Enum.GetValues(typeof(TokensNames)))
                {
                    Console.WriteLine("Token name: " + Enum.GetName(typeof(TokensNames), tokens));
                    for (int i = 0; i < result.Count(); i++)
                    {
                        if (Enum.GetName(typeof(TokensNames), result[i].tokensNames) == Enum.GetName(typeof(TokensNames), tokens))
                        {
                            Console.WriteLine("\"" + result[i].token + "\"");
                        }

                    }
                    Console.WriteLine();
                }
            }
        }


        private static void getWords(string input)
        {
            string[] lines = File.ReadAllLines(input, Encoding.UTF8);
            foreach(var line in lines)
                 words.AddRange(line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void prindWords()
        {
            foreach (string word in words)
            {
                Console.WriteLine(word);
            }
        }

        
    }
}
