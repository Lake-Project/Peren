using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
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
        return new IntegerType();
    }

    public override LacusType Visit(FloatNode node)
    {
        return new FloatType();
    }

    public override LacusType Visit(BoolNode node)
    {
        return new BoolType();
    }

    public override LacusType Visit(FunctionCallNode node)
    {
        throw new NotImplementedException();
    }

    public override LacusType Visit(OpNode node)
    {
        LacusType LType = node.right.Visit(this);
        LacusType RType = node.left.Visit(this);
        if (AssignedType is FloatType)
            node.FloatExpr = true;
        if (AssignedType.CanAccept(LType) && AssignedType.CanAccept(RType))
        {
            if (LType.GetType() == RType.GetType())
            {
                if (LType is not FloatType)
                    node.FloatExpr = false;
                return LType;
            }
            else if (
                LType.GetType() != RType.GetType()
                && RType.GetType() != AssignedType.GetType()
            )
            {
                if (RType is not FloatType)
                    node.FloatExpr = false;
                return RType;
            }
            else if (
                LType.GetType() != RType.GetType()
                && LType.GetType() != AssignedType.GetType()
            )
            {
                if (LType is not FloatType)
                    node.FloatExpr = false;
                return LType;
            }
        }

        throw new Exception("Type error assigned ");
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
        return new CharType();
    }
}
