using Lexxer;
public class Parse
{
	public List<Tokens> TokenList;
#pragma warning disable IDE0051 // Remove unused private members
	private Tokens Current;

	public Parse(List<Tokens> tokenList)
	{
		this.TokenList = tokenList;
	}

	private Tokens? MatchAndRemove(TokenType type)
	{
		Current = new Tokens();
		return null;
	}
#pragma warning restore IDE0051 // Remove unused private members


}