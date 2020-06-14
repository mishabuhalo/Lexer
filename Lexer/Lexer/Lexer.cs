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
        private static readonly string singeLineCommentRegEx = @"(^|\w*)\/\/(\w*|$)";


        private static List<string> words = new List<string>();
        private static List<Tokens> result = new List<Tokens>();

        public static List<string> keyWordList = new List<string>() { "function", "for", "var", "while", "do", "try", "catch" };
        public static List<string> operatorsList = new List<string>() { "+", "-", "/", "*", "=" };


        public static List<Tokens> init(string input, int option)
        {
            getWords(input);

            //prindWords();

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

            List<string> tempWords = new List<string>();
            foreach (var line in lines)
            {
                if (Regex.IsMatch(line, singeLineCommentRegEx))
                {
                    result.AddRange(TokenRecognition.singleLineTokens(line));
                }

                else
                {
                    tempWords.Clear();
                    //words.AddRange(line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                    tempWords.AddRange(line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                    foreach (var word in tempWords)
                    {
                        result.AddRange(recognize(word));
                    }
                }

            }

        }

        public static List<Tokens> recognize(string word)
        {
            List<Tokens> result = new List<Tokens>();

                if (Regex.IsMatch(word, simpleNumberRegEx))
                {
                    result.Add(new Tokens(TokensNames.Number, word));
                   
                }
                if (Regex.IsMatch(word, eolRegEx))
                {
                    result.AddRange(TokenRecognition.EndOfLineTokens(word));
                  
                }
                if (Regex.IsMatch(word, keyWordsRegEx))
                {
                    result.AddRange(TokenRecognition.KeyWordTokens(word));
                   
                }
                if (Regex.IsMatch(word, operatorsRegEx))
                {
                    result.Add(new Tokens(TokensNames.Operator, word));
                   
                }
                if (Regex.IsMatch(word, attributesRegEx, RegexOptions.IgnoreCase))
                {
                    result.AddRange(TokenRecognition.AttributeTokens(word));
                   
                }
                if (Regex.IsMatch(word, expressionFirstRegEx) || Regex.IsMatch(word, expressionSecondRegEx))
                {
                    result.AddRange(TokenRecognition.ExpressionTokens(word));
                   
                }

                if (Regex.IsMatch(word, literalsRegEx))
                {
                    result.Add(new Tokens(TokensNames.SybmolLiteral, word));
                   
                }
                if (Regex.IsMatch(word, punctualMarksRegEx))
                {
                    result.Add(new Tokens(TokensNames.PunctuationMark, word));
                   
                }

                if (result.Count() > 0 && TokenRecognition.isVariable(result, word))
                {
                    if (Regex.IsMatch(word, simpleWordRegEx))
                    {
                        result.Add(new Tokens(TokensNames.Variable, word));
                        
                    }

                    else if (Regex.IsMatch(word, wordWithLiteralsRegEx))
                    {
                        result.AddRange(TokenRecognition.VariableTokens(word));
                       
                    }

                }
                else
                {
                    result.Add(new Tokens(TokensNames.ErrorToken, word));
                    
                }

            return result;
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
