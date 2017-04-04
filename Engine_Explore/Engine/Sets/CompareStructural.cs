using BHoM.Base;
using Engine_Explore.BHoM.Materials;
using Engine_Explore.BHoM.Structural.Elements;
using Engine_Explore.BHoM.Structural.Properties;
using Engine_Explore.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Engine.Sets
{
    public static partial class Compare
    {
        public static double Identity(BHoMObject a, BHoMObject b)
        {
            if (a.BHoM_Guid == b.BHoM_Guid)
                return 1;
            else if (a.Name == b.Name && a.Name.Length > 0)
                return 0.75;
            else
                return 0;
        }

        /***************************************************/

        public static double Value(Node a, Node b) //TODO
        {
            return 1 - Measure.Distance(a.Point, b.Point) / BHoM.Base.Tolerance.MIN_DIST;
        }

        /***************************************************/

        public static double Value(Material a, Material b) //TODO
        {
            double weight = 0;

            if (a.Type == b.Type) weight += 1; 

            return weight;
        }

        /***************************************************/

        public static double Value(SectionProperty a, SectionProperty b)  //TODO
        {
            double weight = 0;

            if (a.Shape == b.Shape) weight += 0.5;
            weight += 0.5 * Value(a.Material, b.Material);

            return weight;
        }

        /***************************************************/

        public static double Value(Bar a, Bar b) //TODO
        {
            double weight = 0;

            weight += 0.35 * Value(a.StartNode, b.StartNode);
            weight += 0.35 * Value(a.EndNode, b.EndNode);
            weight += 0.30 * Value(a.SectionProperty, b.SectionProperty);

            return weight;
        }
    }
}
