/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Queries the IElement2Ds area defined as the area confined by the outline curves subtracting the area of the internal elements.")]
        [Input("element2D", "The IElement2D to query the area of.")]
        [Output("area", "The area of the region confined by the IElement2Ds outline elements subtracting the area of the internal elements", typeof(Area))]
        public static double Area(this IElement2D element2D)
        {
            double result = element2D.OutlineCurve().IArea();

            List<PolyCurve> openings = element2D.InternalOutlineCurves().BooleanUnion();

            foreach (PolyCurve o in openings)
                result -= o.Area();

            return result;
        }


        /******************************************/
        /****            IProfile              ****/
        /******************************************/

        [Description("Gets the area for a IProfile where the curves are defining a region's border and eachother step inward defines a hole's border.")]
        [Input("profile", "The IProfile to evaluate the area of. TaperedProfiles average area is returned.")]
        [Output("area", "The area for eached closed curve, subtracting eachother closed curve's area stepping inwards.", typeof(Area))]
        public static double Area(this IProfile profile)
        {
            if (profile is TaperedProfile)
            {
                Engine.Reflection.Compute.RecordWarning("TaperedProfiles don't have one area. Linear relation between the profiles in the TaperedProfile has been assumed and it's average area returned.");
                TaperedProfile taperedProfile = profile as TaperedProfile;
                double sum = 0;
                for (int i = 0; i < taperedProfile.Profiles.Count - 1; i++)
                {
                    double temArea = (taperedProfile.Profiles.ElementAt(i).Value.Area() + taperedProfile.Profiles.ElementAt(i + 1).Value.Area()) / 2;
                    sum += temArea * System.Convert.ToDouble(taperedProfile.Profiles.ElementAt(i + 1).Key - taperedProfile.Profiles.ElementAt(i).Key);
                }
                return sum;
            }

            List<PolyCurve> curvesZ = Engine.Geometry.Compute.IJoin(profile.Edges.ToList());

            int[] depth = new int[curvesZ.Count];
            if (curvesZ.Count > 1)
            {
                // find which is in which
                for (int i = 0; i < curvesZ.Count; i++)
                    for (int j = 0; j < curvesZ.Count; j++)
                        if (i != j)
                            if (curvesZ[i].IsContaining(new List<Point>() { curvesZ[j].IStartPoint() }))
                                depth[j]++;
            }

            // Using region integration as the Curves are defined on the XY-Plane.
            depth = depth.Select(x => x % 2 == 0 ? 1 : -1).ToArray();   // positive area as 1 and negative area as -1
            return curvesZ.Select((x, i) => Math.Abs(x.IIntegrateRegion(0)) * depth[i]).Sum();
        }

        /******************************************/
    }
}

