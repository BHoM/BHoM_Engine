/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Reflection.Attributes;
using BH.oM.Geometry;
using BH.Engine.Base;
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;


namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Distributes outlines into lists representing containing outlines and the outlines that they each contain.")]
        [Input("outlines", "The outlines to sort.")]
        [Input("tolerance", "The tolerance to apply when detecting if outlines are closed and contained.")]
        [Output("distributedOutlines", "Lists representing each set of outlines. For each list, the first outline is the containing outline and the remainder are the outlines it contains.")]
        public static List<List<Polyline>> DistributeOutlines(this List<Polyline> outlines, double tolerance = Tolerance.Distance)
        {
            if (outlines == null || outlines.Where(x => x != null).Count() == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute a null list of outlines.");
                return new List<List<Polyline>>();
            }

            if (tolerance == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute outlines as tolerance is null.");
                return new List<List<Polyline>>();
            }

            foreach (Polyline p in outlines)
            {
                if (!p.IsClosed(tolerance))
                    throw new Exception("All curves need to be closed to create a panel");
            }

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
                    if (outlinesByType[i].Item1.IsContaining(o.ControlPoints, true, tolerance))
                    {
                        outlinesByType.Add(new Tuple<Polyline, bool>(o, !outlinesByType[i].Item2));
                        assigned = true;
                        break;
                    }
                }

                if (!assigned)
                    outlinesByType.Add(new Tuple<Polyline, bool>(o, true));
            }

            List<Polyline> panelOutlines = outlinesByType.Where(x => x.Item2 == true).Select(x => x.Item1.DeepClone()).ToList();
            List<Polyline> panelOpenings = outlinesByType.Where(x => x.Item2 == false).Select(x => x.Item1.DeepClone()).ToList();
            return panelOutlines.DistributeOpenings(panelOpenings, tolerance);
        }

        /***************************************************/

        [Description("Distributes outlines into lists representing containing outlines and the outlines that they each contain.")]
        [Input("outlines", "The outlines to sort.")]
        [Input("tolerance", "The tolerance to apply when detecting if outlines are closed and contained.")]
        [Output("distributedOutlines", "Lists representing each set of outlines. For each list, the first outline is the containing outline and the remainder are the outlines it contains.")]
        public static List<List<ICurve>> DistributeOutlines(this List<ICurve> outlines, double tolerance = Tolerance.Distance)
        {
            if (outlines == null || outlines.Where(x => x != null).Count() == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute a null list of outlines.");
                return new List<List<ICurve>>();
            }

            if (tolerance == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute outlines as tolerance is null.");
                return new List<List<ICurve>>();
            }

            foreach (ICurve p in outlines)
            {
                if (!p.IIsClosed(tolerance))
                    throw new Exception("All curves need to be closed to create a panel");
            }

            outlines.Sort(delegate (ICurve p1, ICurve p2)
            {
                return p1.IArea(tolerance).CompareTo(p2.IArea(tolerance));
            });
            outlines.Reverse();

            List<Tuple<ICurve, bool>> outlinesByType = new List<Tuple<ICurve, bool>>();
            foreach (ICurve o in outlines)
            {
                bool assigned = false;
                for (int i = outlinesByType.Count - 1; i >= 0; i--)
                {
                    if (outlinesByType[i].Item1.IIsContaining(o, true, tolerance))
                    {
                        outlinesByType.Add(new Tuple<ICurve, bool>(o, !outlinesByType[i].Item2));
                        assigned = true;
                        break;
                    }
                }

                if (!assigned)
                    outlinesByType.Add(new Tuple<ICurve, bool>(o, true));
            }
            List<ICurve> panelOutlines = outlinesByType.Where(x => x.Item2 == true).Select(x => x.Item1.DeepClone()).ToList();
            List<ICurve> panelOpenings = outlinesByType.Where(x => x.Item2 == false).Select(x => x.Item1.DeepClone()).ToList();
            return panelOutlines.DistributeOpenings(panelOpenings, tolerance);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Distributes outlines into lists representing containing outlines and the outlines that they each contain.")]
        [Input("panels", "The Polylines representing the containing outlines.")]
        [Input("openings", "The Polylines representing the contained outlines.")]
        [Input("tolerance", "The tolerance to apply when detecting if outlines are closed and contained.")]
        [Output("distributedOutlines", "Lists representing each set of outlines. For each list, the first outline is the containing outline and the remainder are the outlines it contains.")]
        private static List<List<Polyline>> DistributeOpenings(this List<Polyline> panels, List<Polyline> openings, double tolerance = Tolerance.Distance)
        {
            if (panels == null || panels.Where(x => x != null).Count() == 0 || openings == null || openings.Where(x => x != null).Count() == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute a null list of panels/openings.");
                return new List<List<Polyline>>();
            }

            if (tolerance == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute outlines as tolerance is null.");
                return new List<List<Polyline>>();
            }

            List<List<Polyline>> result = new List<List<Polyline>>();
            foreach (Polyline p in panels)
            {
                result.Add(new List<Polyline> { p });
            }
            result.Reverse();

            foreach (Polyline o in openings)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i][0].IsContaining(o.ControlPoints, true, tolerance))
                    {
                        result[i].Add(o);
                        break;
                    }
                }
            }

            return result;
        }

        /***************************************************/

        [Description("Distributes outlines into lists representing containing outlines and the outlines that they each contain.")]
        [Input("panels", "The PolyCurves representing the containing outlines.")]
        [Input("openings", "The PolyCurves representing the contained outlines.")]
        [Input("tolerance", "The tolerance to apply when detecting if outlines are closed and contained.")]
        [Output("distributedOutlines", "Lists representing each set of outlines. For each list, the first outline is the containing outline and the remainder are the outlines it contains.")]
        private static List<List<PolyCurve>> DistributeOpenings(this List<PolyCurve> panels, List<PolyCurve> openings, double tolerance = Tolerance.Distance)
        {
            if (panels == null || panels.Where(x => x != null).Count() == 0 || openings == null || openings.Where(x => x != null).Count() == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute a null list of panels/openings.");
                return new List<List<PolyCurve>>();
            }

            if (tolerance == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute outlines as tolerance is null.");
                return new List<List<PolyCurve>>();
            }

            List<List<PolyCurve>> result = new List<List<PolyCurve>>();
            foreach (PolyCurve p in panels)
            {
                result.Add(new List<PolyCurve> { p });
            }
            result.Reverse();

            foreach (PolyCurve o in openings)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i][0].IsContaining(o, true, tolerance))
                    {
                        result[i].Add(o);
                        break;
                    }
                }
            }

            return result;
        }

        /***************************************************/

        [Description("Distributes outlines into lists representing containing outlines and the outlines that they each contain.")]
        [Input("panels", "The curves representing the containing outlines.")]
        [Input("openings", "The curves representing the contained outlines.")]
        [Input("tolerance", "The tolerance to apply when detecting if outlines are closed and contained.")]
        [Output("distributedOutlines", "Lists representing each set of outlines. For each list, the first outline is the containing outline and the remainder are the outlines it contains.")]
        private static List<List<ICurve>> DistributeOpenings(this List<ICurve> panels, List<ICurve> openings, double tolerance = Tolerance.Distance)
        {
            if (panels == null || panels.Where(x => x != null).Count() == 0 || openings == null || openings.Where(x => x != null).Count() == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot distribute a null list of panels/openings.");
                return new List<List<ICurve>>();
            }

            List<List<ICurve>> result = new List<List<ICurve>>();
            foreach (ICurve p in panels)
            {
                result.Add(new List<ICurve> { p });
            }
            result.Reverse();

            foreach (ICurve o in openings)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i][0].IIsContaining(o, true, tolerance))
                    {
                        result[i].Add(o);
                        break;
                    }
                }
            }

            return result;
        }

        /***************************************************/
    }
}


