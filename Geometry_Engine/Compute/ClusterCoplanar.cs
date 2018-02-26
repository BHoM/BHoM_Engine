using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****         public Methods - Vectors          ****/
        /***************************************************/

        public static List<List<Plane>> ClusterCoplanar(this List<Plane> planes)
        {
            List<List<Plane>> planeClusters = new List<List<Plane>>();
            foreach (Plane p in planes)
            {
                bool coplanar = false;
                foreach (List<Plane> pp in planeClusters)
                {
                    if (p.IsCoplanar(pp[0]))
                    {
                        pp.Add(p.Clone());
                        coplanar = true;
                        break;
                    }
                }
                if (!coplanar) planeClusters.Add(new List<Plane> { p.Clone() });
            }
            return planeClusters;
        }


        /***************************************************/
        /****          public Methods - Curves          ****/
        /***************************************************/

        public static List<List<Polyline>> ClusterCoplanar(this List<Polyline> curves)
        {
            List<List<Polyline>> curveClusters = new List<List<Polyline>>();
            foreach (Polyline p in curves)
            {
                bool coplanar = false;
                foreach (List<Polyline> pp in curveClusters)
                {
                    if (p.IsCoplanar(pp[0]))
                    {
                        pp.Add(p.Clone());
                        coplanar = true;
                        break;
                    }
                }
                if (!coplanar) curveClusters.Add(new List<Polyline> { p.Clone() });
            }
            return curveClusters;
        }

        /***************************************************/
    }
}
