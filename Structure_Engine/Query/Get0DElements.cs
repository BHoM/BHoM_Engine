using BH.oM.Common;
using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static List<IElement0D> Get0DElements(this Bar bar)
        {
            return new List<IElement0D> { bar.StartNode, bar.EndNode };
        }

        /******************************************/
    }
}
