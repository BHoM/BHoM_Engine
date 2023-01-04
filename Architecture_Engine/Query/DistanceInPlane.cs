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

using BH.Engine.Geometry;
using BH.oM.Architecture.BuildersWork;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Architecture
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculate planar distance between two openings (BH.oM.Architecture.BuildersWork.Opening). Method checks if openings are parallel, on the same plane and closer than expected maximum distance.")]
        [Input("opening1", "First opening.")]
        [Input("opening2", "Second opening.")]
        [Input("maxExpectedDistance", "Maximum expected distance between openings. If no value is provided, maximum distance will not be checked.")]
        [Input("distanceTolerance", "Tolerance for distance between openings.")]
        [Input("angleTolerance", "Tolerance for openings parallelism calculations.")]
        [Output("distanceInPlane", "Distance between two openings.")]
        public static double DistanceInPlane(this Opening opening1, Opening opening2, double maxExpectedDistance = double.NaN, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (opening1 == null || opening2 == null)
            {
                BH.Engine.Base.Compute.RecordError("One of the openings is null, distance cannot be caluclated.");
                return double.NaN;
            }

            if (!opening1.IsValidForInPlaneDistanceCheck(opening2, maxExpectedDistance, distanceTolerance, angleTolerance)) 
            {
                BH.Engine.Base.Compute.RecordWarning("Openings do not fulfil preliminary requirements for distance check.");
                return double.NaN;
            }

            double minDistance = double.MaxValue;
            Point centerPoint1 = opening1.CoordinateSystem.Origin;
            Plane plane1 = new Plane { Origin = centerPoint1, Normal = opening1.CoordinateSystem.Z };
            Cartesian global = new Cartesian();
            List<ICurve> edges1 = opening1.Profile.Edges.Select(x => x.Orient(global, opening1.CoordinateSystem)).ToList();
            List<ICurve> edges2 = opening2.Profile.Edges.Select(x => x.Orient(global, opening2.CoordinateSystem).IProject(plane1)).ToList();            
            foreach (ICurve edge1 in edges1)
            {
                foreach (ICurve edge2 in edges2)
                {
                    double dist = edge1.Distance(edge2);
                    minDistance = Math.Min(minDistance, dist);
                }
            }

            return minDistance;
        }

        /***************************************************/
    }
}




