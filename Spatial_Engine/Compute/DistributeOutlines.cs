/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Compute
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static List<List<List<IElement1D>>> DistributeOutlines(this List<List<IElement1D>> outlines, bool canCutOpenings = true, double tolerance = Tolerance.Distance)
        {
            List<Tuple<PolyCurve, List<IElement1D>>> outlineCurves = new List<Tuple<PolyCurve, List<IElement1D>>>();
            foreach (List<IElement1D> outline in outlines)
            {
                PolyCurve outlineCurve = new PolyCurve { Curves = outline.Select(x => x.IGeometry()).ToList() };
                outlineCurves.Add(new Tuple<PolyCurve, List<IElement1D>>(outlineCurve, outline));
            }
            
            outlineCurves.Sort(delegate (Tuple<PolyCurve, List<IElement1D>> t1, Tuple<PolyCurve, List<IElement1D>> t2)
            {
                return Geometry.Query.Area(t1.Item1).CompareTo(Geometry.Query.Area(t2.Item1));
            });
            outlineCurves.Reverse();

            List<Tuple<PolyCurve, List<IElement1D>, bool>> outlinesByType = new List<Tuple<PolyCurve, List<IElement1D>, bool>>();
            foreach (Tuple<PolyCurve, List<IElement1D>> o in outlineCurves)
            {
                bool assigned = false;
                for (int i = outlinesByType.Count - 1; i >= 0; i--)
                {
                    if (outlinesByType[i].Item1.IsContaining(o.Item1, true, tolerance))
                    {
                        if (canCutOpenings || i == 0)
                            outlinesByType.Add(new Tuple<PolyCurve, List<IElement1D>, bool>(o.Item1, o.Item2, !outlinesByType[i].Item3));

                        assigned = true;
                        break;
                    }
                }

                if (!assigned)
                    outlinesByType.Add(new Tuple<PolyCurve, List<IElement1D>, bool>(o.Item1, o.Item2, true));
            }

            List<Tuple<PolyCurve, List<IElement1D>>> panelOutlines = outlinesByType.Where(x => x.Item3 == true).Select(x => new Tuple<PolyCurve, List<IElement1D>>(x.Item1, x.Item2)).ToList();
            List<Tuple<PolyCurve, List<IElement1D>>> panelOpenings = outlinesByType.Where(x => x.Item3 == false).Select(x => new Tuple<PolyCurve, List<IElement1D>>(x.Item1, x.Item2)).ToList();
            return panelOutlines.DistributeOpenings(panelOpenings, tolerance);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<List<List<IElement1D>>> DistributeOpenings(this List<Tuple<PolyCurve, List<IElement1D>>> panels, List<Tuple<PolyCurve, List<IElement1D>>> openings, double tolerance)
        {
            double sqTolerance = tolerance * tolerance;
            List<List<Tuple<PolyCurve, List<IElement1D>>>> result = new List<List<Tuple<PolyCurve, List<IElement1D>>>>();
            foreach (Tuple<PolyCurve, List<IElement1D>> panel in panels)
            {
                result.Add(new List<Tuple<PolyCurve, List<IElement1D>>> { panel });
            }
            result.Reverse();

            foreach (Tuple<PolyCurve, List<IElement1D>> opening in openings)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i][0].Item1.IsContaining(opening.Item1, true, tolerance) && Geometry.Query.Area(result[i][0].Item1) - Geometry.Query.Area(opening.Item1) > sqTolerance)
                    {
                        result[i].Add(opening);
                        break;
                    }
                }
            }

            return result.Select(x => x.Select(y => y.Item2).ToList()).ToList();
        }

        /***************************************************/
    }
}





