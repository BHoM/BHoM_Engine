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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the void area enclosed by an IProfile. This assumes that the outermost curve(s) are solid. Curves inside a solid region are assumed to be openings, and curves within openings are assumed to be solid, etc. Also, for TaperedProfiles, the average void area is returned.")]
        [Input("profile", "The IProfile to evaluate.")]
        [Output("area", "The void area enclosed by the solid regions in the profile", typeof(Area))]
        public static double VoidArea(this IProfile profile)
        {
            if (profile is TaperedProfile)
            {
                Engine.Base.Compute.RecordWarning("The sectional area of TaperedProfiles may vary along their length. The average area of the TaperedProfile has been returned, assuming that the section varies linearly.");
                TaperedProfile taperedProfile = profile as TaperedProfile;
                double sum = 0;
                for (int i = 0; i < taperedProfile.Profiles.Count - 1; i++)
                {
                    double temArea = (taperedProfile.Profiles.ElementAt(i).Value.VoidArea() + taperedProfile.Profiles.ElementAt(i + 1).Value.VoidArea()) / 2;
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

            curvesZ = curvesZ.Where((x, i) => depth[i] != 0).ToList();
            depth = depth.Where(x => x != 0).ToArray();

            // Using region integration as the Curves are defined on the XY-Plane.
            depth = depth.Select(x => x % 2 != 0 ? 1 : -1).ToArray();   // positive area as 1 and negative area as -1
            return curvesZ.Select((x, i) => Math.Abs(x.IIntegrateRegion(0)) * depth[i]).Sum();
        }

        /***************************************************/
    }
}




