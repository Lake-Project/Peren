using System;
using System.Text.RegularExpressions;
using LacusLLVM.Frontend.Parser.AST;
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
            return new IntegerNode(Current);
        }
        else if (MatchAndRemove(TokenType.CHAR_LITERAL) != null)
        {
            return new CharNode(char.Parse(Current.buffer));
        }
        else if (MatchAndRemove(TokenType.TRUE) != null)
        {
            return new BoolNode(true);
        }
        else if (MatchAndRemove(TokenType.FALSE) != null)
        {
            return new BoolNode(false);
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

    private INode? BoolExpr()
    {
        INode? opNode = Factor(); //returns a mathOPNode.
        Tokens? op =
            (MatchAndRemove(TokenType.GT) != null)
                ? Current
                : (MatchAndRemove(TokenType.LT) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.BOOL_EQ) != null)
                        ? Current
                        : (MatchAndRemove(TokenType.LTE) != null)
                            ? Current
                            : (MatchAndRemove(TokenType.GTE) != null)
                                ? Current
                                : null;
        if (op != null)
        {
            INode? right = Factor();
            opNode = new BooleanExprNode(opNode, right, op.Value);
        }

        return opNode;
    }

    private INode? Term()
    {
        INode? opNode = BoolExpr(); //returns a mathOPNode.

        Tokens? op =
            (MatchAndRemove(TokenType.MULTIPLICATION) != null)
                ? Current
                : (MatchAndRemove(TokenType.DIVISION) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.MODULAS) != null)
                        ? Current
                        : MatchAndRemove(TokenType.AND) != null
                            ? Current
                            : MatchAndRemove(TokenType.OR) != null
                                ? Current
                                : MatchAndRemove(TokenType.L_SHIFT) != null
                                    ? Current
                                    : MatchAndRemove(TokenType.R_SHIFT) != null
                                        ? Current
                                        : null;
        if (opNode == null && op != null)
            throw new Exception("unauthorized statement");
        while (op != null)
        {
            INode? right = BoolExpr();
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
                            : MatchAndRemove(TokenType.AND) != null
                                ? Current
                                : MatchAndRemove(TokenType.OR) != null
                                    ? Current
                                    : MatchAndRemove(TokenType.L_SHIFT) != null
                                        ? Current
                                        : MatchAndRemove(TokenType.R_SHIFT) != null
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

    public ElseNode ParseElse()
    {
        List<StatementNode> statementNodes = new();
        if (MatchAndRemove(TokenType.ELSE) != null)
        {
            statementNodes = ParseBlock();
        }

        return new ElseNode(statementNodes);
    }

    public StatementNode ParseIf()
    {
        MatchAndRemove(TokenType.OP_PAREN);
        INode expr = Expression() ?? throw new Exception($"null expr in if {Current.GetLine()} ");
        MatchAndRemove(TokenType.CL_PAREN);
        List<StatementNode> statementNodes = ParseBlock();
        return new IfNode(expr, ParseElse(), statementNodes);
    }

    public StatementNode Statemnts()
    {
        if (
            this.GetTokenType() != null
            && (!LookAhead(TokenType.EQUALS) && !LookAhead(TokenType.OP_PAREN))
        )
            return ParseVar();
        else if (Current.tokenType == TokenType.WORD)
            return ParseWordType();
        else if (MatchAndRemove(TokenType.IF) != null)
            return ParseIf();
        else
            throw new Exception("Statement invalid " + Current.ToString());
    }

    public FunctionCallNode ParseFunctionCalls()
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

    public VaraibleReferenceStatementNode ParseVarRef()
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

    public StatementNode ParseWordType()
    {
        if (LookAhead(TokenType.EQUALS))
            return ParseVarRef();
        else if (LookAhead(TokenType.OP_PAREN))
            return ParseFunctionCalls();
        else
            throw new Exception($"invalid identifier statement {Current.ToString()}");
    }

    public VaraibleDeclarationNode ParseVar()
    {
        Tokens Type = Current;
        bool isExtern = MatchAndRemove(TokenType.EXTERN) != null;
        Tokens? name = MatchAndRemove(TokenType.WORD) ?? throw new Exception("invalid type");
        Tokens? e = MatchAndRemove(TokenType.EQUALS);

        if (e != null)
            return new VaraibleDeclarationNode(Type, name.Value, Expression(), isExtern);
        else
            return new VaraibleDeclarationNode(Type, name.Value, null, isExtern);
    }

    public StatementNode GlobalStatements()
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

    public StatementNode ParseStructs()
    {
        return null;
    }

    public FunctionNode PaseFunction()
    {
        bool isExtern = MatchAndRemove(TokenType.EXTERN) != null;
        Tokens name = MatchAndRemove(TokenType.WORD) ?? throw new Exception();
        List<StatementNode> statements = new List<StatementNode>();

        MatchAndRemove(TokenType.OP_PAREN);
        List<VaraibleDeclarationNode> param = new List<VaraibleDeclarationNode>();

        while (MatchAndRemove(TokenType.CL_PAREN) == null)
        {
            GetTokenType();
            param.Add(ParseVar());
            MatchAndRemove(TokenType.COMMA);
        }

        Tokens type = new Tokens(TokenType.VOID);
        if (MatchAndRemove(TokenType.RETURNS) != null)
        {
            type = GetTokenType() ?? throw new Exception("inavlid retrun");
        }

        statements = ParseBlock();
        return new FunctionNode(name, param, type, statements, isExtern);
    }

    public List<StatementNode> ParseBlock()
    {
        List<StatementNode> statements = new();
        if (MatchAndRemove(TokenType.BEGIN) != null)
        {
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
        }
        else
        {
            if (MatchAndRemove(TokenType.RETURN) == null)
                statements.Add(Statemnts());
            else
                statements.Add(new ReturnNode(Expression()));
        }
        return statements;
    }

    public List<StatementNode> ParseFile()
    {
        List<StatementNode> a = new List<StatementNode>();
        while (TokenList.Count != 0)
        {
            a.Add(GlobalStatements());
            MatchAndRemove(TokenType.EOL);
        }

        return a;
    }
}