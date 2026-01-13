public class IsFrozen : Decorator
{
    private Freezable _freezable;
    
    public IsFrozen(Freezable freezable, Node child) : base(child)
    {
        _freezable = freezable;
    }

    public override ReturnValue Evaluate()
    {
        if (_freezable.IsFrozen)
            return Child.Evaluate();
        
        return ReturnValue.Failure;
    }
}
