namespace LacusLLVM.SemanticAanylyzerVisitor;

public class VaraibleAlreadyDefinedException : Exception
{
    public VaraibleAlreadyDefinedException(string message)
        : base(message) { }
}
