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
            else if (LookAhead(TokenType.OP_BRACKET))
                return ParseArrayRef();
            return new VaraibleReferenceNode(Current);
        }
        else if (MatchAndRemove(TokenType.STRING_LITERAL) != null)
        {
            return new StringNode(Current);
        }
        else if (MatchAndRemove(TokenType.OP_PAREN) != null)
        {
            Tokens? type = GetNativeType();
            if (type != null)
            {
                MatchAndRemove(TokenType.CL_PAREN);

                ExpressionNode? b = Factor();
                return new CastNode(b, type.Value);
            }

            ExpressionNode? a = ParseSingleExpr();
            Console.WriteLine("hai");
            MatchAndRemove(TokenType.CL_PAREN);

            return a;
        }
        else if (MatchAndRemove(TokenType.NOT) != null)
        {
            Tokens a = Current;
            ExpressionNode? v = Factor();
            return
                new
                    OpNode(v, v, a);
        }
        else if (MatchAndRemove(TokenType.SIZE) != null)
        {
            MatchAndRemove(TokenType.OP_PAREN);
            ExpressionNode? v = Expression();
            MatchAndRemove(TokenType.CL_PAREN);
            return new OpNode(v,
                v, new Tokens(TokenType.SIZE));
        }

        return null;
    }

    private ExpressionNode? BoolExpr()
    {
        ExpressionNode? opNode = Factor();
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
                                : (MatchAndRemove(TokenType.NOT_EQUALS) != null)
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
                                        : MatchAndRemove(TokenType.XOR) != null
                                            ? Current
                                            : null;
        if (opNode == null && op != null && op.Value.tokenType != TokenType.NOT)
            throw new Exception("unauthorized statement");
        while (op != null)
        {
            ExpressionNode? right = ParseNot();

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
                                            : MatchAndRemove(TokenType.XOR) != null
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
            (MatchAndRemove(TokenType.ADDITION) != null)
                ? Current
                : (MatchAndRemove(TokenType.SUBTRACTION) != null)
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
                (MatchAndRemove(TokenType.ADDITION) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.SUBTRACTION) != null)
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

        while (MatchAndRemove(TokenType.CL_PAREN) == null)
        {
            expr.Add(
                ParseSingleExpr()
                ?? throw new Exception( //e
                    $"need all types tuple on line {Current.GetLine()}"
                )
            );
            MatchAndRemove(TokenType.COMMA);
        }

        return expr;
    }

    public FunctionCallNode ParseFunctionCalls()
    {
        Tokens name = Current;
        Tokens? a =
            MatchAndRemove(TokenType.OP_PAREN) ?? throw new Exception("function is a tuple type");
        List<ExpressionNode> expr = ParseTupleAssignment();
        return new FunctionCallNode(name, expr);
    }

    public ElseNode? ParseElse()
    {
        List<StatementNode> statementNodes = new();
        if (MatchAndRemove(TokenType.ELSE) != null)
        {
            statementNodes = ParseBlock();
        }

        return new ElseNode(statementNodes);
    }

    public IfNode ParseIf()
    {
        MatchAndRemove(TokenType.OP_PAREN);
        ExpressionNode expr = ParseSingleExpr() ?? throw new Exception($"null expr in if {Current.GetLine()} ");
        MatchAndRemove(TokenType.CL_PAREN);
        List<StatementNode> statementNodes = ParseBlock();
        return new IfNode(expr, ParseElse(), statementNodes);
    }

    public List<StatementNode> ParseBlock()
    {
        List<StatementNode> statements = new();
        if (MatchAndRemove(TokenType.BEGIN) != null)
        {
            while (
                MatchAndRemove(TokenType.END) == null
            )
            {
                statements.Add(Statements());
                MatchAndRemove(TokenType.EOL);
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
            if (MatchAndRemove(TokenType.EOL) != null)
                return statements;
            statements.Add(Statements());
            return statements;
        }

        return statements;
    }

    public ArrayRefNode ParseArrayRef()
    {
        Tokens name = Current;
        MatchAndRemove(TokenType.OP_BRACKET);
        ExpressionNode? elem = Expression() ?? throw new Exception($" need a size for arr Element {name.GetLine()}");
        MatchAndRemove(TokenType.CL_BRACKET);

        return new ArrayRefNode(name, elem);
    }

    public ArrayRefStatementNode ParseArrRefStatement()
    {
        Tokens? name = Current;
        MatchAndRemove(TokenType.OP_BRACKET);
        ExpressionNode? elem =
            Expression() ?? throw new Exception($" need a size for arr Element {name.Value.GetLine()}");
        MatchAndRemove(TokenType.CL_BRACKET);

        Tokens? e =
            MatchAndRemove(TokenType.EQUALS)
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
        if (LookAhead(TokenType.EQUALS)
            || LookAhead(TokenType.DOT))
            return ParseVarRef();
        else if (LookAhead(TokenType.OP_PAREN))
            return ParseFunctionCalls();
        else if (LookAhead(TokenType.WORD))
            return ParseVar();
        else if (LookAhead(TokenType.OP_BRACKET))
            return ParseArrRefStatement();
        else
            throw new Exception($"invalid identifier statement {Current.ToString()}");
    }

    public Tokens? GetNativeType()
    {
        return (MatchAndRemove(TokenType.FLOAT) != null)
            ? Current
            : (MatchAndRemove(TokenType.INT) != null)
                ? Current
                : (MatchAndRemove(TokenType.BOOL) != null)
                    ? Current
                    : (MatchAndRemove(TokenType.CHAR) != null)
                        ? Current
                        : (MatchAndRemove(TokenType.INT16) != null)
                            ? Current
                            : (MatchAndRemove(TokenType.STRING) != null)
                                ? Current
                                : (MatchAndRemove(TokenType.INT64) != null)
                                    ? Current
                                    : (MatchAndRemove(TokenType.ULONG) != null)
                                        ? Current
                                        : (MatchAndRemove(TokenType.UINT_16) != null)
                                            ? Current
                                            : (MatchAndRemove(TokenType.UINT) != null)
                                                ? Current
                                                : (MatchAndRemove(TokenType.BYTE) != null)
                                                    ? Current
                                                    : (MatchAndRemove(TokenType.SBYTE) != null)
                                                        ? Current
                                                        : null;
    }

    public Tokens? GetTokenType()
    {
        return GetNativeType() != null ? Current : MatchAndRemove(TokenType.WORD);
    }


    public VaraibleReferenceStatementNode ParseVarRef()
    {
        Tokens? name = Current;
        Tokens? e =
            MatchAndRemove(TokenType.EQUALS)
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
            if (v.tokenType == TokenType.PUB && list.Contains(TokenType.PUB))
                attributesTuple.isPub = true;
            else if (v.tokenType == TokenType.EXTERN
                     && list.Contains(TokenType.EXTERN))
                attributesTuple.isExtern = true;
            else if (v.tokenType == TokenType.CONST && list.Contains(TokenType.CONST))
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
            if (MatchAndRemove(TokenType.CONST) != null || MatchAndRemove(TokenType.UNSIGNED) != null)
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
            if (MatchAndRemove(TokenType.CONST) != null || MatchAndRemove(TokenType.UNSIGNED) != null)
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
            TokenType.CONST,
            TokenType.UNSIGNED
        }));
    }


    public VaraibleDeclarationNode ParseVar()
    {
        Tokens Type = Current;


        Tokens? name = MatchAndRemove(TokenType.WORD) ?? throw new Exception($"invalid type {Current.ToString()}");
        Tokens? e = MatchAndRemove(TokenType.EQUALS);
        AttributesTuple attributesTuple = GetAttributes(new List<TokenType>()
            { TokenType.CONST, TokenType.EXTERN, TokenType.UNSIGNED });

        if (e != null)
            return new VaraibleDeclarationNode(Type, name.Value, attributesTuple, ParseSingleExpr());
        else
            return new VaraibleDeclarationNode(Type, name.Value, attributesTuple);
    }

    public StatementNode ParseFor()
    {
        MatchAndRemove(TokenType.OP_PAREN);
        // GetTokenType();
        var iterator = ParseVar();
        MatchAndRemove(TokenType.EOL);
        var cond = ParseSingleExpr();
        MatchAndRemove(TokenType.EOL);
        MatchAndRemove(TokenType.WORD);
        var inc = ParseVarRef();
        MatchAndRemove(TokenType.CL_PAREN);
        var statements = ParseBlock();
        return new ForLoopNode(iterator, cond, inc, statements); //c is good for a reason
    }

    public ArrayNode ParseArray()
    {
        MatchAndRemove(TokenType.OP_BRACKET);
        var Type = ParseType();
        // var Type = GetTokenType() ?? throw new Exception("type is null");
        MatchAndRemove(TokenType.COLON);
        var size = ParseSingleExpr();
        MatchAndRemove(TokenType.CL_BRACKET);
        var name = MatchAndRemove(TokenType.WORD) ?? throw new Exception("type is null");
        return new ArrayNode(Type.Name, name, size, Type.tuple);
    }

    public ReturnNode ParseReturn()
    {
        return new ReturnNode(Expression());
    }

    public StatementNode Statements()
    {
        if (MatchAndRemove(TokenType.WORD) != null)
        {
            return ParseWordType();
        }
        else if (
            MatchAndRemove(new[]
            {
                TokenType.INT,
                TokenType.STRING,
                TokenType.INT16,
                TokenType.INT64,
                TokenType.FLOAT,
                TokenType.BOOL,
                TokenType.CHAR,
                TokenType.UINT_16,
                TokenType.BYTE,
                TokenType.SBYTE,
                TokenType.UINT,
                TokenType.ULONG
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
                         TokenType.CONST,
                     }) != null)
        {
            return ParseAttributes();
        }
        else if (MatchAndRemove(TokenType.ARRAY) != null)
            return ParseArray();
        else if (MatchAndRemove(TokenType.IF) != null)
            return ParseIf();
        else if (MatchAndRemove(TokenType.WHILE) != null)
            return ParseWhile();
        else if (MatchAndRemove(TokenType.FOR) != null)
        {
            return ParseFor();
        }
        else if (MatchAndRemove(TokenType.RETURN) != null)
            return ParseReturn();
        else
            throw new Exception("Statement invalid " + TokenList[0].ToString());
    }

    public StructNode ParseStructs()
    {
        if (attributes.Any())
            throw new Exception("Struct cant have attributes");
        Tokens? name = MatchAndRemove(TokenType.WORD) ?? throw new Exception("name is nul");
        return new StructNode(ParseTupleDef(), name.Value);
    }

    public StatementNode ParseWhile()
    {
        MatchAndRemove(TokenType.OP_PAREN);
        ExpressionNode expr = ParseSingleExpr() ?? throw new Exception($"null expr in if {Current.GetLine()} ");
        MatchAndRemove(TokenType.CL_PAREN);

        List<StatementNode> statementNodes = ParseBlock();
        return new WhileLoopNode(expr, statementNodes);
    }

    public List<VaraibleDeclarationNode> ParseTupleDef()
    {
        MatchAndRemove(TokenType.OP_PAREN);

        List<VaraibleDeclarationNode> param = new List<VaraibleDeclarationNode>();

        while (MatchAndRemove(TokenType.CL_PAREN) == null)
        {
            // GetTokenType();
            param.Add(ParseAttributes());
            MatchAndRemove(TokenType.COMMA);
        }

        return param;
    }

    public FunctionNode ParseFunction()
    {
        var p = GetAttributes(new List<TokenType>()
        {
            TokenType.EXTERN,
            TokenType.PUB
        });
        Tokens name = MatchAndRemove(TokenType.WORD) ?? throw new Exception();
        List<StatementNode> statements = new List<StatementNode>();
        List<VaraibleDeclarationNode> param = ParseTupleDef();
        // Tokens type = new Tokens(TokenType.VOID);
        TypeNode type = new TypeNode(new Tokens(TokenType.VOID), new AttributesTuple());
        if (MatchAndRemove(TokenType.RETURNS) != null)
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
        if (MatchAndRemove(TokenType.FUNCTION) != null)
            return ParseFunction();
        else if (MatchAndRemove(TokenType.STRUCT) != null)
            return ParseStructs();
        else if (GetTokenType() != null && !LookAhead(TokenType.EQUALS))
            return ParseVar();
        else if (
            MatchAndRemove(TokenType.EXTERN) != null
            || MatchAndRemove(TokenType.UNSIGNED) != null
            || MatchAndRemove(TokenType.CONST) != null
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
        Tokens? name = MatchAndRemove(TokenType.WORD) ?? throw new Exception("module needs name");
        List<Tokens> Imports = new();
        if (MatchAndRemove(TokenType.OP_PAREN) != null)
        {
            while (MatchAndRemove(TokenType.CL_PAREN) == null)
            {
                Imports.Add(MatchAndRemove(TokenType.WORD) ?? throw new Exception("error"));
                MatchAndRemove(TokenType.COMMA);
            }
        }

        ModuleNode moduleNode = new(name.Value, Imports);

        Tokens? t = MatchAndRemove(TokenType.BEGIN) ?? throw new Exception("need begin");
        while (MatchAndRemove(TokenType.END) == null)
        {
            if (MatchAndRemove(TokenType.FUNCTION) != null)
                moduleNode.FunctionNodes.Add(ParseFunction());
            else if (MatchAndRemove(TokenType.STRUCT) != null)
                moduleNode.StructNodes.Add(ParseStructs());
            else if (MatchAndRemove(TokenType.ARRAY) != null)
                moduleNode.VaraibleDeclarationNodes.Add(ParseArray());
            else if (GetTokenType() != null && !LookAhead(TokenType.EQUALS))
                moduleNode.VaraibleDeclarationNodes.Add(ParseVar());
            else if (
                MatchAndRemove(TokenType.EXTERN) != null
                || MatchAndRemove(TokenType.CONST) != null
                || MatchAndRemove(TokenType.PUB) != null
            )
                attributes.Push(Current);
            MatchAndRemove(TokenType.EOL);
        }

        return moduleNode;
    }

    public PerenNode ParseFile()
    {
        Dictionary<string, ModuleNode> modules = new();
        while (TokenList.Count != 0)
        {
            if (MatchAndRemove(TokenType.MOD) != null)
            {
                ModuleNode m = ParseModuleNode();
                modules.Add(m.Name.buffer, m);
            }
        }
        return new PerenNode(modules);
    }
}