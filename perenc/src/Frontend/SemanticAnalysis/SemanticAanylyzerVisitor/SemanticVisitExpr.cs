using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticVisitExpr(SemanticProgram program, PerenType assignedType)
    : ExpressionVisit<PerenType>
{
    public SemanticProgram Context { get; set; } = program;

    public override PerenType Visit(IntegerNode node)
    {
        node.Range = assignedType is IntegerType ? assignedType.Range : Range.ThirtyTwoBit;
        return new IntegerType(true, node.Range, assignedType.IsUnsigned);
    }

    public override PerenType Visit(FloatNode node)
    {
        return new FloatType(true);
    }

    public override PerenType Visit(BoolNode node)
    {
        return new BoolType(true);
    }

    public override PerenType Visit(FunctionCallNode node)
    {
        SemanticFunction f = Context.GetFunction(node.Name);
        if (node.ParamValues.Count != f.ParamTypes.Count)
            throw new Exception("no matching type");
        for (int i = 0; i < f.ParamTypes.Count; i++)
        {
            PerenType t = node.ParamValues[i]
                .Visit(new SemanticVisitExpr(Context, f.ParamTypes[i]));
            if (!f.ParamTypes[i].CanAccept(t))
                throw new Exception("error");
        }

        return f.RetType;
    }

    public override PerenType Visit(OpNode node)
    {
        PerenType LType = node.Left.Visit(this);
        PerenType RType = node.Right.Visit(this);
        if (assignedType is FloatType && LType is FloatType && RType is FloatType)
            node.FloatExpr = true;
        else
            node.FloatExpr = false;
        node.IsUnsignedExpr = assignedType.IsUnsigned;
        if (assignedType is UnknownType)
        {
            if (RType.CanAccept(LType) && LType.GetType() == RType.GetType())
            {
                if (RType.OpAccept(node.Token))
                    return LType;
                throw new TypeMisMatchException(
                    $"type  {LType} cant fit "
                    + $"{node.Token}"
                );
            }
        }
        else if (assignedType is BoolType)
        {
            if (RType.CanAccept(LType) && LType.GetType() == RType.GetType())
            {
                if (RType.OpAccept(node.Token))
                    return LType;
                throw new TypeMisMatchException(
                    $"type  {LType} cant fit "
                    + $"{node.Token}"
                );
            }

            throw new TypeMisMatchException(
                $"type  {LType} cant fit "
                + $"{(assignedType.CanAccept(LType)
                    ? RType : LType)}"
            );
        }
        else if (assignedType.CanAccept(LType) && assignedType.CanAccept(RType))
        {
            if (!assignedType.OpAccept(node.Token))
            {
                throw new TypeMisMatchException(
                    $"operator {node.Token} cant be for type {assignedType}"
                );
            }

            return RType.GetType() != assignedType.GetType() ? RType :
                LType.GetType() != assignedType.GetType() ? LType : RType;
        }

        throw new TypeMisMatchException(
            $"type  a {assignedType} cant fit "
            + $"{(assignedType.CanAccept(LType)
                ? RType : LType)}"
        );
    }

    public override PerenType Visit(VaraibleReferenceNode node)
    {
        var v = Context.GetVar(node.Name);
        if (node is ArrayRefNode arr)
        {
            arr.Elem.Visit(new SemanticVisitExpr(program, new IntegerType(false)));
            return v.VarType.simplerType;
        }
        return v.VarType;
    }

    public override PerenType Visit(BooleanExprNode node)
    {
        PerenType LType = node.Left.Visit(this);
        PerenType RType = node.Right.Visit(this);
        if (LType.IsUnsigned || RType.IsUnsigned)
            node.IsUnsigned = true;
        // Console.WriteLine(node.ToString());
        if (LType.GetType() == RType.GetType() && assignedType.OpAccept(node.Op))
        {
            if (LType is FloatType)
                node.IsFloat = true;
            return new BoolType(true);
        }

        throw new TypeMisMatchException(
            $"type {RType} cant be cmp to "
            + $"{LType}"
        );
    }

    public override PerenType Visit(CharNode node)
    {
        return new CharType(true);
    }

    public override PerenType Visit(CastNode node)
    {
        // LacusType t = node.type.tokenType switch
        // {
        //     TokenType.INT => new IntegerType(false, Range.ThirtyTwoBit),
        //     TokenType.INT16 => new IntegerType(false, Range.SixteenBit),
        //     TokenType.INT64 => new IntegerType(false, Range.SixtyFourBit),
        //     TokenType.BOOL => new BoolType(false, Range.OneBit),
        //     TokenType.FLOAT => new FloatType(false, Range.Float),
        //     TokenType.CHAR => new CharType(false, Range.EightBit),
        //     TokenType.ULONG => new IntegerType(false, Range.SixtyFourBit, true),
        //     TokenType.BYTE => new IntegerType(false, Range.EightBit, true),
        //     TokenType.SBYTE => new IntegerType(false, Range.EightBit),
        //     TokenType.UINT => new IntegerType(false, Range.ThirtyTwoBit, true),
        //     TokenType.UINT_16 => new IntegerType(false, Range.SixteenBit, true),
        //     TokenType.STRING => new ArrayType(new CharType(false, Range.EightBit), false),
        //     _ => throw new Exception($"type{node.type.ToString()} doesnt exist")
        // };
        var t = SemanticAnaylsis.TokenToPerenType(node.type, false, program);
        var ty = node.Expr.Visit(new SemanticVisitExpr(program, new UnknownType(false, Range.None)));
        // t switch ()
        switch (t)
        {
            case IntegerType when ty is FloatType:
                node.inferredtype = CastType.FLOAT;
                break;
            case FloatType when ty is IntegerType:
                node.inferredtype = CastType.INT;
                break;
            default:
            {
                if (t.Range < ty.Range)
                {
                    node.inferredtype = CastType.TRUNCATE;
                }
                else if (t.Range > ty.Range)
                {
                    node.inferredtype = CastType.SEXT;
                }

                break;
            }
        }

        return t;
    }

    public override PerenType Visit(StringNode node)
    {
        return new ArrayType(new CharType(false), true);
    }
}