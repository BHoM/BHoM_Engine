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
        public static Symmetry ISymmetric(this IProfile profile, double tolerance = Tolerance.Distance)
        {
            if (profile.IsNull())
                return Symmetry.Asymmetric;

            return Symmetric(profile as dynamic, tolerance);
        }

        /***************************************************/

        private static Symmetry Symmetric(this IProfile profile, double tolerance = Tolerance.Distance)
        {
            Plane pXZ = Plane.XZ;
            Plane pYZ = Plane.YZ;

            // Get all the control points of the edges, and remove any duplicates
            List<Point> pts = profile.Edges.SelectMany(x => x.ControlPoints()).ToList().CullDuplicates(tolerance);

            // Iterate over each point and check an equivalent point exists in the reflection
            bool symmetricX = Engine.Geometry.Query.Symmetric(pts, pYZ, tolerance);
            bool symmetricY = Engine.Geometry.Query.Symmetric(pts, pXZ, tolerance);

            if (symmetricX && symmetricY)
                return Symmetry.DoublySymmetric;
            else if (symmetricX && !symmetricY)
                return Symmetry.SingleSymmetricMajor;
            else if (!symmetricX && symmetricY)
                return Symmetry.SinglySymmetricMinor;
            else
                return Symmetry.Asymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetric(this AngleProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.Asymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetric(this BoxProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.DoublySymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetric(this ChannelProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.SingleSymmetricMajor;
        }

        /***************************************************/

        private static Symmetry Symmetric(this CircleProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.DoublySymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetric(this KiteProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.SinglySymmetricMinor;
        }

        /***************************************************/

        private static Symmetry Symmetric(this RectangleProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.DoublySymmetric;
        }

        private static Symmetry Symmetric(this TaperedProfile profile, double tolerance = Tolerance.Distance)
        {
            List<Symmetry> symmetries = new List<Symmetry>();
            foreach (IProfile iProfile in profile.Profiles.Values)
            {
                symmetries.Add(Symmetric(iProfile as dynamic, tolerance));
            }

            List<Symmetry> distinct = symmetries.Distinct().ToList();

            if (distinct.Count() > 1)
                return Symmetry.Asymmetric;
            else if (distinct.First() == Symmetry.Asymmetric)
                return Symmetry.Asymmetric;
            else if (distinct.First() == Symmetry.SinglySymmetricMinor)
                return Symmetry.SinglySymmetricMinor;
            else if (distinct.First() == Symmetry.SingleSymmetricMajor)
                return Symmetry.SinglySymmetricMinor;

            return Symmetry.DoublySymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetric(this TSectionProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.SinglySymmetricMinor;
        }

        /***************************************************/

        private static Symmetry Symmetric(this TubeProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.DoublySymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetric(this ZSectionProfile profile, double tolerance = Tolerance.Distance)
        {
            return Symmetry.Asymmetric;
        }
    }
}





