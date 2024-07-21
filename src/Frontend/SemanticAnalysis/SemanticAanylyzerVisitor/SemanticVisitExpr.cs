using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.Frontend.SemanticAnalysis;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public class SemanticVisitExpr(SemanticProgram program, LacusType assignedType)
    : ExpressionVisit<LacusType>
{
    public SemanticProgram Context { get; set; } = program;

    public override LacusType Visit(IntegerNode node)
    {
        return new IntegerType(true);
    }

    public override LacusType Visit(FloatNode node)
    {
        return new FloatType(true);
    }

    public override LacusType Visit(BoolNode node)
    {
        return new BoolType(true);
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
        if (assignedType is FloatType && LType is FloatType && RType is FloatType)
            node.FloatExpr = true;
        else
            node.FloatExpr = false;

        if (assignedType is BoolType)
        {
            if (RType.CanAccept(LType) && LType.GetType() == RType.GetType())
            {
                return LType;
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

    public override LacusType Visit(VaraibleReferenceNode node)
    {
        SemanticVar v = Context.GetVar(node.Name);
        // if(v.VarType.IsConst 
        // if (node is ArrayRefNode arr)
        // {
        //     arr.Elem.Visit(new SemanticVisitExpr(program, new IntegerType(false)));
        //     return v.VarType.simplerType;
        // }

        node.ScopeLocation = v.ScopeLocation;
        return v.VarType;
    }

    public override LacusType Visit(BooleanExprNode node)
    {
        LacusType LType = node.Left.Visit(this);
        LacusType RType = node.Right.Visit(this);
        // Console.WriteLine(node.ToString());
        if (RType.CanAccept(LType) && LType.GetType() == RType.GetType() && assignedType.OpAccept(node.Op))
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

    public override LacusType Visit(CharNode node)
    {
        return new CharType(true);
    }

    public override LacusType Visit(CastNode node)
    {
        // LacusType t = node.Expr.Visit(new SemanticVisitExpr(program, new UnknownType()));
        // Func<LacusType, Tokens> ToTokens = (t) =>
        // {
        //     return t switch
        //     {
        //         FloatType => new Tokens(TokenType.FLOAT),
        //         CharType => new Tokens(TokenType.CHAR),
        //         BoolType => new Tokens(TokenType.BOOL),
        //         IntegerType => new Tokens(TokenType.INT),
        //         _ => throw new Exception("unknown type")
        //     };
        // };
        //
        // node.inferredtype = ToTokens(t);
        // return node.type.tokenType switch
        // {
        //     TokenType.INT => new IntegerType(),
        //     TokenType.INT16 => new IntegerType(),
        //     TokenType.INT64 => new IntegerType(),
        //     TokenType.BOOL => new BoolType(),
        //     TokenType.FLOAT => new FloatType(),
        //     TokenType.CHAR => new CharType(),
        //     TokenType.VOID => new VoidType(),
        //     _ => throw new Exception($"type {node.type} is unknown ")
        // };
        throw new NotImplementedException();
    }

    public override LacusType Visit(StringNode node)
    {
        return new ArrayType(new CharType(false), true);
    }
}