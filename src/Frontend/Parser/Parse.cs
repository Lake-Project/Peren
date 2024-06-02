using System;
using System.Text.RegularExpressions;
using LacusLLVM.Frontend.Parser.AST;
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
        // foreach (Tokens token in TokenList)
        // {
        //     Console.WriteLine(token.ToString());
        // }
        // Console.WriteLine("type: " + type);

        // Console.WriteLine("");

        if (TokenList.Count == 0)
        {
            return null;
        }
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
            if (Current.buffer.Contains("."))
                return new FloatNode(Current);
            return new IntegerNode(Current, LLVMTypeRef.Int32);
        }
        else if (MatchAndRemove(TokenType.CHAR_LITERAL) != null)
        {
            return new IntegerNode(Current.buffer.ToCharArray()[0], LLVMTypeRef.Int8);
        }
        else if (MatchAndRemove(TokenType.TRUE) != null)
        {
            return new IntegerNode(1, LLVMTypeRef.Int1);
        }
        else if (MatchAndRemove(TokenType.FALSE) != null)
        {
            return new IntegerNode(0, LLVMTypeRef.Int1);
        }
        else if (MatchAndRemove(TokenType.WORD) != null)
        {
            if (LookAhead(TokenType.OP_PAREN))
                return ParseFunctionCalls();
            return new VaraibleReferenceNode(Current);
        }
        else if (MatchAndRemove(TokenType.OP_PAREN) != null)
        {
            INode? a = Expression();
            MatchAndRemove(TokenType.CL_PAREN);
            return a;
        }
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
                    : (MatchAndRemove(TokenType.MODULAS) != null)
                        ? Current
                        : null;
        if (opNode == null && op != null)
            throw new Exception("unauthorized statement");
        while (op != null)
        {
            INode? right = Factor();
            if (right == null && op != null)
                throw new Exception("unauthorized statement");
            opNode = new OpNode(opNode, right, op.Value);
            op =
                (MatchAndRemove(TokenType.MULTIPLICATION) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.DIVISION) != null)
                        ? Current
                        : (MatchAndRemove(TokenType.MODULAS) != null)
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
        if (opNode == null && op != null)
            throw new Exception("unauthorized statement");
        while (op != null)
        {
            INode? right = Term();
            if (right == null && op != null)
                throw new Exception("unauthorized statement");
            opNode = new OpNode(opNode, right, op.Value);
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
            TokenType.CHAR => LLVMTypeRef.Int8,
            TokenType.BOOL => LLVMTypeRef.Int1,
            _ => LLVMTypeRef.Void
        };
    }

    public INode Statemnts()
    {
        if (this.GetTokenType() != null && !LookAhead(TokenType.EQUALS))
            return ParseVar();
        else if (Current.tokenType == TokenType.WORD)
            return ParseWordType();
        else
            throw new Exception("Statement invalid " + Current.ToString());
    }

    public INode ParseFunctionCalls()
    {
        Tokens name = Current;
        Tokens? a = MatchAndRemove(TokenType.OP_PAREN) ?? throw new Exception("");
        List<INode> expr = new List<INode>();
        while (MatchAndRemove(TokenType.CL_PAREN) == null)
        {
            expr.Add(Expression() ?? throw new Exception($"empty param on line {name.GetLine()}"));
            MatchAndRemove(TokenType.COMMA);
        }

        return new FunctionCallNode(name, expr);
    }

    public INode ParseVarRef()
    {
        Tokens? name = Current;
        Tokens? e =
            MatchAndRemove(TokenType.EQUALS)
            ?? throw new Exception($"invalid equals on Line {name.Value.GetLine()}");
        return new VaraibleReferenceStatementNode(
            name.Value,
            Expression() ?? throw new Exception($"poor refrence on line {name.Value.GetLine()}")
        );
    }

    public INode ParseWordType()
    {
        if (LookAhead(TokenType.EQUALS))
            return ParseVarRef();
        else if (LookAhead(TokenType.OP_PAREN))
            return ParseFunctionCalls();
        else
            throw new Exception("invalid identifier statement");
    }

    public INode ParseVar()
    {
        LLVMTypeRef type = TokenToLLVMType(Current.tokenType);
        bool isExtern = MatchAndRemove(TokenType.EXTERN) != null;
        Tokens? name = MatchAndRemove(TokenType.WORD) ?? throw new Exception("invalid type");
        Tokens? e = MatchAndRemove(TokenType.EQUALS);
        if (e != null)
            return new VaraibleDeclarationNode(type, name.Value, Expression(), isExtern);
        else
            return new VaraibleDeclarationNode(type, name.Value, null, isExtern);
    }

    public INode? GlobalStatements()
    {
        if (MatchAndRemove(TokenType.FUNCTION) != null)
            return PaseFunction();
        else if (MatchAndRemove(TokenType.STRUCT) != null)
            return ParseStructs();
        else if (GetTokenType() != null && !LookAhead(TokenType.EQUALS))
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
        bool isExtern = false;
        if (MatchAndRemove(TokenType.EXTERN) != null)
            isExtern = true;
        Tokens name = MatchAndRemove(TokenType.WORD) ?? throw new Exception();
        List<INode?> statements = new List<INode?>();

        MatchAndRemove(TokenType.OP_PAREN);
        List<VaraibleDeclarationNode> param = new List<VaraibleDeclarationNode>();

        while (MatchAndRemove(TokenType.CL_PAREN) == null)
        {
            GetTokenType();
            param.Add((VaraibleDeclarationNode)ParseVar());
            MatchAndRemove(TokenType.COMMA);
        }
        LLVMTypeRef returnType = LLVMTypeRef.Void;
        if (MatchAndRemove(TokenType.RETURNS) != null)
        {
            Tokens? type = GetTokenType() ?? throw new Exception("inavlid retrun");
            returnType = TokenToLLVMType(type.Value.tokenType);
        }
        if (MatchAndRemove(TokenType.BEGIN) != null)
            statements = ParseBlock();
        return new FunctionNode(name, param, returnType, statements, isExtern);
    }

    public List<INode?> ParseBlock()
    {
        List<INode?> statements = new();
        while (MatchAndRemove(TokenType.END) == null && MatchAndRemove(TokenType.RETURN) == null)
        {
            statements.Add(Statemnts());
            MatchAndRemove(TokenType.EOL);
        }

        if (Current.tokenType == TokenType.RETURN)
        {
            statements.Add(new ReturnNode(Expression()));
            while (MatchAndRemove(TokenType.END) == null)
                TokenList.RemoveAt(0);
        }
        return statements;
    }

    public List<StatementNode> ParseFile()
    {
        List<INode?> a = new List<INode?>();
        while (TokenList.Count != 0)
        {
            a.Add(GlobalStatements());
            MatchAndRemove(TokenType.EOL);
        }
        return a;
    }
}
