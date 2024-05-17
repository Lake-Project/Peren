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
        CHAR_LITERAL
    }

    public struct Tokens
    {
        public TokenType tokenType;

        // public int lineNumber {get => ; set;};
        private int lineNumber;
        public string buffer;

        public Tokens(TokenType tokenType, string buffer, int lineNumber)
        {
            this.tokenType = tokenType;
            this.lineNumber = lineNumber;
            this.buffer = buffer;
        }

        public Tokens(TokenType tokenType)
        {
            this.buffer = "";
            this.tokenType = tokenType;
        }

        public void setLine(int lineNumber)
        {
            this.lineNumber = lineNumber;
        }

        public int GetLine()
        {
            return lineNumber;
        }

        public override string ToString()
        {
            return "lineNumber: " + lineNumber.ToString() + " " + tokenType + " (" + buffer + ")";
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
                };
            if (double.TryParse(buffer.ToString(), out _))
            {
                tokens.Add(new(TokenType.NUMBER, buffer.ToString(), lineNumber));
            }
            else if (Type.ContainsKey(buffer.ToString()))
            {
                Tokens a = Type[buffer.ToString()];
                a.setLine(lineNumber);
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
            else if (
                currentChar == ":"
                || currentChar == "="
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

        public List<Tokens> Lex(string[] Lines)
        {
            List<Tokens> Tokens = new();
            int state = 1;
            StringBuilder Buffer = new();
            bool isSTring = false;
            for (int i = 0; i < Lines.Length; i++)
            {
                for (int nextToken = 0; nextToken < Lines[i].Length; nextToken++)
                {
                    string CurrentToken = Lines[i][nextToken].ToString();
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

                    if (state == 1)
                    {
                        Number(CurrentToken, Tokens, Buffer, ref state, i);
                    }
                    else if (state == 2)
                    {
                        Operand(CurrentToken, Tokens, Buffer, ref state, i);
                    }
                }
            }
            if (Buffer.Length != 0)
            {
                groupings(Tokens, Buffer, Lines.Length);
            }
            return Tokens;
        }

        public void printList(List<Tokens> tokens)
        {
            foreach (Tokens token in tokens)
            {
                Console.WriteLine(token.ToString());
            }
        }
    }
}
