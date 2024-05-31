using System.Diagnostics;
using LacusLLVM.Frontend.Parser.AST;
using Lexxer;

namespace LacusLLVM.SemanticAanylyzerVisitor;

public struct SemanticVar
{
    public LacusType VarType { get; set; }
    public int ScopeLocation;

    public SemanticVar(LacusType type, int scopeLocation)
    {
        VarType = type;
        this.ScopeLocation = ScopeLocation;
    }
}

public class SemanticVisitor : SemanticVisit
{
    // private Context _context;
    private SemanticContext<SemanticVar> _Context;
    public LacusType AssignedType;
    public LacusType CurrentFuctionType;

    public Dictionary<TypeEnum, int> TypeOrdance =
        new()
        {
            [TypeEnum.BOOL] = 0,
            [TypeEnum.CHAR] = 1,
            [TypeEnum.INTEGER] = 2,
            [TypeEnum.FLOAT] = 3,
        };

    public SemanticVisitor(SemanticContext<SemanticVar> context)
    {
        // this._context = context;
        _Context = context;
        AssignedType = new LacusType(TypeEnum.VOID);
        CurrentFuctionType = new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(OpNode node)
    {
        LacusType LType = node.right.VisitSemanticAnaylsis(this);
        LacusType RType = node.left.VisitSemanticAnaylsis(this);
        if (AssignedType.Type == TypeEnum.FLOAT)
        {
            node.FloatExpr = true;
        }

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
        SemanticVar v = _Context.GetValue(node.name);
        node.ScopeLocation = v.ScopeLocation;
        return v.VarType;
    }

    public override LacusType SemanticAccept(VaraibleReferenceStatementNode node)
    {
        SemanticVar v = _Context.GetValue(node.name);
        node.ScopeLocation = v.ScopeLocation;
        LacusType t = node.expression.VisitSemanticAnaylsis(this);
        if (TypeOrdance[t.Type] > TypeOrdance[AssignedType.Type])
            throw new Exception("type error");
        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(FunctionNode node)
    {
        _Context.AllocateScope();
        CurrentFuctionType = TokenToType(node.returType);
        node.Parameters.ForEach(n => n.VisitSemanticAnaylsis(this));
        node.statements.ForEach(n => n.VisitSemanticAnaylsis(this));
        CurrentFuctionType = new LacusType(TypeEnum.VOID);

        _Context.DeallocateScope();
        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(VaraibleDeclarationNode node)
    {
        AssignedType = TokenToType(node.type);
        SemanticVar s = new SemanticVar(AssignedType, _Context.GetSize());
        _Context.AddValue(node.name, s);
        if (node.ExpressionNode != null)
        {
            LacusType t = node.ExpressionNode.VisitSemanticAnaylsis(this);
            if (TypeOrdance[t.Type] > TypeOrdance[AssignedType.Type])
                throw new Exception("type error");
        }

        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(ReturnNode node)
    {
        if (CurrentFuctionType.Type == TypeEnum.VOID && node.expression != null)
            throw new Exception("type error");
        if (node.expression == null && CurrentFuctionType.Type != TypeEnum.VOID)
            throw new Exception("type error");
        if (node.expression != null)
        {
            LacusType t = node.expression.VisitSemanticAnaylsis(this);
            if (TypeOrdance[t.Type] > TypeOrdance[CurrentFuctionType.Type])
                throw new Exception("type error");
        }

        return new LacusType(TypeEnum.VOID);
    }

    public override LacusType SemanticAccept(CharNode node)
    {
        return new LacusType(TypeEnum.CHAR);
    }

    public override LacusType SemanticAccept(BoolNode node)
    {
        return new LacusType(TypeEnum.BOOL);
    }

    private LacusType TokenToType(Tokens type)
    {
        return type.tokenType switch
        {
            TokenType.INT => new LacusType(TypeEnum.INTEGER),
            TokenType.FLOAT => new LacusType(TypeEnum.FLOAT),
            TokenType.CHAR => new LacusType(TypeEnum.CHAR),
            TokenType.BOOL => new LacusType(TypeEnum.BOOL),
            TokenType.VOID => new LacusType(TypeEnum.VOID),
            TokenType.WORD => HandleCustomType(type.buffer),
            _ => throw new Exception("non existent type")
        };
    }

    private LacusType HandleCustomType(string Type)
    {
        return new LacusType(TypeEnum.VOID);
    }
}
