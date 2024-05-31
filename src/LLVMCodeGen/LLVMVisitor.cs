using LacusLLVM.Frontend.Parser.AST;
using LLVMSharp.Interop;

namespace LacusLLVM.LLVMCodeGen;

public abstract class LLVMVisitor
{
    public abstract LLVMValueRef Visit(IntegerNode node);
    public abstract LLVMValueRef Visit(FloatNode node);
    public abstract LLVMValueRef Visit(BoolNode node);
    public abstract LLVMValueRef Visit(VaraibleDeclarationNode node);
    public abstract LLVMValueRef Visit(VaraibleReferenceNode node);
    public abstract LLVMValueRef Visit(VaraibleReferenceStatementNode node);
}
