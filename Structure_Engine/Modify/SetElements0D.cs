using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static Bar SetElements0D(this Bar bar, List<Node> newElements0D)
        {
            if (newElements0D.Count != 2)
            {
                Reflection.Compute.RecordError("A bar is defined by 2 nodes.");
                return null;
            }

            Bar clone = bar.GetShallowClone() as Bar;
            clone.StartNode = newElements0D[0];
            clone.EndNode = newElements0D[1];
            return clone;
        }

        /******************************************/
    }
}
