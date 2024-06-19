using System.Text;

namespace Lexxer
{
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
        INT64
    }

    public struct Tokens(TokenType tokenType, string buffer, int number)
    {
        public TokenType tokenType = tokenType;
        public string buffer = buffer;

        public Tokens(TokenType tokenType)
            : this(tokenType, "", 0) { }

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
        private void groupings(List<Tokens> tokens, StringBuilder buffer, int lineNumber)
        {
            Dictionary<string, Tokens> Type =
                new()
                {
                    ["+"] = new(TokenType.ADDITION),
                    ["-"] = new(TokenType.SUBTRACTION),
                    ["*"] = new(TokenType.MULTIPLICATION),
                    ["/"] = new(TokenType.DIVISION),
                    ["%"] = new(TokenType.MODULAS),
                    [")"] = new(TokenType.CL_PAREN),
                    ["("] = new(TokenType.OP_PAREN),
                    ["fn"] = new(TokenType.FUNCTION),
                    ["int"] = new(TokenType.INT),
                    ["float"] = new(TokenType.FLOAT),
                    ["char"] = new(TokenType.CHAR),
                    ["bool"] = new(TokenType.BOOL),
                    ["unsigned"] = new(TokenType.UNSIGNED),
                    ["true"] = new(TokenType.TRUE),
                    ["false"] = new(TokenType.FALSE),
                    ["{"] = new(TokenType.BEGIN),
                    ["}"] = new(TokenType.END),
                    [";"] = new(TokenType.EOL),
                    ["="] = new(TokenType.EQUALS),
                    ["return"] = new(TokenType.RETURN),
                    ["returns"] = new(TokenType.RETURNS),
                    [","] = new(TokenType.COMMA),
                    ["~"] = new(TokenType.NOT),
                    ["extern"] = new(TokenType.EXTERN),
                    ["^"] = new(TokenType.XOR),
                    ["and"] = new(TokenType.AND),
                    ["or"] = new(TokenType.OR),
                    ["|"] = new(TokenType.OR),
                    ["&"] = new(TokenType.AND),
                    [">"] = new(TokenType.GT),
                    ["<"] = new(TokenType.LT),
                    ["<="] = new(TokenType.LTE),
                    [">>"] = new(TokenType.R_SHIFT),
                    ["<<"] = new(TokenType.L_SHIFT),
                    [">="] = new(TokenType.GTE),
                    ["if"] = new(TokenType.IF),
                    ["else"] = new(TokenType.ELSE),
                    ["int16"] = new(TokenType.INT16),
                    ["int64"] = new(TokenType.INT64),

                    ["else"] = new(TokenType.ELSE),

                    ["while"] = new(TokenType.WHILE),
                    ["mod"] = new(TokenType.MOD),
                    ["Array"] = new(TokenType.ARRAY),
                    ["import"] = new(TokenType.IMPORT),
                    ["=="] = new(TokenType.BOOL_EQ),
                    ["const"] = new(TokenType.CONST),
                    ["unsigned"] = new(TokenType.UNSIGNED),
                };
            if (double.TryParse(buffer.ToString(), out _))
            {
                tokens.Add(new(TokenType.NUMBER, buffer.ToString(), lineNumber));
            }
            else if (Type.ContainsKey(buffer.ToString()))
            {
                Tokens a = Type[buffer.ToString()];
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
            ref int state,
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

            state = 1;
        }

        private void Number(
            string currentChar,
            List<Tokens> tokens,
            StringBuilder buffer,
            ref int state,
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

                state = 3;
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
                state = 2;
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
            ref int state,
            int lineNumber
        )
        {
            if (currentChar == "=")
            {
                buffer.Append(currentChar);
            }
            else if (currentChar == ">" || currentChar == "<")
            {
                buffer.Append(currentChar);
            }
            else
            {
                if (buffer.Length != 0)
                    groupings(tokens, buffer, lineNumber);
                buffer.Append(currentChar);
                state = 1;
                return;
            }

            if (buffer.Length != 0)
                groupings(tokens, buffer, lineNumber);
            state = 1;
        }

        public List<Tokens> Lex(string[] Lines)
        {
            List<Tokens> Tokens = new();
            int state = 1;
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

                    if (CurrentToken == "\'")
                    {
                        if (Buffer.Length != 0)
                        {
                            if (isSTring)
                            {
                                Tokens.Add(
                                    new Tokens(TokenType.CHAR_LITERAL, Buffer.ToString(), i)
                                );
                                Buffer.Clear();
                            }
                            else
                            {
                                groupings(Tokens, Buffer, i);
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

                    switch (state)
                    {
                        case 1:
                            Number(CurrentToken, Tokens, Buffer, ref state, i);
                            break;
                        case 2:
                            Operand(CurrentToken, Tokens, Buffer, ref state, i);
                            break;
                        case 3:
                            Equals(CurrentToken, Tokens, Buffer, ref state, i);
                            break;
                    }
                }
            }

            if (Buffer.Length != 0)
            {
                groupings(Tokens, Buffer, Lines.Length);
            }

            return Tokens;
        }
    }
}
