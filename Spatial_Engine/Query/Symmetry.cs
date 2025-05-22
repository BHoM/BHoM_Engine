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
using SP = BH.oM.Spatial.ShapeProfiles;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines the symmetry of the given IProfile about the X-Axis and Y-Axis.")]
        [Input("profile", "The layout object to query the points from.")]
        [Output("s", "The level of symmetry.")]
        public static SP.Symmetry ISymmetry(this IProfile profile, double tolerance = Tolerance.Distance)
        {
            if (profile.IsNull())
                return SP.Symmetry.Asymmetric;

            return Symmetry(profile as dynamic, tolerance);
        }

        /***************************************************/

        private static Symmetry Symmetry(this IProfile profile, double tolerance = Tolerance.Distance)
        {
            Plane pXZ = Plane.XZ;
            Plane pYZ = Plane.YZ;

            // Get all the control points of the edges, and remove any duplicates
            List<Point> pts = profile.Edges.SelectMany(x => x.ControlPoints()).ToList().CullDuplicates(tolerance);

            // Iterate over each point and check an equivalent point exists in the reflection
            bool symmetricX = Engine.Geometry.Query.IsSymmetric(pts, pYZ, tolerance);
            bool symmetricY = Engine.Geometry.Query.IsSymmetric(pts, pXZ, tolerance);

            if (symmetricX && symmetricY)
                return SP.Symmetry.DoublySymmetric;
            else if (symmetricX && !symmetricY)
                return SP.Symmetry.SingleSymmetricMajor;
            else if (!symmetricX && symmetricY)
                return SP.Symmetry.SinglySymmetricMinor;
            else
                return SP.Symmetry.Asymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetry(this AngleProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.Asymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetry(this BoxProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.DoublySymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetry(this ChannelProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.SingleSymmetricMajor;
        }

        /***************************************************/

        private static Symmetry Symmetry(this CircleProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.DoublySymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetry(this KiteProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.SinglySymmetricMinor;
        }

        /***************************************************/

        private static Symmetry Symmetry(this RectangleProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.DoublySymmetric;
        }

        private static Symmetry Symmetry(this TaperedProfile profile, double tolerance = Tolerance.Distance)
        {
            List<Symmetry> symmetries = new List<Symmetry>();
            foreach (IProfile iProfile in profile.Profiles.Values)
            {
                symmetries.Add(Symmetry(iProfile as dynamic, tolerance));
            }

            List<Symmetry> distinct = symmetries.Distinct().ToList();

            if (distinct.Count() > 1)
                return SP.Symmetry.Asymmetric;
            else if (distinct.First() == SP.Symmetry.Asymmetric)
                return SP.Symmetry.Asymmetric;
            else if (distinct.First() == SP.Symmetry.SinglySymmetricMinor)
                return SP.Symmetry.SinglySymmetricMinor;
            else if (distinct.First() == SP.Symmetry.SingleSymmetricMajor)
                return SP.Symmetry.SinglySymmetricMinor;

            return SP.Symmetry.DoublySymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetry(this TSectionProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.SinglySymmetricMinor;
        }

        /***************************************************/

        private static Symmetry Symmetry(this TubeProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.DoublySymmetric;
        }

        /***************************************************/

        private static Symmetry Symmetry(this ZSectionProfile profile, double tolerance = Tolerance.Distance)
        {
            return SP.Symmetry.Asymmetric;
        }
    }
}





