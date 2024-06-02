using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticVisitExpr : ExpressionVisit<LacusType>
{
    public SemanticContext<SemanticVar> _Context { get; set; }

    public LacusType AssignedType;

    public SemanticVisitExpr(SemanticContext<SemanticVar> context, LacusType AssignedType)
    {
        this.AssignedType = AssignedType;
        this._Context = context;
    }

    public override LacusType Visit(IntegerNode node)
    {
        return new LacusType(TypeEnum.INTEGER);
    }

    public override LacusType Visit(FloatNode node)
    {
        return new LacusType(TypeEnum.FLOAT);
    }

    public override LacusType Visit(BoolNode node)
    {
        return new LacusType(TypeEnum.BOOL);
    }

    public override LacusType Visit(FunctionCallNode node)
    {
        throw new NotImplementedException();
    }

    public override LacusType Visit(OpNode node)
    {
        LacusType LType = node.right.Visit(this);
        LacusType RType = node.left.Visit(this);

        if (LType.Type == TypeEnum.VOID || RType.Type == TypeEnum.VOID)
            throw new Exception("type error");
        else
            return LType;
    }

    public override LacusType Visit(VaraibleReferenceNode node)
    {
        SemanticVar v = _Context.GetValue(node.name);
        node.ScopeLocation = v.ScopeLocation;
        return v.VarType;
    }

    public override LacusType Visit(BooleanExprNode node)
    {
        throw new NotImplementedException();
    }

    public override LacusType Visit(CharNode node)
    {
        throw new NotImplementedException();
    }
}
