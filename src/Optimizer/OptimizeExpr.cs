using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.SemanticAanylyzerVisitor.Optimizer;

public class OptimizeExpr : ExpressionVisit<INode>
{
    public override INode Visit(IntegerNode node)
    {
        return node;
    }

    public override INode Visit(FloatNode node)
    {
        return node;
    }

    public override INode Visit(BoolNode node)
    {
        throw new NotImplementedException();
    }

    public override INode Visit(FunctionCallNode node)
    {
        throw new NotImplementedException();
    }

    public override INode Visit(OpNode node)
    {
        throw new NotImplementedException();
    }

    public override INode Visit(VaraibleReferenceNode node)
    {
        throw new NotImplementedException();
    }

    public override INode Visit(BooleanExprNode node)
    {
        throw new NotImplementedException();
    }

    public override INode Visit(CharNode node)
    {
        return node;
    }

    public override INode Visit(CastNode node)
    {
        throw new NotImplementedException();
    }

    public override INode Visit(StringNode node)
    {
        return node;
    }
}