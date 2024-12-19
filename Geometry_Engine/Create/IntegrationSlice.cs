/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IntegrationSlice IntegrationSlice(double width, double length, double centre, double[] placement)
        {
            return new IntegrationSlice
            {
                Width = width,
                Length = length,
                Centre = centre,
                Placement = placement
            };
        }

        /***************************************************/

        public static List<IntegrationSlice> IntegrationSlices(List<ICurve> edges, Vector direction, double increment = 0.001, double tolerance = Tolerance.Distance)
        {
            List<IntegrationSlice> slices = new List<IntegrationSlice>();

            if (edges.Count == 0)
                return slices;

            List<double> cutAt = new List<double>();
            List<double> sliceSegments = new List<double>();
            Plane p = new Plane { Origin = oM.Geometry.Point.Origin, Normal = direction };

            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < edges[i].IControlPoints().Count; j++)
                {
                    cutAt.Add(Query.DotProduct(Create.Vector(edges[i].IControlPoints()[j]), p.Normal));
                }
            }

            cutAt.Sort();
            cutAt = cutAt.Distinct<double>().ToList();

            double currentValue = Query.DotProduct(Create.Vector(Query.Bounds(new PolyCurve { Curves = edges }).Min), p.Normal);
            double max = Query.DotProduct(Create.Vector(Query.Bounds(new PolyCurve { Curves = edges }).Max), p.Normal);
            int index = 0;

            while (currentValue < max)
            {
                if (cutAt.Count > index && currentValue > cutAt[index])
                {
                    sliceSegments.Add(cutAt[index]);
                    index++;
                }
                else
                {
                    sliceSegments.Add(currentValue);
                    currentValue += increment;
                }
            }

            sliceSegments.Add(max);

            for (int i = 0; i < sliceSegments.Count - 1; i++)
            {
                if (sliceSegments[i] == sliceSegments[i + 1])
                {
                    continue;
                }

                currentValue = (sliceSegments[i] + sliceSegments[i + 1]) / 2;
                slices.Add(Query.SliceAt(edges, currentValue, -sliceSegments[i] + sliceSegments[i + 1], p, tolerance));
            }

            return slices;
        }

        /***************************************************/
    }
}






