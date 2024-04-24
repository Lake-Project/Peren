using LLVMSharp.Interop;

public class VaraibleReferenceStatementNode : INode
{
    public INode expression;
    public string name;

    public VaraibleReferenceStatementNode(string name, INode expresion)
    {
        this.name = name;
        this.expression = expresion;
    }

    public LLVMValueRef CodeGen(
        IVisitor visitor,
        LLVMBuilderRef builder,
        LLVMModuleRef module,
        Context context
    )
    {
        unsafe
        {
            Var b = context.GetVar(name);
            // var typeRef = LLVM.TypeOf(b);
            // LLVMTypeRef pointedType = LLVM.GetElementType(b.type);
            if (
                b.type == LLVMTypeRef.Int32
                || b.type == LLVMTypeRef.Int8
                || b.type == LLVMTypeRef.Int1
            )
            {
                return builder.BuildStore(
                    expression.CodeGen(new IntegerExpressionVisitor(), builder, module, context),
                    b.valueRef
                );
            }
            else
            {
                return builder.BuildStore(
                    expression.CodeGen(new CodeGenVisitor(), builder, module, context),
                    b.valueRef
                );
            }
        }

        // throw new NotImplementedException();
    }
}
