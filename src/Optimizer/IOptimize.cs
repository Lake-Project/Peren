public interface IOptimize
{
    public int Visit(IntegerNode node, Context context);
    public int Visit(FloatNode node, Context context);

    public int Visit(OpNode node, Context context);
    public int Visit(VaraibleReferenceNode node, Context context);

    public int Visit(FunctionCallNode node, Context context);
}
