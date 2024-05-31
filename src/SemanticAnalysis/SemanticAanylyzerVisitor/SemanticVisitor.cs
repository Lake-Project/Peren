using System.Diagnostics;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public struct SemanticVar
{
    public LacusType VarType { get; set; }

    public SemanticVar(LacusType type)
    {
        VarType = type;
    }
}

public class SemanticVisitor : SemanticVisit
{
    // private Context _context;
    private SemanticContext<SemanticVar> _Context;
    public LacusType AssignedType;

    public Dictionary<TypeEnum, int> TypeOrdance =
        new()
        {
            [TypeEnum.CHAR] = 0,
            [TypeEnum.INTEGER] = 1,
            [TypeEnum.FLOAT] = 2,
        };

    public SemanticVisitor(SemanticContext<SemanticVar> context)
    {
        // this._context = context;
        _Context = context;
        AssignedType = new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(OpNode node)
    {
        LacusType LType = node.right.VisitSemanticAnaylsis(this);
        LacusType RType = node.left.VisitSemanticAnaylsis(this);
        if (LType.Type == TypeEnum.VOID || RType.Type == TypeEnum.VOID)
        {
            throw new Exception("type error");
        }

        if (
            TypeOrdance[LType.Type] > TypeOrdance[AssignedType.Type]
            || TypeOrdance[RType.Type] > TypeOrdance[AssignedType.Type]
        )
        {
            throw new Exception("type error");
        }

        return
            TypeOrdance[AssignedType.Type] >= TypeOrdance[LType.Type]
            && TypeOrdance[LType.Type] >= TypeOrdance[RType.Type]
            ? LType
            : RType;
    }

    public override LacusType SemanticAccept(IntegerNode node)
    {
        return new LacusType(TypeEnum.INTEGER);
    }

    public override LacusType SemanticAccept(FloatNode node)
    {
        return new LacusType(TypeEnum.FLOAT);
    }

    public override LacusType SemanticAccept(VaraibleReferenceNode node)
    {
        return _Context.GetValue(node.name).VarType;
    }

    public override LacusType SemanticAccept(VaraibleReferenceStatementNode node)
    {
        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(FunctionNode node)
    {
        _Context.AllocateScope();
        node.statements.ForEach(n => n.VisitSemanticAnaylsis(this));
        _Context.DeallocateScope();
        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(VaraibleDeclarationNode node)
    {
        if (node.ExpressionNode != null)
        {
            AssignedType = TokenToType(node.type);
            LacusType t = node.ExpressionNode.VisitSemanticAnaylsis(this);
            _Context.AddValue(node.name, new SemanticVar(TokenToType(node.type)));
            if (TypeOrdance[t.Type] > TypeOrdance[AssignedType.Type])
                throw new Exception("type error");
        }

        return new LacusType(TypeEnum.VOID);
    }

    private LacusType TokenToType(Tokens type)
    {
        return type.tokenType switch
        {
            TokenType.INT => new LacusType(TypeEnum.INTEGER),
            TokenType.FLOAT => new LacusType(TypeEnum.FLOAT),
            TokenType.CHAR => new LacusType(TypeEnum.CHAR),
            TokenType.WORD => HandleCustomType(type.buffer),
            _ => throw new Exception("non existent type")
        };
    }

    private LacusType HandleCustomType(string Type)
    {
        return new LacusType(TypeEnum.VOID);
    }
}
