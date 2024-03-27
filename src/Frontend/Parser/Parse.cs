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
		// Current = new Tokens();
		if (TokenList[0].tokenType == type)
		{
			Current = TokenList[0];
			TokenList.RemoveAt(0);
			return Current;

		}
		return null;
	}
	
	private INode? Factor()
	{
		if (MatchAndRemove(TokenType.NUMBER) != null)
		{
			return new IntegerNode(Current);
		}
		else if (MatchAndRemove(TokenType.OP_PAREN) != null)
		{
			INode? a = Expression();
			MatchAndRemove(TokenType.CL_PAREN);
			return a;
		}
		// else if (MatchAndRemove(TokenType.WORD) != null)
		// {
		// 	return new VaraibleReferenceNode(Current);
		// }

		return null;
	}
	private INode? Term()
	{
		INode? opNode = Factor(); //returns a mathOPNode.

		Tokens? op = (MatchAndRemove(TokenType.MULTIPLICATION) != null) ? Current
			 		: (MatchAndRemove(TokenType.DIVISION) != null) ? Current
			  								: null;

		while (op != null)
		{
			opNode = new OpNode(opNode, Factor(), (Tokens)op);

			op = (MatchAndRemove(TokenType.MULTIPLICATION) != null) ? Current
				: (MatchAndRemove(TokenType.DIVISION) != null) ? Current																			: null;
		}

		return opNode;
	}
	private INode? Expression()
	{
		INode? opNode = Term(); //returns a mathOPNode.

		Tokens? op = (MatchAndRemove(TokenType.ADDITION) != null) ? Current
			 		: (MatchAndRemove(TokenType.SUBTRACTION) != null) ? Current
			  									: null;

		while (op != null)
		{
			opNode = new OpNode(opNode, Term(), (Tokens)op);

			op = (MatchAndRemove(TokenType.ADDITION) != null) ? Current
								: (MatchAndRemove(TokenType.SUBTRACTION) != null) ? Current
														: null;

		}
		return opNode;
	}


}