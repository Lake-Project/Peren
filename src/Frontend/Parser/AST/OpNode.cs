using LacusLLVM.Frontend.Parser.AST;
using LacusLLVM.LLVMCodeGen.Visitors.StatementVisit;
using LacusLLVM.SemanticAanylyzerVisitor;
using Lexxer;
using LLVMSharp.Interop;

public class OpNode : INode
{
    public INode left;
    public INode right;
    public Tokens token;
    public bool FloatExpr;

    public OpNode(INode left, INode right)
    {
        this.left = left;
        this.right = right;
    }

    public OpNode(INode left, INode right, Tokens tokens)
    {
        this.left = left;
        this.right = right;
        this.token = tokens;
        FloatExpr = false;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        // return solve.Solve(this, builder, module);
        return visitor.Visit(this, builder, module, context);
    }

    public T Visit<T>(ExpressionVisit<T> visit)
    {
        return visit.Visit(this);
    }

    public override string ToString()
    {
        if (left == null && right != null)
        {
            return right.ToString() + " " + token.ToString();
        }
        else if (right == null && left != null)
        {
            return left.ToString() + " " + token.ToString();
        }
        else if (right != null && left != null)
        {
            return right.ToString() + " " + left.ToString() + " " + token.ToString();
        }
        else
        {
            return "NULL";
        }
    }
}
