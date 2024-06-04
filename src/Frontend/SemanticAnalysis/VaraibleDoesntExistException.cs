namespace Lexxer.SemanticAnalysis;

public class VaraibleDoesntExistException : Exception
{
    public VaraibleDoesntExistException(string message)
        : base(message) { }
}
