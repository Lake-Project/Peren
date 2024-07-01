using System.Text;

namespace Lexxer
{
    public enum State
    {
        NumberState,
        OperationState,
        EqualsState,
        DotState
    }

    public enum TokenType
    {
        ADDITION,
        SUBTRACTION,
        NUMBER,
        DIVISION,
        MULTIPLICATION,
        MODULAS,
        OP_PAREN,
        CL_PAREN,
        WORD,
        FUNCTION,
        INT,
        FLOAT,
        BEGIN,
        END,
        CHAR,
        EOL,
        EQUALS,
        RETURN,
        RETURNS,
        STRUCT,
        BOOL,
        COMMA,
        NOT,
        XOR,
        EXTERN,
        UNSIGNED,
        TRUE,
        FALSE,
        CHAR_LITERAL,
        BOOL_EQ,
        LT,
        GT,
        GTE,
        LTE,
        R_SHIFT,
        L_SHIFT,
        AND,
        OR,
        VOID,
        IF,
        WHILE,
        MOD,
        ARRAY,
        IMPORT,
        ELSE,
        CONST,
        INT16,
        INT64,
        FOR,
        DEPENDS,
        PUB,
        DOT,
        STRING_LITERAL,
        STRING,
        SIZE,
        NOT_EQUALS
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
                    ["+"] = new(TokenType.ADDITION),
                    ["-"] = new(TokenType.SUBTRACTION),
                    ["*"] = new(TokenType.MULTIPLICATION),
                    ["/"] = new(TokenType.DIVISION),
                    ["%"] = new(TokenType.MODULAS),
                    [")"] = new(TokenType.CL_PAREN),
                    ["("] = new(TokenType.OP_PAREN),
                    ["^"] = new(TokenType.XOR),
                    ["."] = new(TokenType.DOT),
                    ["|"] = new(TokenType.OR),
                    ["&"] = new(TokenType.AND),
                    [">"] = new(TokenType.GT),
                    ["<"] = new(TokenType.LT),
                    ["<="] = new(TokenType.LTE),
                    [">>"] = new(TokenType.R_SHIFT),
                    ["<<"] = new(TokenType.L_SHIFT),
                    [">="] = new(TokenType.GTE),
                    ["="] = new(TokenType.EQUALS),
                    ["=/"] = new(TokenType.NOT_EQUALS),
                    ["=="] = new(TokenType.BOOL_EQ),
                    ["~"] = new(TokenType.NOT),
                    ["and"] = new(TokenType.AND),
                    ["or"] = new(TokenType.OR),
                    ["true"] = new(TokenType.TRUE),
                    ["false"] = new(TokenType.FALSE),
                    ["sizeof"] = new(TokenType.SIZE),

                    //types
                    ["int"] = new(TokenType.INT),
                    ["float"] = new(TokenType.FLOAT),
                    ["char"] = new(TokenType.CHAR),
                    ["bool"] = new(TokenType.BOOL),
                    ["int16"] = new(TokenType.INT16),
                    ["int64"] = new(TokenType.INT64),
                    ["string"] = new(TokenType.STRING),
                    ["struct"] = new(TokenType.STRUCT),

                    //attributes
                    ["unsigned"] = new(TokenType.UNSIGNED),
                    ["extern"] = new(TokenType.EXTERN),
                    ["pub"] = new(TokenType.PUB),
                    ["const"] = new(TokenType.CONST),

                    //delims 
                    ["{"] = new(TokenType.BEGIN),
                    ["}"] = new(TokenType.END),
                    [";"] = new(TokenType.EOL),
                    [","] = new(TokenType.COMMA),


                    //keywords
                    ["return"] = new(TokenType.RETURN),
                    ["returns"] = new(TokenType.RETURNS),
                    ["fn"] = new(TokenType.FUNCTION),

                    ["if"] = new(TokenType.IF),
                    ["else"] = new(TokenType.ELSE),

                    ["for"] = new(TokenType.FOR),
                    ["depends"] = new(TokenType.DEPENDS),

                    ["while"] = new(TokenType.WHILE),
                    ["mod"] = new(TokenType.MOD),
                    ["Array"] = new(TokenType.ARRAY),
                    ["import"] = new(TokenType.IMPORT),
                };
            if (double.TryParse(buffer.ToString(), out _))
            {
                tokens.Add(new(TokenType.NUMBER, buffer.ToString(), lineNumber));
            }
            else if (keyWords.ContainsKey(buffer.ToString()))
            {
                Tokens a = keyWords[buffer.ToString()];
                a.SetLine(lineNumber);
                tokens.Add(a);
            }
            else
            {
                tokens.Add(new(TokenType.WORD, buffer.ToString(), lineNumber));
            }

            buffer.Clear();
        }


        private void Operand(
            string currentChar,
            List<Tokens> tokens,
            StringBuilder buffer,
            // ref int state,
            int lineNumber
        )
        {
            if (buffer.Length != 0)
            {
                groupings(tokens, buffer, lineNumber);
            }

            buffer.Append(currentChar);
            if (currentChar == "(" || currentChar == ")")
            {
                groupings(tokens, buffer, lineNumber);
            }

            CurrentState = State.NumberState;

            // state = 1;
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
            else if (currentChar == "=" || currentChar == "<" || currentChar == ">")
            {
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer, lineNumber);
                }

                CurrentState = State.EqualsState;
                // state = 3;
                buffer.Append(currentChar);
            }
            else if (
                currentChar == ":"
                || currentChar == ";"
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
                currentChar == "+"
                || currentChar == "-"
                || currentChar == "/"
                || currentChar == "*"
                || currentChar == "%"
                || currentChar == "^"
                || currentChar == "~"
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
            else if (currentChar == "(" || currentChar == ")")
            {
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer, lineNumber);
                }

                buffer.Append(currentChar);
                groupings(tokens, buffer, lineNumber);
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
            // ref int state,
            int lineNumber
        )
        {
            if (currentChar == "=")
            {
                buffer.Append(currentChar);
            }
            else if (currentChar == ">" || currentChar == "<" || currentChar == "/")
            {
                buffer.Append(currentChar);
            }
            else
            {
                if (buffer.Length != 0)
                    groupings(tokens, buffer, lineNumber);
                buffer.Append(currentChar);
                CurrentState = State.NumberState;
                // state = 1;
                return;
            }

            if (buffer.Length != 0)
                groupings(tokens, buffer, lineNumber);
            // state = 1;
        }

        public void Lex(string[] Lines, List<Tokens> Tokens)
        {
            

            StringBuilder Buffer = new();
            bool isSTring = false;
            bool multiLineComments = false;

            for (int i = 0; i < Lines.Length; i++)
            {
                for (int nextToken = 0; nextToken < Lines[i].Length; nextToken++)
                {
                    string CurrentToken = Lines[i][nextToken].ToString();
                    if (CurrentToken == "#")
                    {
                        if (Buffer.Length != 0)
                        {
                            groupings(Tokens, Buffer, i);
                        }

                        break;
                    }

                    if (multiLineComments)
                    {
                        if (nextToken >= 1)
                        {
                            if (Lines[i][nextToken - 1] == '*' && Lines[i][nextToken] == ')')
                                multiLineComments = false;
                        }

                        continue;
                    }

                    if (Lines[i][nextToken] == '(' && Lines[i][nextToken + 1] == '*')
                    {
                        if (Buffer.Length != 0)
                        {
                            groupings(Tokens, Buffer, i);
                        }

                        multiLineComments = true;
                        continue;
                    }

                    if (CurrentToken is "\'" or "\"")
                    {
                        if (Buffer.Length != 0)
                        {
                            switch (isSTring)
                            {
                                case true when CurrentToken == "\'":
                                    Tokens.Add(
                                        new Tokens(TokenType.CHAR_LITERAL, Buffer.ToString(), i)
                                    );
                                    Buffer.Clear();
                                    break;
                                case true when CurrentToken == "\"":
                                    Tokens.Add(
                                        new Tokens(TokenType.STRING_LITERAL, Buffer.ToString(), i)
                                    );
                                    Buffer.Clear();
                                    break;
                                default:
                                    groupings(Tokens, Buffer, i);
                                    break;
                            }
                        }

                        isSTring = !isSTring;
                        continue;
                    }

                    if (isSTring)
                    {
                        Buffer.Append(CurrentToken);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(CurrentToken))
                    {
                        if (Buffer.Length != 0)
                        {
                            groupings(Tokens, Buffer, i);
                        }

                        continue;
                    }

                    switch (CurrentState)
                    {
                        case State.NumberState:
                            Number(CurrentToken, Tokens, Buffer, i);
                            break;
                        case State.OperationState:
                            Operand(CurrentToken, Tokens, Buffer, i);
                            break;
                        case State.EqualsState:
                            Equals(CurrentToken, Tokens, Buffer, i);
                            break;
                    }
                }
            }

            if (Buffer.Length != 0)
            {
                groupings(Tokens, Buffer, Lines.Length);
            }

        }
    }
}