using System.Text;
using System.Text.RegularExpressions;

namespace Lexxer
{
    // int a = 0;
    public enum State
    {
        NumberState,
        OperationState,
        EqualsState,
        DotState
    }

    public enum TokenType
    {
        Addition,
        Subtraction,
        Number,
        Division,
        Multiplication,
        Modulas,
        OpParen,
        ClParen,
        Word,
        Function,
        Int,
        Float,
        Begin,
        End,
        Char,
        Eol,
        Equals,
        Return,
        Returns,
        Struct,
        Bool,
        Comma,
        Not,
        Xor,
        Extern,
        Unsigned,
        True,
        False,
        CharLiteral,
        BoolEq,
        Lt,
        Gt,
        Gte,
        Lte,
        RShift,
        LShift,
        And,
        Or,
        Void,
        If,
        While,
        Mod,
        Array,
        Import,
        Else,
        Const,
        Int16,
        Int64,
        For,
        Depends,
        Pub,
        Dot,
        StringLiteral,
        String,
        Size,
        NotEquals,
        OpBracket,
        ClBracket,
        Colon,
        Uint16,
        Uint,
        Byte,
        Sbyte,
        Ulong
    }

    public struct Tokens(TokenType tokenType, string buffer, int number)
    {
        public TokenType tokenType = tokenType;
        public string buffer = buffer;

        public Tokens(TokenType tokenType)
            : this(tokenType, "", 0)
        {
        }

        public void SetLine(int lineNumber)
        {
            number = lineNumber;
        }

        public int GetLine()
        {
            return number;
        }

        public override string ToString()
        {
            return "lineNumber: " + number.ToString() + " " + tokenType + " (" + buffer + ")";
        }
    }

    public class LexTokens
    {
        private State CurrentState = State.NumberState;

        private void groupings(List<Tokens> tokens, StringBuilder buffer, int lineNumber)
        {
            Dictionary<string, Tokens> keyWords =
                new()
                {
                    //operators/numbers
                    ["+"] = new(TokenType.Addition),
                    ["-"] = new(TokenType.Subtraction),
                    ["*"] = new(TokenType.Multiplication),
                    ["/"] = new(TokenType.Division),
                    ["%"] = new(TokenType.Modulas),
                    [")"] = new(TokenType.ClParen),
                    ["("] = new(TokenType.OpParen),
                    ["^"] = new(TokenType.Xor),
                    ["."] = new(TokenType.Dot),
                    ["|"] = new(TokenType.Or),
                    ["&"] = new(TokenType.And),
                    [">"] = new(TokenType.Gt),
                    ["<"] = new(TokenType.Lt),
                    ["<="] = new(TokenType.Lte),
                    [">>"] = new(TokenType.RShift),
                    ["<<"] = new(TokenType.LShift),
                    [">="] = new(TokenType.Gte),
                    [":="] = new(TokenType.Equals),
                    ["=/"] = new(TokenType.NotEquals),
                    ["=="] = new(TokenType.BoolEq),
                    ["~"] = new(TokenType.Not),
                    ["and"] = new(TokenType.And),
                    ["or"] = new(TokenType.Or),
                    ["true"] = new(TokenType.True),
                    ["false"] = new(TokenType.False),
                    ["sizeof"] = new(TokenType.Size),

                    //types
                    ["int"] = new(TokenType.Int),
                    ["float"] = new(TokenType.Float),
                    ["char"] = new(TokenType.Char),
                    ["bool"] = new(TokenType.Bool),
                    ["int16"] = new(TokenType.Int16),
                    ["uint16"] = new(TokenType.Uint16),
                    ["uint"] = new(TokenType.Uint),
                    ["uint64"] = new(TokenType.Ulong),
                    ["ubyte"] = new(TokenType.Byte),
                    ["byte"] = new(TokenType.Sbyte),


                    ["int64"] = new(TokenType.Int64),
                    ["string"] = new(TokenType.String),
                    ["struct"] = new(TokenType.Struct),

                    //attributes
                    ["extern"] = new(TokenType.Extern),
                    ["pub"] = new(TokenType.Pub),
                    ["const"] = new(TokenType.Const),

                    //delims 
                    ["{"] = new(TokenType.Begin),
                    ["}"] = new(TokenType.End),
                    ["["] = new(TokenType.OpBracket),
                    ["]"] = new(TokenType.ClBracket),

                    [";"] = new(TokenType.Eol),
                    [","] = new(TokenType.Comma),
                    [":"] = new(TokenType.Colon),

                    //keywords
                    ["return"] = new(TokenType.Return),
                    ["returns"] = new(TokenType.Returns),
                    ["fn"] = new(TokenType.Function),

                    ["if"] = new(TokenType.If),
                    ["else"] = new(TokenType.Else),

                    ["for"] = new(TokenType.For),
                    ["depends"] = new(TokenType.Depends),

                    ["while"] = new(TokenType.While),
                    ["module"] = new(TokenType.Mod),
                    ["Array"] = new(TokenType.Array),
                };
            if (double.TryParse(buffer.ToString(), out _))
            {
                tokens.Add(new(TokenType.Number, buffer.ToString(), lineNumber));
            }
            else if (keyWords.ContainsKey(buffer.ToString()))
            {
                Tokens a = keyWords[buffer.ToString()];
                a.SetLine(lineNumber);
                tokens.Add(a);
            }
            else
            {
                tokens.Add(new(TokenType.Word, buffer.ToString(), lineNumber));
            }

            buffer.Clear();
        }


        private void Operand(
            string currentChar,
            List<Tokens> tokens,
            StringBuilder buffer,
            int lineNumber
        )
        {
            if (buffer.Length != 0)
            {
                groupings(tokens, buffer, lineNumber);
            }

            buffer.Append(currentChar);
            if (currentChar is "(" or ")" or "[" or "]"
                or "~")
            {
                groupings(tokens, buffer, lineNumber);
            }

            CurrentState = State.NumberState;
        }

        private void Number(
            string currentChar,
            List<Tokens> tokens,
            StringBuilder buffer,
            // ref int state,
            int lineNumber
        )
        {
            // buffer += currentChar;
            if (currentChar == "-" && buffer.Length == 0)
            {
                buffer.Append(currentChar);
            }
            else if (currentChar is "=" or "<" or ">")
            {
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer, lineNumber);
                }

                CurrentState = State.EqualsState;
                // state = 3;
                buffer.Append(currentChar);
            }
            else if (currentChar == ":")
            {
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer, lineNumber);
                }

                CurrentState = State.EqualsState;
                buffer.Append(currentChar);
            }
            else if (
                // currentChar == ":"
                currentChar == ";"
                || currentChar == "{"
                || currentChar == "}"
                || currentChar == ","
            )
            {
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer, lineNumber);
                }

                buffer.Append(currentChar);
                groupings(tokens, buffer, lineNumber);
            }
            else if (
                currentChar is "+" or "-" or "/" or "*" or "%" or "^" or "~"
            )
            {
                CurrentState = State.OperationState;
                // state = 2;
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer, lineNumber);
                }

                buffer.Append(currentChar);
            }
            else if (currentChar is "(" or ")"
                     or "[" or "]")
            {
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer, lineNumber);
                }

                buffer.Append(currentChar);
                groupings(tokens, buffer, lineNumber);
            }
            else if (currentChar == ".")
            {
                CurrentState = State.DotState;
            }
            else
            {
                buffer.Append(currentChar);
            }
        }

        private void Equals(
            string currentChar,
            List<Tokens> tokens,
            StringBuilder buffer,
            int lineNumber
        )
        {
            if (currentChar == "=")
            {
                buffer.Append(currentChar);
                if (buffer.Length != 0)
                    groupings(tokens, buffer, lineNumber);
            }
            else if (currentChar is ">" or "<" or "/")
            {
                buffer.Append(currentChar);
            }
            else
            {
                if (buffer.Length != 0)
                    groupings(tokens, buffer, lineNumber);
                buffer.Append(currentChar);
                if (currentChar == "~" || currentChar == "(")
                {
                    groupings(tokens, buffer, lineNumber);
                }

                CurrentState = State.NumberState;
                // state = 1;
            }
        }

        public void DotState(string currentChar,
            List<Tokens> tokens,
            StringBuilder buffer,
            // ref int state,
            int lineNumber)
        {
            if (char.IsNumber(currentChar, 0))
            {
                buffer.Append(".");
                buffer.Append(currentChar);
            }
            else
            {
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer, lineNumber);
                }

                tokens.Add(new Tokens(TokenType.Dot, ".", lineNumber));
                buffer.Append(currentChar);
            }

            CurrentState = State.NumberState;
        }

        public void LexString(string line, List<Tokens> Tokens,
            int lineNumber, ref bool multiLineComments)
        {
            bool isString = false;
            StringBuilder buffer = new();
            for (var nextToken = 0; nextToken < line.Length; nextToken++)
            {
                string currentToken = line[nextToken].ToString();

                if (currentToken == "#")
                {
                    if (buffer.Length != 0)
                    {
                        groupings(Tokens, buffer, lineNumber);
                    }

                    break;
                }

                if (multiLineComments)
                {
                    if (nextToken >= 1)
                    {
                        if (line[nextToken - 1] == '*' && line[nextToken] == ')')
                            multiLineComments = false;
                    }

                    continue;
                }

                if (line[nextToken] == '(' && line[nextToken + 1] == '*')
                {
                    if (buffer.Length != 0)
                    {
                        groupings(Tokens, buffer, lineNumber);
                    }

                    multiLineComments = true;
                    continue;
                }

                if (currentToken is "\'" or "\"")
                {
                    if (buffer.Length != 0)
                    {
                        switch (isString)
                        {
                            case true when currentToken == "\'":
                                Tokens.Add(
                                    new Tokens(TokenType.CharLiteral, buffer.ToString(), lineNumber)
                                );
                                buffer.Clear();
                                break;
                            case true when currentToken == "\"":
                                Tokens.Add(
                                    new Tokens(TokenType.StringLiteral, buffer.ToString(), lineNumber)
                                );
                                buffer.Clear();
                                break;
                            default:
                                groupings(Tokens, buffer, lineNumber);
                                break;
                        }
                    }

                    isString = !isString;
                    continue;
                }

                if (isString)
                {
                    buffer.Append(currentToken);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(currentToken))
                {
                    if (buffer.Length != 0)
                    {
                        groupings(Tokens, buffer, lineNumber);
                    }

                    continue;
                }

                switch (CurrentState)
                {
                    case State.NumberState:
                        Number(currentToken, Tokens, buffer, lineNumber);
                        break;
                    case State.OperationState:
                        Operand(currentToken, Tokens, buffer, lineNumber);
                        break;
                    case State.EqualsState:
                        Equals(currentToken, Tokens, buffer, lineNumber);
                        break;
                    case State.DotState:
                        DotState(currentToken, Tokens, buffer, lineNumber);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (buffer.Length != 0)
            {
                groupings(Tokens, buffer, lineNumber);
            }
        }

        public void LexList(string[] Lines, List<Tokens> Tokens)
        {
            StringBuilder buffer = new();
            bool isString = false;
            bool multiLineComments = false;
            int lineNumber = 0;
            Lines.ToList().ForEach(n => { LexString(n, Tokens, ++lineNumber, ref multiLineComments); });
            if (buffer.Length != 0)
            {
                groupings(Tokens, buffer, Lines.Length);
            }
        }
    }
}