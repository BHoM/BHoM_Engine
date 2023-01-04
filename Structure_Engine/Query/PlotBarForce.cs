/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Results;
using BH.oM.Base.Attributes;
using BH.oM.Base;

using BH.Engine.Geometry;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> PlotBarForce(List<Bar> bars, List<BarForce> forces, Type adapterIdType, double scaleFactor = 1.0, object loadCase = null, bool fx = true, bool fy = true, bool fz = true, bool mx = true, bool my = true, bool mz = true)
        {
            if (adapterIdType == null)
            {
                Base.Compute.RecordError("The provided adapter id type is null.");
                return new List<ICurve>();
            }
            if (!typeof(IAdapterId).IsAssignableFrom(adapterIdType))
            {
                Base.Compute.RecordError($"The `{adapterIdType.Name}` is not a valid `{typeof(IAdapterId).Name}`.");
                return new List<ICurve>();
            }

            forces = forces.SelectCase(loadCase);

            List<ICurve> plots = new List<ICurve>();

            foreach (Bar bar in bars)
            {
                IAdapterId id = bar.FindFragment<IAdapterId>(adapterIdType);
                if (id == null)
                {
                    Engine.Base.Compute.RecordWarning("Could not find the adapter id for at least one Bar.");
                    continue;
                }

                string barId = id.Id.ToString();
                List<BarForce> elementForces = forces.Where(x => x.ObjectId.ToString() == barId).ToList();
                elementForces.Sort();
                plots.AddRange(PlotBarForce(bar, elementForces, scaleFactor, fx,fy,fz,mx,my,mz));
            }

            return plots;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> PlotBarForce(Bar bar, List<BarForce> forces, double scaleFactor = 1.0, bool fx = true, bool fy = true, bool fz = true, bool mx = true, bool my = true, bool mz = true)
        {
            Vector tan = bar.Tangent();
            Vector unitTan = tan.Normalise();
            Vector normal = bar.Normal();
            Vector yAxis = normal.CrossProduct(unitTan);

            scaleFactor /= 1000;

            List<Point> basePoints = forces.Select(x => bar.StartNode.Position + tan * x.Position).ToList();

            List<ICurve> plots = new List<ICurve>();

            if (fx) plots.AddRange(PlotSpecificForce(normal, basePoints, forces.Select(x => x.FX * scaleFactor).ToList()));
            if (fy) plots.AddRange(PlotSpecificForce(yAxis, basePoints, forces.Select(x => x.FY * scaleFactor).ToList()));
            if (fz) plots.AddRange(PlotSpecificForce(normal, basePoints, forces.Select(x => x.FZ * scaleFactor).ToList()));
            if (mx) plots.AddRange(PlotSpecificForce(normal, basePoints, forces.Select(x => x.MX * scaleFactor).ToList()));
            if (my) plots.AddRange(PlotSpecificForce(normal, basePoints, forces.Select(x => x.MY * scaleFactor).ToList()));
            if (mz) plots.AddRange(PlotSpecificForce(yAxis, basePoints, forces.Select(x => x.MZ * scaleFactor).ToList()));

            return plots;
        }

        /***************************************************/

        private static List<ICurve> PlotSpecificForce(Vector v, List<Point> pts, List<double> values)
        {
            List<Point> otherPTs = new List<Point>();

            for (int i = 0; i < pts.Count; i++)
            {
                otherPTs.Add(pts[i] + v * values[i]);
            }

            List<ICurve> curves = new List<ICurve>();

            for (int i = 0; i < pts.Count; i++)
            {
                curves.Add(Engine.Geometry.Create.Line(pts[i], otherPTs[i]));
            }

            curves.Add(Engine.Geometry.Create.Polyline(otherPTs));
            return curves;
        }

        /***************************************************/
    }
}




