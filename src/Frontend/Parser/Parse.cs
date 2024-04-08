using System.Text.RegularExpressions;
using Lexxer;
using LLVMSharp.Interop;
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
				: (MatchAndRemove(TokenType.DIVISION) != null) ? Current
				: null;
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
	public INode? PaseFunction()
	{
		Tokens name = MatchAndRemove(TokenType.WORD) ?? throw new Exception();
		List<INode> statements = new List<INode>();
		MatchAndRemove(TokenType.OP_PAREN);
		MatchAndRemove(TokenType.CL_PAREN);

		if (MatchAndRemove(TokenType.RETURNS) != null)
		{


		}

		return new FunctionNode(name.buffer, LLVMTypeRef.Void, statements);

	}
	private LLVMTypeRef TypeToLLVM()
	{
		return Current.tokenType switch
		{
			TokenType.INT => LLVMTypeRef.Int32,
			TokenType.FLOAT => LLVMTypeRef.Float,
			TokenType.CHAR => LLVMTypeRef.Int8,
			_ => LLVMTypeRef.Void
		};
	}
	public INode? ParseVaraible()
	{

		LLVMTypeRef type = TypeToLLVM();
		// if()
		Tokens word = MatchAndRemove(TokenType.WORD) ?? throw new Exception();
		MatchAndRemove(TokenType.EQUALS);
		return new VaraibleDeclarationNode(type, word.buffer, Expression());
	}
	public INode? GlobalStatements()
	{
		if (MatchAndRemove(TokenType.FUNCTION) != null)
		{
			return PaseFunction();
		}
		else if (MatchAndRemove(TokenType.INT) != null || MatchAndRemove(TokenType.FLOAT) != null)
		{
			return ParseVaraible();
		}

		return null;
	}
	private void RemoveEOLS()
	{
		while (MatchAndRemove(TokenType.EOL) != null) ;
	}
	public INode LocalStatements()
	{
		if (MatchAndRemove(TokenType.INT) != null || MatchAndRemove(TokenType.FLOAT) != null)
		{
			return ParseVaraible();
		}

		return null;
	}
	public List<INode> ParseFile()
	{
		List<INode> statements = new List<INode>();
		while (TokenList.Count != 0)
		{
			statements.Add(GlobalStatements());
			RemoveEOLS();
		}
		return statements;
	}


}