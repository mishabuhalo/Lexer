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
        private static readonly string expressionFirstRegEx = @"(\=|\+|\\|\-|\*|==|>|<|<=|>=|\|\||&&)?(\=|\+|\\|\-|\*|==|>|<|<=|>=|\|\||&&)";
        private static readonly string expressionSecondRegEx = @"(\=|\+|\\|\-|\*|==|>|<|<=|>=|\|\||&&)(\w+|\d+)(\=|\+|\\|\-|\*|==|>|<|<=|>=|\|\||&&)?";
        private static readonly string keyWordsRegEx = @"^function\(?|^for\(?|^while\(?|^var\(?|^do\(?|^try\(?|^catch\(?|^else\(?|^if\(?";
        private static readonly string operatorsRegEx = @"^(\=|\+|\/|\-|\*)$";
        private static readonly string attributesRegEx = @"^\w*\[""\w+""\]($|;)";
        private static readonly string literalsRegEx = @"^(\(|\)|\'|""|\[|\{|\}|\])$";
        private static readonly string punctualMarksRegEx = @"^(,|;)$";
        private static readonly string singeLineCommentRegEx = @"(^|\w*)\/\/(\w*|$)";
        private static readonly string multyLineCommentStartRegEx = @"(^|\w*)\/\*(\w*|$)";
        private static readonly string multyLineCommentEndRegEx = @"(^|\w*)\*\/(\w*|$)";
        private static readonly string multyCommentInOneLineRegEx = @"(^|\w*)\/\*\w*\*\/(\w*|$)";


        private static List<string> words = new List<string>();
        public static List<Tokens> result = new List<Tokens>();


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
            int counter = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i], singeLineCommentRegEx))
                {
                    result.AddRange(TokenRecognition.singleLineCommentsToken(lines[i]));
                }
                else if (Regex.IsMatch(lines[i], multyLineCommentStartRegEx))
                {

                    string tempLine = lines[i];

                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (Regex.IsMatch(lines[j], multyLineCommentEndRegEx) || j == lines.Length - 1)
                        {
                            tempLine += " " + lines[j];

                            if (tempLine.Length > 0)
                            {
                                result.AddRange(TokenRecognition.multyLineCommentTextTokens(tempLine));
                                i += counter +1;
                                tempLine = "";
                                break;
                            }
                        }
                        else
                        {
                            tempLine += " " + lines[j];
                            counter++;
                        }
                    }
                    if (tempLine.Length > 0)
                        result.AddRange(TokenRecognition.multyLineCommentTextTokens(tempLine));
                }

                else
                {
                    tempWords.Clear();
                    //words.AddRange(line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                    tempWords.AddRange(lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

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

                return result;
            }

            if (Regex.IsMatch(word, keyWordsRegEx))
            {
                result.AddRange(TokenRecognition.KeyWordTokens(word));
                return result;

            }
            if (Regex.IsMatch(word, operatorsRegEx))
            {
                result.Add(new Tokens(TokensNames.Operator, word));
                return result;

            }
            if (Regex.IsMatch(word, attributesRegEx, RegexOptions.IgnoreCase))
            {
                result.AddRange(TokenRecognition.AttributeTokens(word));
                return result;

            }
            if (Regex.IsMatch(word, expressionFirstRegEx) || Regex.IsMatch(word, expressionSecondRegEx))
            {
                result.AddRange(TokenRecognition.ExpressionTokens(word));
                return result;
            }

            if (Regex.IsMatch(word, literalsRegEx))
            {
                result.Add(new Tokens(TokensNames.SybmolLiteral, word));
                return result;
            }
            if (Regex.IsMatch(word, punctualMarksRegEx))
            {
                result.Add(new Tokens(TokensNames.PunctuationMark, word));
                return result;
            }

            if (Lexer.result.Count() > 0 && TokenRecognition.isVariable(Lexer.result, word))
            {
                if (Regex.IsMatch(word, simpleWordRegEx))
                {
                    result.Add(new Tokens(TokensNames.Variable, word));

                }

                else if (Regex.IsMatch(word, wordWithLiteralsRegEx))
                {
                    result.AddRange(TokenRecognition.VariableTokens(word));

                }

                return result;

            }

            if (Regex.IsMatch(word, eolRegEx))
            {
                result.AddRange(TokenRecognition.EndOfLineTokens(word));
                return result;


            }
            else
            {
                result.Add(new Tokens(TokensNames.ErrorToken, word));
                return result;
            }
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
