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
            List<List<Tuple<Polyline, Plane>>> curveClusters = new List<List<Tuple<Polyline, Plane>>>();
            foreach (Polyline c in curves)
            {
                bool coplanar = false;
                Tuple<Polyline, Plane> cp = new Tuple<Polyline, Plane>(c.Clone(), c.FitPlane());
                foreach (List<Tuple<Polyline, Plane>> cc in curveClusters)
                {
                    if (cp.Item2.IsCoplanar(cc[0].Item2))
                    {
                        cc.Add(cp);
                        coplanar = true;
                        break;
                    }
                }
                if (!coplanar) curveClusters.Add(new List<Tuple<Polyline, Plane>> { cp });
            }
            return curveClusters.Select(cc => cc.Select(c => c.Item1).ToList()).ToList();
        }

        /***************************************************/
    }
}
