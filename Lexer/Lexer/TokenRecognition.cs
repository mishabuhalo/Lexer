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
        public static List<string> keyWordList = new List<string>() { "function", "for", "var", "while", "do", "try", "catch", "if", "else" };
        public static List<string> operatorsList = new List<string>() { "+", "-", "/", "*", "==", ">=", "<=", "&&", "||", "=",  "%", "<", ">" };
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
            else if (isVariable(Lexer.result, temp))
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
                        else if (isVariable(Lexer.result, temp))
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
                else if (isVariable(Lexer.result, temp))
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
                if (!isOperator(template[j].ToString()))
                {
                    temp += template[j];
                    template = template.Remove(j, 1);
                    j--;
                }

                else
                {
                    if (temp != "" && isDigit(temp))
                    {
                        result.Add(new Tokens(TokensNames.Number, temp));
                        temp = "";
                    }
                    else if (temp != "" && isVariable(Lexer.result, temp))
                    {
                        result.Add(new Tokens(TokensNames.Variable, temp));
                        temp = "";
                    }
                    else if (temp != "")
                    {
                        result.Add(new Tokens(TokensNames.ErrorToken, temp));
                        temp = "";

                    }

                    if (j +1 <= template.Length -1 && isOperator(template[j].ToString() + template[j + 1].ToString()))
                    {

                        result.Add(new Tokens(TokensNames.Operator, template[j].ToString() + template[j + 1].ToString()));
                        template = template.Remove(j, 2);
                        j --;
                    }
                    else
                    {
                        result.Add(new Tokens(TokensNames.Operator, template[j].ToString()));
                        template = template.Remove(j, 1);
                        j--;
                    }
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
                    if (!Regex.IsMatch(template[j].ToString(), literalsRegEx)&&!Regex.IsMatch(template[j].ToString(), punctualMarksRegEx))
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

        public static List<Tokens> singleLineCommentsToken(string line)
        {
            List<string> tempWords = new List<string>();
            List<Tokens> result = new List<Tokens>();
            string temp = "";
            string tempLine = line;
            for (int i = 0; i < line.Length - 1; i++)
            {

                if (line[i] == '/' && line[i + 1] == '/')
                {
                    tempWords.Clear();
                    tempLine = line.Remove(0, temp.Length + 2);

                    List<string> comments = new List<string>();

                    tempWords.Clear();
                    tempWords.AddRange(temp.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    comments.AddRange(tempLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                    foreach (var word in tempWords)
                    {
                        result.AddRange(Lexer.recognize(word));
                    }

                    foreach (var comment in comments)
                    {
                        result.Add(new Tokens(TokensNames.CommentElement, comment));

                    }

                    break;
                }
                else
                {
                    temp += line[i];
                    continue;
                }
            }

            return result;
        }
        public static List<Tokens> multyLineCommentTextTokens(string line)
        {
            List<Tokens> result = new List<Tokens>();
            string startLine = "";
            string endLine = "";
            string commentsLine = "";

            List<string> startTokens = new List<string>();
            List<string> commentsTokens = new List<string>();
            List<string> endTokens = new List<string>();

            string templine = line;
            

            for (int i = 0; i < line.Length-1; i ++)
            {
                if(line[i] == '/' && line[i+1] == '*')
                {

                    startTokens.Clear();
                    templine = templine.Remove(0, startLine.Length + 2);

                    for(int j = 0; j < templine.Length-1; j++)
                    {
                        if(templine[j] == '*' && templine[j+1] == '/')
                        {
                            templine = templine.Remove(0, commentsLine.Length +2);

                            endLine = templine;
                        }

                        else
                        {
                            commentsLine += templine[j];
                            continue;
                        }
                    }
                    break;
                    
                }

                else
                {
                    startLine += line[i];
                    continue;
                }
            }

            startTokens.AddRange(startLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            commentsTokens.AddRange(commentsLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            endTokens.AddRange(endLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            foreach(var token in startTokens)
            {
                result.AddRange(Lexer.recognize(token));
            }

            foreach(var token in commentsTokens)
            {
                result.Add(new Tokens(TokensNames.CommentElement, token));
            }

            foreach(var token in endTokens)
            {
                result.AddRange(Lexer.recognize(token));
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

        private static bool isOperator(string input)
        {
            bool flag = false;
            for (int i = 0; i < operatorsList.Count(); i++)
            {
                if (input == operatorsList[i])
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
