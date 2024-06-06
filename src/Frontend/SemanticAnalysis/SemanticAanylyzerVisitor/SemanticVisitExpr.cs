using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticVisitExpr : ExpressionVisit<LacusType>
{
    public SemanticProgram _Context { get; set; }

    public LacusType AssignedType;

    public SemanticVisitExpr(SemanticProgram program, LacusType AssignedType)
    {
        this.AssignedType = AssignedType;
        this._Context = program;
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
        SemanticFunction f = _Context.GetFunction(node.Name);
        if (node.ParamValues.Count != f.ParamTypes.Count)
            throw new Exception("no matching type");
        for (int i = 0; i < f.ParamTypes.Count; i++)
        {
            LacusType t = node.ParamValues[i]
                .Visit(new SemanticVisitExpr(_Context, f.ParamTypes[i]));
            if (!f.ParamTypes[i].CanAccept(t))
                throw new Exception("error");
        }

        return f.retType;
    }

    public override LacusType Visit(OpNode node)
    {
        LacusType LType = node.left.Visit(this);
        LacusType RType = node.right.Visit(this);
        if (AssignedType is FloatType)
            node.FloatExpr = true;

        if (AssignedType is BoolType)
        {
            if (LType.CanAccept(RType) && LType.GetType() == RType.GetType())
            {
                if (LType is not FloatType)
                    node.FloatExpr = false;
                return LType;
            }

            throw new TypeMisMatchException(
                $"type  {AssignedType} cant fit "
                    + $"{(AssignedType.CanAccept(LType)
                    ? RType : LType)}"
            );
        }

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

        throw new TypeMisMatchException(
            $"type  {AssignedType} cant fit "
                + $"{(AssignedType.CanAccept(LType)
                ? RType : LType)}"
        );
    }

    public override LacusType Visit(VaraibleReferenceNode node)
    {
        SemanticVar v = _Context.GetVar(node.name);
        node.ScopeLocation = v.ScopeLocation;
        return v.VarType;
    }

    public override LacusType Visit(BooleanExprNode node)
    {
        LacusType LType = node.left.Visit(this);
        LacusType RType = node.right.Visit(this);
        if (RType.CanAccept(LType) && LType.GetType() == RType.GetType())
        {
            if (LType is FloatType)
                node.isFloat = true;
            return new BoolType();
        }

        throw new Exception("error");
    }

    public override LacusType Visit(CharNode node)
    {
        return new CharType();
    }
}