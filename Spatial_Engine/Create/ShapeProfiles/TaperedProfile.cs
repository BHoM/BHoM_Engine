/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using System.Collections.ObjectModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.Engine.Reflection;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a TaperedProfile using a list of positions (parametric between 0 and 1) and list of profiles \n" +
            "corresponding to those positions.")]
        [Input("positions", "Describes the position of each profile parametrically (i.e. between 0 and 1) along the Bar it is assigned to. \n" +
            "The smallest position indicates the start profile and the largest position indicates the end profile.")]
        [Input("profiles", "The ShapeProfile at each of the positions specified. A profile must be provided at the start and end as a minimum.")]
        [Input("interpolationOrder", "Describes the order of the polynomial function between profiles whereby 1 = Linear, 2 = Quadratic, 3 = Cubic etc. " +
            "There should be one fewer (n-1) interpolation values than profiles. For nonlinear profiles a concave profile is achieved by setting the larger profile at the smallest position. " +
            "To achieve a convex profile, the larger profile must be at the largest position.")]
        [Output("taperedProfile", "The created TaperedProfile.")]
        public static TaperedProfile TaperedProfile(List<double> positions, List<IProfile> profiles, List<int> interpolationOrder = null)
        {
            //Checks for positions and profiles
            if (positions.Count != profiles.Count)
            {
                Base.Compute.RecordError("Number of positions and profiles provided are not equal");
                return null;
            }
            else if (positions.Exists((double d) => { return d > 1; }) || positions.Exists((double d) => { return d < 0; }))
            {
                Base.Compute.RecordError("Positions must exist between 0 and 1 (inclusive)");
                return null;
            }

            if (positions.Zip(positions.Skip(1), (a, b) => new { a, b }).Any(p => p.a > p.b))
            {
                Base.Compute.RecordError("Positions must be sorted in ascending order.");
                return null;
            }

            if (positions.First() != 0 || positions.Last() != 1)
            {
                Base.Compute.RecordError("Positions must include 0 (start) and 1 (end).");
                return null;
            }

            //Checks for interpolationOrder
            if (interpolationOrder == null || interpolationOrder.Count == 0)
            {
                interpolationOrder = Enumerable.Repeat(1, positions.Count - 1).ToList();
            }
            else if (interpolationOrder.Count == 1)
            {
                interpolationOrder = Enumerable.Repeat(interpolationOrder.First(), positions.Count - 1).ToList();
            }
            else if (!(interpolationOrder.Count == positions.Count - 1))
            {
                Base.Compute.RecordError("InterpolationOrder is between the profiles provided. Therefore, the number of interpolationOrder should be one less (n - 1) than the number of profiles/positions.");
                return null;
            }

            if (interpolationOrder.Any(x => x < 1))
            {
                Base.Compute.RecordError("The interpolationOrder values must be greater than 1.");
                return null;
            }

            //Create ditionary for TaperedProfile
            SortedDictionary<double, IProfile> profileDict = new SortedDictionary<double, IProfile>();
            for (int i = 0; i < positions.Count; i++)
            {
                profileDict[positions[i]] = profiles[i];
            }

            ShapeType shape = GetShapeType(profiles);
            TaperedProfile taperedProfile = new TaperedProfile(profileDict, interpolationOrder, shape);

            return taperedProfile;
        }

        /***************************************************/

        [Description("Creates a TaperedProfile using a start and end profile.")]
        [Input("startProfile", "The IProfile at the start of the Bar (i.e. position = 0).")]
        [Input("endProfile", "The IProfile at the end of the Bar (i.e. position = 1).")]
        [Input("interpolationOrder", "Describes the order of the polynomial function between profiles whereby 1 = Linear, 2 = Quadratic, 3 = Cubic etc. " +
            "There should be one fewer (n-1) interpolation values than profiles. For nonlinear profiles a concave profile is achieved by setting the larger profile at the smallest position. " +
            "To achieve a convex profile, the larger profile must be at the largest position.")]
        [Output("taperedProfile", "The created TaperedProfile.")]
        public static TaperedProfile TaperedProfile(IProfile startProfile, IProfile endProfile, int interpolationOrder = 1)
        {
            return TaperedProfile(new List<double>() { 0, 1 }, new List<IProfile>() { startProfile, endProfile }, new List<int>() { interpolationOrder });
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static ShapeType GetShapeType(List<IProfile> profiles)
        {
            ShapeType shape = profiles.First().Shape;

            return profiles.Any(x => x.Shape != shape) ? ShapeType.FreeForm : shape;
        }

        /***************************************************/

    }
}






