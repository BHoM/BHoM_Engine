using BH.oM.Common;
using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static Bar SetElements0D(this Bar bar, List<IElement0D> newElements0D)
        {
            if (newElements0D.Count != 2)
            {
                Reflection.Compute.RecordError("A bar is defined by 2 nodes.");
                return null;
            }

            Bar clone = bar.GetShallowClone() as Bar;
            clone.StartNode = newElements0D[0] as Node;
            clone.EndNode = newElements0D[1] as Node;
            return clone;
        }

        /******************************************/
    }
}
