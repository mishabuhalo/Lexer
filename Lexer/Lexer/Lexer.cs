using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Lexer
{
    static class Lexer
    {
        private static string simpleNumberRegEx = @"^\d+($)";
        private static string simpleWordRegEx = @"^\w+$";
        private static string wordWithLiteralsRegEx = @"^\w+(\)|;|\(|,)$";
        private static string eolRegEx = @"^(\d+|\w+);$";
        private static string expressionFirstRegEx = @"(\=|\+|\\|\-|\*)?(\w+|\d+)(\=|\+|\\|\-|\*)";
        private static string expressionSecondRegEx = @"(\=|\+|\\|\-|\*)(\w+|\d+)(\=|\+|\\|\-|\*)?";
        private static string keyWordsRegEx = @"^function\(?|^for\(?|^while\(?|^var\(?|^do\(?|^try\(?|^catch\(?";
        private static string operatorsRegEx = @"^(\=|\+|\/|\-|\*)$";
        private static string attributesRegEx = @"^\w*\[""\w+""\]($|;)";
        private static string literalsRegEx = @"^(\(|\)|\'|""|\[|\{|\}|\])$";
        private static string punctualMarksRegEx = @"^(,|;)$";


        private static List<string> words;
        private static List<Tokens> result = new List<Tokens>();

        public static List<string> keyWordList = new List<string>() { "function", "for", "var", "while", "do", "try", "catch" };
        public static List<string> operatorsList = new List<string>() { "+", "-", "/", "*", "=" };

        public static List<Tokens> init(string input, int option)
        {
            words = new List<string>();
            words.AddRange(getWords(input));

           // prindWords();

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
                    string temp = "";
                    for (int j = 0; j < words[i].Length; j++)
                    {
                        if (words[i][j] != ';')
                            temp += words[i][j];
                    }

                    if (isDigit(temp))
                        result.Add(new Tokens(TokensNames.Number, temp));
                    else if (iskeyWord(temp))
                        result.Add(new Tokens(TokensNames.KeyWord, temp));
                    else
                        result.Add(new Tokens(TokensNames.ErrorToken, temp));

                    result.Add(new Tokens(TokensNames.SybmolLiteral, ";"));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], keyWordsRegEx))
                {
                    string temp = "";
                    bool flag = true;
                    for (int j = 0; j < words[i].Length; j++)
                    {
                        if (!Regex.IsMatch(words[i][j].ToString(), literalsRegEx) && flag)
                        {
                            temp += words[i][j];
                            continue;
                        }
                        if (temp != "" || Regex.IsMatch(words[i][j].ToString(), literalsRegEx))
                        {
                            if (temp != "")
                            {
                                flag = false;
                                if (iskeyWord(temp))
                                    result.Add(new Tokens(TokensNames.KeyWord, temp));
                                else if (isVariable(result, temp))
                                    result.Add(new Tokens(TokensNames.Variable, temp));
                                else
                                    result.Add(new Tokens(TokensNames.ErrorToken, temp));
                                temp = "";
                            }
                            if (Regex.IsMatch(words[i][j].ToString(), literalsRegEx))
                            {
                                result.Add(new Tokens(TokensNames.SybmolLiteral, words[i][j].ToString()));
                                flag = true;
                            }
                        }
                    }
                    if (temp != "")
                    {
                        if (iskeyWord(temp))
                            result.Add(new Tokens(TokensNames.KeyWord, temp));
                        else
                            result.Add(new Tokens(TokensNames.ErrorToken, temp));
                    }
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
                    string temp = "";
                    bool objectFlag = true;
                    for (int j = 0; j < words[i].Length; j++)
                    {
                        if (words[i][j] != '[' && words[i][j] != '"' && words[i][j] != ']' && words[i][j] != ';')
                        {
                            temp += words[i][j];
                            continue;
                        }
                        if (temp != "" && objectFlag)
                        {
                            result.Add(new Tokens(TokensNames.Object, temp));
                            temp = "";
                        }
                        if (Regex.IsMatch(words[i][j].ToString(), literalsRegEx))
                            result.Add(new Tokens(TokensNames.SybmolLiteral, words[i][j].ToString()));
                        if (Regex.IsMatch(words[i][j].ToString(), punctualMarksRegEx))
                        {
                            result.Add(new Tokens(TokensNames.PunctuationMark, words[i][j].ToString()));
                        }

                    }

                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], expressionFirstRegEx) || Regex.IsMatch(words[i], expressionSecondRegEx))
                {
                    string temp = "";

                    for (int j = 0; j < words[i].Length; j++)
                    {
                        if (!isOperator(words[i][j]))
                        {
                            temp += words[i][j];
                            words[i] = words[i].Remove(j, 1);
                            if (j == words[i].Length)
                            {

                            }
                            j--;
                        }

                        else
                        {
                            if (temp != "" && isDigit(temp))
                            {
                                result.Add(new Tokens(TokensNames.Number, temp));
                                temp = "";
                            }
                            else if (temp != "" && isVariable(result, temp))
                            {
                                result.Add(new Tokens(TokensNames.Variable, temp));
                                temp = "";
                            }
                            else if (temp != "")
                            {
                                result.Add(new Tokens(TokensNames.ErrorToken, temp));
                                temp = "";

                            }
                            result.Add(new Tokens(TokensNames.Operator, words[i][j].ToString()));
                            words[i] = words[i].Remove(j, 1);
                            j--;
                        }

                    }

                    if (temp != "")
                    {
                        result.Add(new Tokens(TokensNames.ErrorToken, temp));
                        temp = "";
                    }

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

                if (result.Count() > 0 && isVariable(result, words[i]))
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
                        string temp = "";
                        for (int j = 0; j < words[i].Length; j++)
                        {
                            if (!Regex.IsMatch(words[i][j].ToString(), literalsRegEx))
                                temp += words[i][j];
                        }

                        result.Add(new Tokens(TokensNames.Variable, temp));
                        if(Regex.IsMatch(words[i][words[i].Length - 1].ToString(), literalsRegEx))
                            result.Add(new Tokens(TokensNames.SybmolLiteral, words[i][words[i].Length-1].ToString()));
                        else if(Regex.IsMatch(words[i][words[i].Length - 1].ToString(), punctualMarksRegEx))
                        {

                            result.Add(new Tokens(TokensNames.PunctuationMark, words[i][words[i].Length - 1].ToString()));
                        }
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


        private static string[] getWords(string input)
        {
            return input.Split(new[] { '\n',' ' ,'\r'}, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void prindWords()
        {
            foreach (string word in words)
            {
                Console.WriteLine(word);
            }
        }

        private static bool iskeyWord(string input)
        {
            bool flag = false;

            for (int i = 0; i < keyWordList.Count(); i++)
            {
                if (keyWordList[i] == input)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        private static bool isDigit(string input)
        {
            if (input.All(Char.IsDigit))
                return true;

            else return false;
        }

        private static bool isOperator(char input)
        {
            bool flag = false;
            for (int i = 0; i < operatorsList.Count(); i++)
            {
                if (input.ToString() == operatorsList[i])
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        private static bool isVariable(List<Tokens> tokenList, string token)
        {
            if (tokenList[tokenList.Count() - 1].token == "var")
                return true;

            for (int i = 0; i < tokenList.Count(); i++)
            {
                if (tokenList[i].token == token && Enum.GetName(typeof(TokensNames), tokenList[i].tokensNames) == "Variable")
                    return true;
            }
            return false;
        }
    }
}
