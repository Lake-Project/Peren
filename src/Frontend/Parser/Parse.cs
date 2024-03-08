using Lexxer;
public class Parse
{
	public List<Tokens> TokenList;
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

}