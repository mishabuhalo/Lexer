﻿using System;
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
        private static string eolRegEx = @"^(\d+|\w+);$";
        private static string expressionFirst = @"(\=|\+|\\|\-|\*)?(\w+|\d+)(\=|\+|\\|\-|\*)";
        private static string expressionSecond = @"(\=|\+|\\|\-|\*)(\w+|\d+)(\=|\+|\\|\-|\*)?";
        private static string keyWords = @"^function\(?|^for\(?|^while\(?|^var\(?|^do\(?|^try\(?|^catch\(?";
        private static string operatorsRexEx = @"^(\=|\+|\/|\-|\*)$";

        private static List<string> words;
        private static List<Tokens> result = new List<Tokens>();

        public static List<string> keyWordList = new List<string>() { "function", "for", "var", "while", "do", "try", "catch"};
        public static List<string> operatorsList = new List<string>() { "+", "-", "/" , "*", "="};

        public static List<Tokens> init(string input)
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
                if(Regex.IsMatch(words[i], eolRegEx))
                {
                    string temp = "";
                    for(int j = 0; j < words[i].Length; j++)
                    {
                        if (words[i][j] != ';')
                            temp += words[i][j];
                    }

                    if(isDigit(temp))
                        result.Add(new Tokens(TokensNames.Number, temp));
                    else if (iskeyWord(temp))
                        result.Add(new Tokens(TokensNames.KeyWord, temp));
                    else
                        result.Add(new Tokens(TokensNames.ErrorToken, temp));

                    result.Add(new Tokens(TokensNames.PunctuationMark, ";"));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], keyWords))
                {
                    result.Add(new Tokens(TokensNames.KeyWord, words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if(Regex.IsMatch(words[i],operatorsRexEx))
                 {
                    result.Add(new Tokens(TokensNames.Operator, words[i]));
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
                if (Regex.IsMatch(words[i], expressionFirst)|| Regex.IsMatch(words[i], expressionSecond))
                {
                    string temp = "";

                    for(int j = 0; j < words[i].Length; j++)
                    {
                        if(!isOperator(words[i][j]))
                        {
                            temp += words[i][j];
                             words[i] = words[i].Remove(j, 1);
                            if(j == words[i].Length)
                            {

                            }
                            j--;
                        }

                        else
                        {
                            if(temp != "" && isDigit(temp))
                            {
                                result.Add(new Tokens(TokensNames.Number, temp));
                                temp = "";
                            }
                            else if(temp != "" && isVariable(temp))
                            {
                                result.Add(new Tokens(TokensNames.Variable, temp));
                                temp = "";
                            }
                            else if(temp != "")
                            {
                                result.Add(new Tokens(TokensNames.ErrorToken, temp));
                                temp = "";

                            }
                            result.Add(new Tokens(TokensNames.Operator, words[i][j].ToString()));
                            words[i] = words[i].Remove(j, 1);
                            j--;
                        }

                    }

                    if(temp!="")
                    {
                        result.Add(new Tokens(TokensNames.ErrorToken, temp));
                        temp = "";
                    }

                    words.RemoveAt(i);
                    i--;
                    continue;
                }

                result.Add(new Tokens(TokensNames.ErrorToken, words[i]));
                words.RemoveAt(i);
                i--;

            }

            printResult();
            return result;
        }

        public static void printResult()
        {
            foreach(var res in result)
                res.printToken();
        }

      
        private static string[] getWords(string input)
        {
            return input.Split(new[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
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

            for(int i = 0; i < keyWordList.Count(); i ++)
            {
                if(keyWordList[i] == input)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        private static bool isVariable(string input)
        {
            bool flag = true;
            for(int i = 0; i < input.Length; i++)
            {
                if (input[i] < 'a' || input[i] > 'z')
                    flag = false;
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
            for(int i =0; i < operatorsList.Count(); i++)
            {
                if(input.ToString() == operatorsList[i])
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
    }
}