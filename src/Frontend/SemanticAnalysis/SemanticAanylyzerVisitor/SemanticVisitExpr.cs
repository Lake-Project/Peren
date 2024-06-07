using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticVisitExpr(SemanticProgram program, LacusType assignedType)
    : ExpressionVisit<LacusType>
{
    public SemanticProgram Context { get; set; } = program;

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
        SemanticFunction f = Context.GetFunction(node.Name);
        if (node.ParamValues.Count != f.ParamTypes.Count)
            throw new Exception("no matching type");
        for (int i = 0; i < f.ParamTypes.Count; i++)
        {
            LacusType t = node.ParamValues[i]
                .Visit(new SemanticVisitExpr(Context, f.ParamTypes[i]));
            if (!f.ParamTypes[i].CanAccept(t))
                throw new Exception("error");
        }

        return f.retType;
    }

    public override LacusType Visit(OpNode node)
    {
        LacusType LType = node.Left.Visit(this);
        LacusType RType = node.Right.Visit(this);
        if (assignedType is FloatType)
            node.FloatExpr = true;

        if (assignedType is BoolType)
        {
            if (LType.CanAccept(RType) && LType.GetType() == RType.GetType())
            {
                if (LType is not FloatType)
                    node.FloatExpr = false;
                return LType;
            }

            throw new TypeMisMatchException(
                $"type aa  {assignedType} cant fit "
                    + $"{(assignedType.CanAccept(LType)
                    ? RType : LType)}"
            );
        }

        if (assignedType.CanAccept(LType) && assignedType.CanAccept(RType))
        {
            if (LType.GetType() == RType.GetType())
            {
                if (LType is not FloatType)
                    node.FloatExpr = false;
                return LType;
            }
            else if (
                LType.GetType() != RType.GetType()
                && RType.GetType() != assignedType.GetType()
            )
            {
                if (RType is not FloatType)
                    node.FloatExpr = false;
                return RType;
            }
            else if (
                LType.GetType() != RType.GetType()
                && LType.GetType() != assignedType.GetType()
            )
            {
                if (LType is not FloatType)
                    node.FloatExpr = false;
                return LType;
            }
        }

        throw new TypeMisMatchException(
            $"type  a {assignedType} cant fit "
                + $"{(assignedType.CanAccept(LType)
                ? RType : LType)}"
        );
    }

    public override LacusType Visit(VaraibleReferenceNode node)
    {
        SemanticVar v = Context.GetVar(node.Name);
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
