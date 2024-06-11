using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor.Backend.Interpter;

public class InpretExpr : ExpressionVisit<int>
{
    public override int Visit(IntegerNode node)
    {
        return node.Value;
    }

    public override int Visit(FloatNode node)
    {
        throw new NotImplementedException();
    }

    public override int Visit(BoolNode node)
    {
        throw new NotImplementedException();
    }

    public override int Visit(FunctionCallNode node)
    {
        throw new NotImplementedException();
    }

    public override int Visit(OpNode node)
    {
        int L = node.Left.Visit(this);
        int R = node.Right.Visit(this);
        return node.Token.tokenType switch
        {
            TokenType.ADDITION => L + R,
            TokenType.SUBTRACTION => L - R,
            TokenType.MULTIPLICATION => L * R,
            TokenType.DIVISION => L / R,
            _ => throw new Exception("")
        };
    }

    public override int Visit(VaraibleReferenceNode node)
    {
        throw new NotImplementedException();
    }

    public override int Visit(BooleanExprNode node)
    {
        int L = node.Left.Visit(this);
        int R = node.Right.Visit(this);
        return node.Op.tokenType switch
        {
            TokenType.GT => (L > R) ? 1 : 0,
            TokenType.LT =>(L < R) ? 1 : 0,
            TokenType.GTE => (L >= R) ? 1 : 0,
            TokenType.LTE => (L <= R) ? 1 :0,
            TokenType.EQUALS => (L == R) ? 1 :0,
            _ => throw new Exception("")
        };    }

    public override int Visit(CharNode node)
    {
        throw new NotImplementedException();
    }

    public override int Visit(CastNode node)
    {
        throw new NotImplementedException();
    }
}