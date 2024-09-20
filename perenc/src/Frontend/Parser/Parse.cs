using System;
using System.Drawing;
using System.Text.RegularExpressions;
using LacusLLVM.Frontend.Parser.AST;
using Lexxer;

public struct AttributesTuple
{
    public bool isPub;
    public bool isExtern;
    public bool isConst;
}

public class Parse
{
    public List<Tokens> TokenList;
    private Tokens Current;

    public Stack<Tokens> attributes = new();

    public Parse(List<Tokens> tokenList)
    {
        this.TokenList = tokenList;
    }

    private Tokens? MatchAndRemove(TokenType type)
    {
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

    public Tokens? MatchAndRemove(TokenType[] tokenTypes)
    {
        if (!TokenList.Any())
            return null;
        if (tokenTypes.ToList().Any(n => n == TokenList[0].tokenType))
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

    private ExpressionNode? Factor()
    {
        if (MatchAndRemove(TokenType.Number) != null)
        {
            if (Current.buffer.Contains("."))
                return new FloatNode(Current);
            return new IntegerNode(Current);
        }
        else if (MatchAndRemove(TokenType.CharLiteral) != null)
        {
            return new CharNode(char.Parse(Current.buffer));
        }
        else if (MatchAndRemove(TokenType.True) != null)
        {
            return new BoolNode(true);
        }
        else if (MatchAndRemove(TokenType.False) != null)
        {
            return new BoolNode(false);
        }
        else if (MatchAndRemove(TokenType.Word) != null)
        {
            if (LookAhead(TokenType.OpParen))
                return ParseFunctionCalls();
            else if (LookAhead(TokenType.OpBracket))
                return ParseArrayRef();
            return new VaraibleReferenceNode(Current);
        }
        else if (MatchAndRemove(TokenType.StringLiteral) != null)
        {
            return new StringNode(Current);
        }
        else if (MatchAndRemove(TokenType.OpParen) != null)
        {
            Tokens? type = GetNativeType();
            if (type != null)
            {
                MatchAndRemove(TokenType.ClParen);

                ExpressionNode? b = Factor();
                return new CastNode(b, type.Value);
            }

            ExpressionNode? a = ParseSingleExpr();
            Console.WriteLine("hai");
            MatchAndRemove(TokenType.ClParen);

            return a;
        }
        else if (MatchAndRemove(TokenType.Not) != null)
        {
            Tokens a = Current;
            ExpressionNode? v = Factor();
            return
                new
                    OpNode(v, v, a);
        }
        else if (MatchAndRemove(TokenType.Size) != null)
        {
            MatchAndRemove(TokenType.OpParen);
            ExpressionNode? v = Expression();
            MatchAndRemove(TokenType.ClParen);
            return new OpNode(v,
                v, new Tokens(TokenType.Size));
        }

        return null;
    }

    private ExpressionNode? BoolExpr()
    {
        ExpressionNode? opNode = Factor();
        Tokens? op =
            (MatchAndRemove(TokenType.Gt) != null)
                ? Current
                : (MatchAndRemove(TokenType.Lt) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.BoolEq) != null)
                        ? Current
                        : (MatchAndRemove(TokenType.Lte) != null)
                            ? Current
                            : (MatchAndRemove(TokenType.Gte) != null)
                                ? Current
                                : (MatchAndRemove(TokenType.NotEquals) != null)
                                    ? Current
                                    : null;
        if (op != null)
        {
            ExpressionNode? right = Factor();
            opNode = new BooleanExprNode(opNode, right, op.Value);
        }


        return opNode;
    }

    public ExpressionNode? ParseNot()
    {
        return BoolExpr();
    }

    private ExpressionNode? Term()
    {
        Tokens? op;
        ExpressionNode? opNode = ParseNot(); //returns a mathOPNode.
        op =
            (MatchAndRemove(TokenType.Multiplication) != null)
                ? Current
                : (MatchAndRemove(TokenType.Division) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.Modulas) != null)
                        ? Current
                        : MatchAndRemove(TokenType.And) != null
                            ? Current
                            : MatchAndRemove(TokenType.Or) != null
                                ? Current
                                : MatchAndRemove(TokenType.LShift) != null
                                    ? Current
                                    : MatchAndRemove(TokenType.RShift) != null
                                        ? Current
                                        : MatchAndRemove(TokenType.Xor) != null
                                            ? Current
                                            : null;
        if (opNode == null && op != null && op.Value.tokenType != TokenType.Not)
            throw new Exception("unauthorized statement");
        while (op != null)
        {
            ExpressionNode? right = ParseNot();

            if (right == null && op != null)
                throw new Exception("unauthorized statement");
            opNode = new OpNode(opNode, right, op.Value);

            op =
                (MatchAndRemove(TokenType.Multiplication) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.Division) != null)
                        ? Current
                        : (MatchAndRemove(TokenType.Modulas) != null)
                            ? Current
                            : MatchAndRemove(TokenType.And) != null
                                ? Current
                                : MatchAndRemove(TokenType.Or) != null
                                    ? Current
                                    : MatchAndRemove(TokenType.LShift) != null
                                        ? Current
                                        : MatchAndRemove(TokenType.RShift) != null
                                            ? Current
                                            : MatchAndRemove(TokenType.Xor) != null
                                                ? Current
                                                : null;
        }

        return opNode;
    }

    private ExpressionNode? Expression()
    {
        ExpressionNode? opNode;
        opNode = Term();

        Tokens? op =
            (MatchAndRemove(TokenType.Addition) != null)
                ? Current
                : (MatchAndRemove(TokenType.Subtraction) != null)
                    ? Current
                    : null;
        if (opNode == null && op != null)
            throw new Exception("unauthorized statement");
        while (op != null)
        {
            ExpressionNode? right = Term();
            if (right == null && op != null)
                throw new Exception("unauthorized statement");
            opNode = new OpNode(opNode, right, op.Value);
            op =
                (MatchAndRemove(TokenType.Addition) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.Subtraction) != null)
                        ? Current
                        : null;
        }

        return opNode;
    }

    private ExpressionNode? ParseSingleExpr()
    {
        return Expression();
    }

    public List<ExpressionNode> ParseTupleAssignment()
    {
        List<ExpressionNode> expr = new List<ExpressionNode>();

        while (MatchAndRemove(TokenType.ClParen) == null)
        {
            expr.Add(
                ParseSingleExpr()
                ?? throw new Exception( //e
                    $"need all types tuple on line {Current.GetLine()}"
                )
            );
            MatchAndRemove(TokenType.Comma);
        }

        return expr;
    }

    public FunctionCallNode ParseFunctionCalls()
    {
        Tokens name = Current;
        Tokens? a =
            MatchAndRemove(TokenType.OpParen) ?? throw new Exception("function is a tuple type");
        List<ExpressionNode> expr = ParseTupleAssignment();
        return new FunctionCallNode(name, expr);
    }

    public ElseNode? ParseElse()
    {
        List<StatementNode> statementNodes = new();
        if (MatchAndRemove(TokenType.Else) != null)
        {
            statementNodes = ParseBlock();
        }

        return new ElseNode(statementNodes);
    }

    public IfNode ParseIf()
    {
        MatchAndRemove(TokenType.OpParen);
        ExpressionNode expr = ParseSingleExpr() ?? throw new Exception($"null expr in if {Current.GetLine()} ");
        MatchAndRemove(TokenType.ClParen);
        List<StatementNode> statementNodes = ParseBlock();
        return new IfNode(expr, ParseElse(), statementNodes);
    }

    public List<StatementNode> ParseBlock()
    {
        List<StatementNode> statements = new();
        if (MatchAndRemove(TokenType.Begin) != null)
        {
            while (
                MatchAndRemove(TokenType.End) == null
            )
            {
                statements.Add(Statements());
                MatchAndRemove(TokenType.Eol);
            }

            // if (Current.tokenType == TokenType.RETURN)
            // {
            //     statements.Add(new ReturnNode(Expression()));
            //     while (MatchAndRemove(TokenType.END) == null)
            //         TokenList.RemoveAt(0);
            // }
        }
        else
        {
            if (MatchAndRemove(TokenType.Eol) != null)
                return statements;
            statements.Add(Statements());
            return statements;
        }

        return statements;
    }

    public ArrayRefNode ParseArrayRef()
    {
        Tokens name = Current;
        MatchAndRemove(TokenType.OpBracket);
        ExpressionNode? elem = Expression() ?? throw new Exception($" need a size for arr Element {name.GetLine()}");
        MatchAndRemove(TokenType.ClBracket);

        return new ArrayRefNode(name, elem);
    }

    public ArrayRefStatementNode ParseArrRefStatement()
    {
        Tokens? name = Current;
        MatchAndRemove(TokenType.OpBracket);
        ExpressionNode? elem =
            Expression() ?? throw new Exception($" need a size for arr Element {name.Value.GetLine()}");
        MatchAndRemove(TokenType.ClBracket);

        Tokens? e =
            MatchAndRemove(TokenType.Equals)
            ?? throw new Exception($"invalid equals on Line {name.Value.GetLine()}");
        var expr = ParseSingleExpr() ?? throw new Exception($"invalid Expr");
        return new ArrayRefStatementNode(name.Value, expr, elem);
    }

    // public TypeNode ParseTypes()
    // {
    //     // Tokens? name = GetTokenType();
    //     Tokens? name;
    //     while ((name = GetTokenType()) != null)
    //     {
    //         if (MatchAndRemove(TokenType.UNSIGNED) != null
    //             || MatchAndRemove(TokenType.CONST) != null)
    //         {
    //             attributes.Push(Current);
    //         }
    //     }
    //
    //
    //     return new TypeNode(name.Value, GetAttributes());
    // }

    public StatementNode ParseWordType()
    {
        if (LookAhead(TokenType.Equals)
            || LookAhead(TokenType.Dot))
            return ParseVarRef();
        else if (LookAhead(TokenType.OpParen))
            return ParseFunctionCalls();
        else if (LookAhead(TokenType.Word))
            return ParseVar();
        else if (LookAhead(TokenType.OpBracket))
            return ParseArrRefStatement();
        else
            throw new Exception($"invalid identifier statement {Current.ToString()}");
    }

    public Tokens? GetNativeType()
    {
        return (MatchAndRemove(TokenType.Float) != null)
            ? Current
            : (MatchAndRemove(TokenType.Int) != null)
                ? Current
                : (MatchAndRemove(TokenType.Bool) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.Char) != null)
                        ? Current
                        : (MatchAndRemove(TokenType.Int16) != null)
                            ? Current
                            : (MatchAndRemove(TokenType.String) != null)
                                ? Current
                                : (MatchAndRemove(TokenType.Int64) != null)
                                    ? Current
                                    : (MatchAndRemove(TokenType.Ulong) != null)
                                        ? Current
                                        : (MatchAndRemove(TokenType.Uint16) != null)
                                            ? Current
                                            : (MatchAndRemove(TokenType.Uint) != null)
                                                ? Current
                                                : (MatchAndRemove(TokenType.Byte) != null)
                                                    ? Current
                                                    : (MatchAndRemove(TokenType.Sbyte) != null)
                                                        ? Current
                                                        : null;
    }

    public Tokens? GetTokenType()
    {
        return GetNativeType() != null ? Current : MatchAndRemove(TokenType.Word);
    }


    public VaraibleReferenceStatementNode ParseVarRef()
    {
        Tokens? name = Current;
        Tokens? e =
            MatchAndRemove(TokenType.Equals)
            ?? throw new Exception($"invalid equals on Line {name.Value.GetLine()}");
        return new VaraibleReferenceStatementNode(
            name.Value,
            ParseSingleExpr() ?? throw new Exception($"poor refrence on line {name.Value.GetLine()}")
        );
    }

    public AttributesTuple GetAttributes(List<TokenType> list)
    {
        AttributesTuple attributesTuple = new();

        while (attributes.Any())
        {
            Tokens v = attributes.Pop();
            if (v.tokenType == TokenType.Pub && list.Contains(TokenType.Pub))
                attributesTuple.isPub = true;
            else if (v.tokenType == TokenType.Extern
                     && list.Contains(TokenType.Extern))
                attributesTuple.isExtern = true;
            else if (v.tokenType == TokenType.Const && list.Contains(TokenType.Const))
                attributesTuple.isConst = true;
            else
            {
                throw new Exception($"{v.ToString()} invalid attribute ");
            }
        }

        return attributesTuple;
    }

    public VaraibleDeclarationNode ParseAttributes()
    {
        Tokens? type;
        // AttributesTuple attributesTuple = new();
        // // (bool unsigned, bool isExtern, bool isConst) attributesTuple = (false, false, false);
        while ((type = GetTokenType()) == null)
        {
            if (MatchAndRemove(TokenType.Const) != null || MatchAndRemove(TokenType.Unsigned) != null)
            {
                attributes.Push(Current);
            }
            else
            {
                throw new Exception($"invalid attribute{TokenList[0].ToString()}");
            }
        }

        //     Tokens v = attributes.Pop();
        //     if (v.tokenType == TokenType.UNSIGNED)
        //         attributesTuple.isUnsigned = true;
        //     else if (v.tokenType == TokenType.EXTERN)
        //         attributesTuple.isExtern = true;
        //     else if (v.tokenType == TokenType.CONST)
        //         attributesTuple.isConst = true;
        // }
        //
        // return attributesTuple;
        return ParseVar();
    }

    public TypeNode ParseType()
    {
        Tokens? type;
        while ((type = GetTokenType()) == null)
        {
            if (MatchAndRemove(TokenType.Const) != null || MatchAndRemove(TokenType.Unsigned) != null)
            {
                attributes.Push(Current);
            }
            else
            {
                throw new Exception($"invalid attribute {TokenList[0].ToString()}");
            }
        }

        return new TypeNode(type.Value, GetAttributes(new List<TokenType>()
        {
            TokenType.Const,
            TokenType.Unsigned
        }));
    }


    public VaraibleDeclarationNode ParseVar()
    {
        Tokens Type = Current;


        Tokens? name = MatchAndRemove(TokenType.Word) ?? throw new Exception($"invalid type {Current.ToString()}");
        Tokens? e = MatchAndRemove(TokenType.Equals);
        AttributesTuple attributesTuple = GetAttributes(new List<TokenType>()
            { TokenType.Const, TokenType.Extern, TokenType.Unsigned, TokenType.Pub });

        if (e != null)
            return new VaraibleDeclarationNode(Type, name.Value, attributesTuple, ParseSingleExpr());
        else
            return new VaraibleDeclarationNode(Type, name.Value, attributesTuple);
    }

    public StatementNode ParseFor()
    {
        MatchAndRemove(TokenType.OpParen);
        // GetTokenType();
        var iterator = ParseVar();
        MatchAndRemove(TokenType.Eol);
        var cond = ParseSingleExpr();
        MatchAndRemove(TokenType.Eol);
        MatchAndRemove(TokenType.Word);
        var inc = ParseVarRef();
        MatchAndRemove(TokenType.ClParen);
        var statements = ParseBlock();
        return new ForLoopNode(iterator, cond, inc, statements); //c is good for a reason
    }

    public ArrayNode ParseArray()
    {
        MatchAndRemove(TokenType.OpBracket);
        var Type = ParseType();
        // var Type = GetTokenType() ?? throw new Exception("type is null");
        MatchAndRemove(TokenType.Colon);
        var size = ParseSingleExpr();
        MatchAndRemove(TokenType.ClBracket);
        var name = MatchAndRemove(TokenType.Word) ?? throw new Exception("type is null");
        return new ArrayNode(Type.Name, name, size, Type.tuple);
    }

    public ReturnNode ParseReturn()
    {
        return new ReturnNode(Expression());
    }

    public StatementNode Statements()
    {
        if (MatchAndRemove(TokenType.Word) != null)
        {
            return ParseWordType();
        }
        else if (
            MatchAndRemove(new[]
            {
                TokenType.Int,
                TokenType.String,
                TokenType.Int16,
                TokenType.Int64,
                TokenType.Float,
                TokenType.Bool,
                TokenType.Char,
                TokenType.Uint16,
                TokenType.Byte,
                TokenType.Sbyte,
                TokenType.Uint,
                TokenType.Ulong
            })
            != null
        )
        {
            // attributes.Push(Current);
            return ParseVar();
        }
        else if (MatchAndRemove(
                     new[]
                     {
                         TokenType.Const,
                     }) != null)
        {
            return ParseAttributes();
        }
        else if (MatchAndRemove(TokenType.Array) != null)
            return ParseArray();
        else if (MatchAndRemove(TokenType.If) != null)
            return ParseIf();
        else if (MatchAndRemove(TokenType.While) != null)
            return ParseWhile();
        else if (MatchAndRemove(TokenType.For) != null)
        {
            return ParseFor();
        }
        else if (MatchAndRemove(TokenType.Return) != null)
            return ParseReturn();
        else
            throw new Exception("Statement invalid " + TokenList[0].ToString());
    }

    public StructNode ParseStructs()
    {
        AttributesTuple attributesTuple = GetAttributes(new List<TokenType>()
            {  TokenType.Extern, TokenType.Pub });
        Tokens? name = MatchAndRemove(TokenType.Word) ?? throw new Exception("name is nul");
        return new StructNode(ParseTupleDef(), name.Value, attributesTuple);
    }

    public StatementNode ParseWhile()
    {
        MatchAndRemove(TokenType.OpParen);
        ExpressionNode expr = ParseSingleExpr() ?? throw new Exception($"null expr in while {Current.GetLine()} ");
        MatchAndRemove(TokenType.ClParen);
        List<StatementNode> statementNodes = ParseBlock();
        return new WhileLoopNode(expr, statementNodes);
    }

    public List<VaraibleDeclarationNode> ParseTupleDef()
    {
        MatchAndRemove(TokenType.OpParen);

        List<VaraibleDeclarationNode> param = new List<VaraibleDeclarationNode>();

        while (MatchAndRemove(TokenType.ClParen) == null)
        {
            param.Add(ParseAttributes());
            MatchAndRemove(TokenType.Comma);
        }

        return param;
    }

    public FunctionNode ParseFunction()
    {
        var p = GetAttributes(new List<TokenType>()
        {
            TokenType.Extern,
            TokenType.Pub
        });
        Tokens name = MatchAndRemove(TokenType.Word) ?? throw new Exception();
        List<StatementNode> statements = new List<StatementNode>();
        List<VaraibleDeclarationNode> param = ParseTupleDef();
        // Tokens type = new Tokens(TokenType.VOID);
        TypeNode type = new TypeNode(new Tokens(TokenType.Void), new AttributesTuple());
        if (MatchAndRemove(TokenType.Returns) != null)
        {
            type = ParseType();
            // type = GetTokenType() ?? throw new Exception("inavlid retrun");
        }

        // if (!isExtern)
        statements = ParseBlock();
        return new FunctionNode(p, name, param, type, statements);
    }

    public StatementNode GlobalStatements()
    {
        if (MatchAndRemove(TokenType.Function) != null)
            return ParseFunction();
        else if (MatchAndRemove(TokenType.Struct) != null)
            return ParseStructs();
        else if (GetTokenType() != null && !LookAhead(TokenType.Equals))
            return ParseVar();
        else if (
            MatchAndRemove(TokenType.Extern) != null
            || MatchAndRemove(TokenType.Unsigned) != null
            || MatchAndRemove(TokenType.Const) != null
        )
        {
            attributes.Push(Current);
            return GlobalStatements();
        }
        else
            throw new Exception($"{Current}Statement invalid");
    }

    public ModuleNode ParseModuleNode()
    {
        Tokens? name = MatchAndRemove(TokenType.Word) ?? throw new Exception("module needs name");
        List<Tokens> Imports = new();
        if (MatchAndRemove(TokenType.OpParen) != null)
        {
            while (MatchAndRemove(TokenType.ClParen) == null)
            {
                Imports.Add(MatchAndRemove(TokenType.Word) ?? throw new Exception("error"));
                MatchAndRemove(TokenType.Comma);
            }
        }

        ModuleNode moduleNode = new(name.Value, Imports);

        Tokens? t = MatchAndRemove(TokenType.Begin) ?? throw new Exception("need begin");
        while (MatchAndRemove(TokenType.End) == null)
        {
            if (MatchAndRemove(TokenType.Function) != null)
                moduleNode.FunctionNodes.Add(ParseFunction());
            else if (MatchAndRemove(TokenType.Struct) != null)
                moduleNode.StructNodes.Add(ParseStructs());
            else if (MatchAndRemove(TokenType.Array) != null)
                moduleNode.VaraibleDeclarationNodes.Add(ParseArray());
            else if (GetTokenType() != null && !LookAhead(TokenType.Equals))
                moduleNode.VaraibleDeclarationNodes.Add(ParseVar());
            else if (
                MatchAndRemove(TokenType.Extern) != null
                || MatchAndRemove(TokenType.Const) != null
                || MatchAndRemove(TokenType.Pub) != null
            )
                attributes.Push(Current);
            MatchAndRemove(TokenType.Eol);
        }

        return moduleNode;
    }

    public PerenNode ParseFile()
    {
        Dictionary<string, ModuleNode> modules = new();
        while (TokenList.Count != 0)
        {
            if (MatchAndRemove(TokenType.Mod) != null)
            {
                ModuleNode m = ParseModuleNode();
                modules.Add(m.Name.buffer, m);
            }
        }

        return new PerenNode(modules);
    }
}