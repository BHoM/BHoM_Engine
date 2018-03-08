using System.Collections.Generic;
using BH.oM.Geometry;
using BHE = BH.Engine.Geometry;
using System;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<Polyline>> DistributeOutlines(this List<Polyline> outlines)
        {
            outlines.Sort(delegate (Polyline p1, Polyline p2)
            {
                return p1.Area().CompareTo(p2.Area());
            });
            outlines.Reverse();

            List<Tuple<Polyline, bool>> outlinesByType = new List<Tuple<Polyline, bool>>();
            foreach (Polyline o in outlines)
            {
                bool assigned = false;
                for (int i = outlinesByType.Count - 1; i >= 0; i--)
                {
                    if (outlinesByType[i].Item1.IsContaining(o.ControlPoints, true))
                    {
                        outlinesByType.Add(new Tuple<Polyline, bool>(o, !outlinesByType[i].Item2));
                        assigned = true;
                        break;
                    }
                }
                if (!assigned)
                {
                    outlinesByType.Add(new Tuple<Polyline, bool>(o, true));
                }
            }
            List<Polyline> panelOutlines = outlinesByType.Where(x => x.Item2 == true).Select(x => x.Item1).ToList();
            List<Polyline> panelOpenings = outlinesByType.Where(x => x.Item2 == false).Select(x => x.Item1).ToList();
            return panelOutlines.DistributeOpenings(panelOpenings);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<List<Polyline>> DistributeOpenings(this List<Polyline> panels, List<Polyline> openings)
        {
            List<List<Polyline>> result = new List<List<Polyline>>();
            foreach (Polyline p in panels)
            {
                result.Add(new List<Polyline> { p.Clone() });
            }
            result.Reverse();

            foreach (Polyline o in openings)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i][0].IsContaining(o.ControlPoints, true))
                    {
                        result[i].Add(o);
                        break;
                    }
                }
            }
            return result;
        }
    }
}
