using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static Bar Set0DElements(this Bar bar, List<Node> new0DElements)
        {
            if (new0DElements.Count != 2)
            {
                Reflection.Compute.RecordError("A bar is defined by 2 nodes.");
                return null;
            }

            Bar clone = bar.GetShallowClone() as Bar;
            clone.StartNode = new0DElements[0];
            clone.EndNode = new0DElements[1];
            return clone;
        }

        /******************************************/
    }
}
