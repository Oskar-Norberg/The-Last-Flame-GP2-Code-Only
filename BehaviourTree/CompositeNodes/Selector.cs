using System.Collections.Generic;

public class Selector : Composite
{
    public Selector(List<Node> children) : base(children)
    {
    }
    
    public override ReturnValue Evaluate()
    {
        foreach (Node child in Children)
        {
            switch (child.Evaluate())
            {
                case ReturnValue.Success:
                    return ReturnValue.Success;
                case ReturnValue.Failure:
                    continue;
            }
        }

        return ReturnValue.Failure;
    }
}

