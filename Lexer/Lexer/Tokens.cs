using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public enum TokensNames
    {
        Number,
        PunctuationMark,
        ErrorToken,
        KeyWord,
        Expression,
        Operator,
        Variable
    }

    class Tokens
    {
        public TokensNames tokensNames;
        public string token;

        public Tokens(TokensNames tokensNames, string token)
        {
            this.tokensNames = tokensNames;
            this.token = token;
        }

        public void printToken()
        {
            Console.WriteLine($"[\"" + Enum.GetName(typeof(TokensNames), tokensNames) +"\", \""+token+"\"]");
        }
    }
}
