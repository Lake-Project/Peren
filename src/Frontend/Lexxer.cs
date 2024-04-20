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
        XOR
    }

    public struct Tokens
    {
        public TokenType tokenType;
        public string buffer;

        public Tokens(TokenType tokenType, string buffer)
        {
            this.tokenType = tokenType;
            this.buffer = buffer;
        }

        public Tokens(TokenType tokenType)
        {
            this.buffer = "";
            this.tokenType = tokenType;
        }

        public override string ToString()
        {
            return tokenType + " (" + buffer + ")";
        }
    }

    public class LexTokens
    {
        private void groupings(List<Tokens> tokens, StringBuilder buffer)
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
                    ["{"] = new(TokenType.BEGIN),
                    ["}"] = new(TokenType.END),
                    [";"] = new(TokenType.EOL),
                    ["="] = new(TokenType.EQUALS),
                    ["return"] = new(TokenType.RETURN),
                    ["returns"] = new(TokenType.RETURNS),
                    [","] = new(TokenType.COMMA),
                    ["~"] = new(TokenType.NOT),
                    ["^"] = new(TokenType.XOR),
                };
            if (double.TryParse(buffer.ToString(), out _))
            {
                tokens.Add(new(TokenType.NUMBER, buffer.ToString()));
            }
            else if (Type.ContainsKey(buffer.ToString()))
            {
                tokens.Add(Type[buffer.ToString()]);
            }
            else
            {
                tokens.Add(new(TokenType.WORD, buffer.ToString()));
            }

            buffer.Clear();
        }

        private void Operand(
            string currentChar,
            List<Tokens> tokens,
            StringBuilder buffer,
            ref int state
        )
        {
            if (buffer.Length != 0)
            {
                groupings(tokens, buffer);
            }
            buffer.Append(currentChar);
            if (currentChar == "(" || currentChar == ")")
            {
                groupings(tokens, buffer);
            }
            state = 1;
        }

        private void Number(
            string currentChar,
            List<Tokens> tokens,
            StringBuilder buffer,
            ref int state
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
                    groupings(tokens, buffer);
                }
                buffer.Append(currentChar);
                groupings(tokens, buffer);
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
                    groupings(tokens, buffer);
                }
                buffer.Append(currentChar);
            }
            else if (currentChar == "(" || currentChar == ")")
            {
                if (buffer.Length != 0)
                {
                    groupings(tokens, buffer);
                }
                buffer.Append(currentChar);
                groupings(tokens, buffer);
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
            for (int i = 0; i < Lines.Length; i++)
            {
                for (int nextToken = 0; nextToken < Lines[i].Length; nextToken++)
                {
                    string CurrentToken = Lines[i][nextToken].ToString();
                    if (string.IsNullOrWhiteSpace(CurrentToken))
                    {
                        if (Buffer.Length != 0)
                        {
                            groupings(Tokens, Buffer);
                        }
                        continue;
                    }
                    if (state == 1)
                    {
                        Number(CurrentToken, Tokens, Buffer, ref state);
                    }
                    else if (state == 2)
                    {
                        Operand(CurrentToken, Tokens, Buffer, ref state);
                    }
                }
            }
            if (Buffer.Length != 0)
            {
                groupings(Tokens, Buffer);
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
