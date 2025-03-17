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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Spatial.ShapeProfiles;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines the symmetry of the given IProfile about the X-Axis and Y-Axis.")]
        [Input("profile", "The layout object to query the points from.")]
        [Output("s", "The level of symmetry..")]
        public static Symmetry Symmetric(this IProfile profile, double tolerance = Tolerance.Distance)
        {
            // Check null
            if (profile.IsNull())
                return Symmetry.NonSymmetrical;

            bool symmetricX = true;
            bool symmetricY = true;

            Plane pXZ = Plane.XZ;
            Plane pYZ = Plane.YZ;

            // Get all the control points of the edges, and remove any duplicates
            List<Point> pts = profile.Edges.SelectMany(x => x.ControlPoints()).ToList().CullDuplicates(tolerance);

            // Iterate over each point and check an equivalent point exists in the reflection
            foreach (Point pt in pts)
            {
                if (symmetricX)
                {
                    symmetricX = Engine.Geometry.Query.Symmetric(pts, pt, pYZ, tolerance);
                }

                if (symmetricY)
                {
                    symmetricY = Engine.Geometry.Query.Symmetric(pts, pt, pXZ, tolerance);
                }
            }

            if (symmetricX && symmetricY)
                return Symmetry.DoublySymmetrical;
            else if (symmetricX && !symmetricY)
                return Symmetry.AsymmetricalX;
            else if (!symmetricX && symmetricY)
                return Symmetry.AsymmetricalY;
            else
                return Symmetry.NonSymmetrical;
        }

        /***************************************************/



        /***************************************************/

    }
}





