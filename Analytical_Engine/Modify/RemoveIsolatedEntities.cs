using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/
        public static Graph RemoveIsolatedEntities(this Graph graph)
        {
            Graph clone = graph.DeepClone();
            foreach (Guid n in clone.IsolatedEntities())
                clone.Entities.Remove(n);

            return clone;
        }
        /***************************************************/
    }
}
