using System.Collections.Generic;

public class Sequence : Composite
{
    public Sequence(List<Node> children) : base(children)
    {
    }

    public override ReturnValue Evaluate()
    {
        foreach (Node child in Children)
        {
            switch (child.Evaluate())
            {
                case ReturnValue.Success:
                    continue;
                case ReturnValue.Failure:
                    return ReturnValue.Failure;
            }
        }

        return ReturnValue.Success;
    }
}
