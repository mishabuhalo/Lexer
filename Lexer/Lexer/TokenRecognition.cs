using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lexer
{
    public static class TokenRecognition
    {
        public static List<string> keyWordList = new List<string>() { "function", "for", "var", "while", "do", "try", "catch" };
        public static List<string> operatorsList = new List<string>() { "+", "-", "/", "*", "=" };
        private static string literalsRegEx = @"^(\(|\)|\'|""|\[|\{|\}|\])$";
        private static string punctualMarksRegEx = @"^(,|;)$";
        private static string wordWithLiteralsRegEx = @"^\w+(\)|;|\(|,)$";

        public static List<Tokens> EndOfLineTokens(string template)
        {
            List<Tokens> result = new List<Tokens>();
            string temp = "";
            for (int j = 0; j < template.Length; j++)
            {
                if (template[j] != ';')
                    temp += template[j];
            }

            if (isDigit(temp))
                result.Add(new Tokens(TokensNames.Number, temp));
            else if (iskeyWord(temp))
                result.Add(new Tokens(TokensNames.KeyWord, temp));
            else if (isVariable(result, temp))
                result.Add(new Tokens(TokensNames.Variable, temp));
            else
                result.Add(new Tokens(TokensNames.ErrorToken, temp));

            result.Add(new Tokens(TokensNames.SybmolLiteral, ";"));

            return result;
        }

        public static List<Tokens> KeyWordTokens(string template)
        {
            List<Tokens> result = new List<Tokens>();

            string temp = "";
            bool flag = true;
            for (int j = 0; j < template.Length; j++)
            {
                if (!Regex.IsMatch(template[j].ToString(), literalsRegEx) && flag)
                {
                    temp += template[j];
                    continue;
                }
                if (temp != "" || Regex.IsMatch(template[j].ToString(), literalsRegEx))
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
                    if (Regex.IsMatch(template[j].ToString(), literalsRegEx))
                    {
                        result.Add(new Tokens(TokensNames.SybmolLiteral, template[j].ToString()));
                        flag = true;
                    }
                }
            }
            if (temp != "")
            {
                if (iskeyWord(temp))
                    result.Add(new Tokens(TokensNames.KeyWord, temp));
                else if (isVariable(result, temp))
                    result.Add(new Tokens(TokensNames.Variable, temp));
                else
                    result.Add(new Tokens(TokensNames.ErrorToken, temp));
            }

            return result;
        }

        public static List<Tokens> AttributeTokens(string template)
        {
            List<Tokens> result = new List<Tokens>();

            string temp = "";
            bool objectFlag = true;
            for (int j = 0; j < template.Length; j++)
            {
                if (template[j] != '[' && template[j] != '"' && template[j] != ']' && template[j] != ';')
                {
                    temp += template[j];
                    continue;
                }
                if (temp != "" && objectFlag)
                {
                    result.Add(new Tokens(TokensNames.Object, temp));
                    temp = "";
                }
                if (temp != "" && !objectFlag)
                {
                    result.Add(new Tokens(TokensNames.Attribute, temp));
                    temp = "";
                }
                if (Regex.IsMatch(template[j].ToString(), literalsRegEx))
                {
                    result.Add(new Tokens(TokensNames.SybmolLiteral, template[j].ToString()));
                    objectFlag = false;
                }
                if (Regex.IsMatch(template[j].ToString(), punctualMarksRegEx))
                {
                    result.Add(new Tokens(TokensNames.PunctuationMark, template[j].ToString()));
                    objectFlag = false;
                }

            }

            return result;
        }

        public static List<Tokens> ExpressionTokens(string template)
        {
            List<Tokens> result = new List<Tokens>();

            string temp = "";

            for (int j = 0; j < template.Length; j++)
            {
                if (!isOperator(template[j]))
                {
                    temp += template[j];
                    template = template.Remove(j, 1);
                    if (j == template.Length)
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
                    result.Add(new Tokens(TokensNames.Operator, template[j].ToString()));
                    template = template.Remove(j, 1);
                    j--;
                }

            }

            if (temp != "")
            {
                result.Add(new Tokens(TokensNames.ErrorToken, temp));
                temp = "";
            }

            return result;

        }

        public static List<Tokens> VariableTokens(string template)
        {
            List<Tokens> result = new List<Tokens>();
           

            if (Regex.IsMatch(template, wordWithLiteralsRegEx))
            {
                string temp = "";

                for (int j = 0; j < template.Length; j++)
                {
                    if (!Regex.IsMatch(template[j].ToString(), literalsRegEx))
                        temp += template[j];
                }

                result.Add(new Tokens(TokensNames.Variable, temp));

                if (Regex.IsMatch(template[template.Length - 1].ToString(), literalsRegEx))
                    result.Add(new Tokens(TokensNames.SybmolLiteral, template[template.Length - 1].ToString()));
                else if (Regex.IsMatch(template[template.Length - 1].ToString(), punctualMarksRegEx))
                {

                    result.Add(new Tokens(TokensNames.PunctuationMark, template[template.Length - 1].ToString()));
                }

            }

            return result;
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

        public static bool isVariable(List<Tokens> tokenList, string token)
        {
            if (tokenList.Count() > 0)
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
            else
                return false;
        }

    }
}
