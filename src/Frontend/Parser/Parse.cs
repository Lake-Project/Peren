using Lexxer;
using LLVMSharp.Interop;
using System;
using System.Text.RegularExpressions;

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

    public bool LookAhead(TokenType type)
    {
        if (TokenList[0].tokenType == type)
        {
            return true;
        }
        return false;
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

        Tokens? op =
            (MatchAndRemove(TokenType.MULTIPLICATION) != null)
                ? Current
                : (MatchAndRemove(TokenType.DIVISION) != null)
                    ? Current
                    : null;

        while (op != null)
        {
            opNode = new OpNode(opNode, Factor(), (Tokens)op);

            op =
                (MatchAndRemove(TokenType.MULTIPLICATION) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.DIVISION) != null)
                        ? Current
                        : null;
        }

        return opNode;
    }

    private INode? Expression()
    {
        INode? opNode = Term();

        Tokens? op =
            (MatchAndRemove(TokenType.ADDITION) != null)
                ? Current
                : (MatchAndRemove(TokenType.SUBTRACTION) != null)
                    ? Current
                    : null;

        while (op != null)
        {
            opNode = new OpNode(opNode, Term(), (Tokens)op);

            op =
                (MatchAndRemove(TokenType.ADDITION) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.SUBTRACTION) != null)
                        ? Current
                        : null;
        }
        return opNode;
    }

    public Tokens? GetTokenType()
    {
        return (MatchAndRemove(TokenType.FLOAT) != null)
          ? Current
          : (MatchAndRemove(TokenType.INT) != null)
              ? Current
              : (MatchAndRemove(TokenType.BOOL) != null)
                  ? Current
                  : (MatchAndRemove(TokenType.CHAR) != null)
                      ? Current
                      : (MatchAndRemove(TokenType.WORD) != null)
                          ? Current
                          : null;
    }

    public LLVMTypeRef TokenToLLVMType(TokenType type)
    {
        return type switch
        {
            TokenType.INT => LLVMTypeRef.Int32,
            TokenType.FLOAT => LLVMTypeRef.Float,
            // TokenType.WORD => LLVMTypeRef
            _ => LLVMTypeRef.Void
        };
    }

    public INode? Statemnts()
    {
        if (GetType() != null && !LookAhead(TokenType.EQUALS))
            return ParseVar();
        else if (Current.tokenType == TokenType.WORD)
            return ParseWordType();
        else
            throw new Exception("Statement invalid");
    }

    public INode? ParseFunctionCalls()
    {
        return null;
    }

    public INode? ParseVarRef()
    {
        Tokens? name = Current;
        Tokens? e = MatchAndRemove(TokenType.EQUALS) ?? throw new Exception("invalid equals");
        return new VaraibleReferenceStatementNode(
            name.Value.buffer,
            Expression() ?? throw new Exception("poor refrence")
        );
    }

    public INode? ParseWordType()
    {
        if (LookAhead(TokenType.EQUALS))
            return ParseVarRef();
        else if (LookAhead(TokenType.OP_PAREN))
            return ParseFunctionCalls();
        return null;
    }

    public INode? ParseVar()
    {
        LLVMTypeRef type = TokenToLLVMType(Current.tokenType);
        Tokens? name = MatchAndRemove(TokenType.WORD) ?? throw new Exception("invalud type");
        Tokens? e = MatchAndRemove(TokenType.EQUALS) ?? throw new Exception("invalid equals");
        return new VaraibleDeclarationNode(type, name.Value.buffer, Expression());
    }

    public INode? GlobalStatements()
    {
        if (MatchAndRemove(TokenType.FUNCTION) != null)
            return PaseFunction();
        else if (MatchAndRemove(TokenType.STRUCT) != null)
            return ParseStructs();
        else if (GetType() != null && !LookAhead(TokenType.EQUALS))
            return ParseVar();
        else
            throw new Exception("Statement invalid");
    }

    public INode? ParseStructs()
    {
        return null;
    }

    public INode? PaseFunction()
    {
        Tokens name = MatchAndRemove(TokenType.WORD) ?? throw new Exception();
        List<INode?> statements = new List<INode?>();
        MatchAndRemove(TokenType.OP_PAREN);
        MatchAndRemove(TokenType.CL_PAREN);

        // LLVMTypeRef a;
        // if (MatchAndRemove(TokenType.RETURNS) != null)
        if (MatchAndRemove(TokenType.BEGIN) != null)
            while (MatchAndRemove(TokenType.END) == null)
                statements.Add(Statemnts());
        return new FunctionNode(name.buffer, LLVMTypeRef.Void, statements);
    }

    public List<INode?> ParseFile()
    {
        List<INode?> a = new List<INode?>();
        while (TokenList.Count != 0)
            a.Add(GlobalStatements());
        return a;
    }
}
