namespace Lexxer
{
	public enum TokenType
	{
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
		public List<Tokens> Lex(string[] lines)
		{
			List<Tokens> tokens = new();
			int state = 1;
			for (int i = 0; i < lines.Length; i++)
			{
				if (state == 1)
				{

				}
				else if (state == 2)
				{
				}

			}
			return tokens;
		}

	}
}
