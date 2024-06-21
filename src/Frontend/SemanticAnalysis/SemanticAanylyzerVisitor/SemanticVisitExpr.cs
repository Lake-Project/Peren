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
                $"type  {assignedType} cant fit "
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
        LacusType LType = node.Left.Visit(this);
        LacusType RType = node.Right.Visit(this);
        Console.WriteLine(node.ToString());
        if (RType.CanAccept(LType) && LType.GetType() == RType.GetType())
        {
            if (LType is FloatType)
                node.IsFloat = true;
            return new BoolType();
        }

        throw new TypeMisMatchException(
            $"type {RType} cant be cmp to "
            + $"{LType}"
        );    }

    public override LacusType Visit(CharNode node)
    {
        return new CharType();
    }

    public override LacusType Visit(CastNode node)
    {
        LacusType t = node.Expr.Visit(new SemanticVisitExpr(program, new UnknownType()));
        Func<LacusType, Tokens> ToTokens = (t) =>
        {
            return t switch
            {
                FloatType => new Tokens(TokenType.FLOAT),
                CharType => new Tokens(TokenType.CHAR),
                BoolType => new Tokens(TokenType.BOOL),
                IntegerType => new Tokens(TokenType.INT),
                _ => throw new Exception("unknown type")
            };
        };

        node.inferredtype = ToTokens(t);
        return node.type.tokenType switch
        {
            TokenType.INT => new IntegerType(),
            TokenType.INT16 => new IntegerType(),
            TokenType.INT64 => new IntegerType(),
            TokenType.BOOL => new BoolType(),
            TokenType.FLOAT => new FloatType(),
            TokenType.CHAR => new CharType(),
            TokenType.VOID => new VoidType(),
            _ => throw new Exception($"type {node.type} is unknown ")
        };
    }
}