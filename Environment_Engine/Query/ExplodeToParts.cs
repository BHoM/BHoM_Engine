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
using System.Collections.Generic;
using System.Linq;
using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the sides, top and bottom of a given environment object.")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have geometrical sides, top and bottom.")]
        [Input("distanceTolerance", "Distance tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Distance.")]
        [Input("angleTolerance", "Angle tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Angle.")]
        [Input("numericTolerance", "Tolerance for determining whether a calulated number is within a range defined by the tolerance, default is set to the value defined by BH.oM.Geometry.Tolerance.Distance.")]
        [MultiOutput(0, "bottom", "An ICurve representation of the bottom of the object.")]
        [MultiOutput(1, "sides", "ICurve representations of the sides of the object.")]
        [MultiOutput(2, "top", "An ICurve representation of the top of the object.")]
        public static Output<List<ICurve>, List<ICurve>, List<ICurve>> ExplodeToParts(this IEnvironmentObject environmentObject, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, double numericTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            Output<List<ICurve>, List<ICurve>, List<ICurve>> finalParts = new Output<List<ICurve>, List<ICurve>, List<ICurve>>()
            {
                Item1 = new List<ICurve>(),
                Item2 = new List<ICurve>(),
                Item3 = new List<ICurve>(),
            };

            if (environmentObject == null) 
                return finalParts;

            finalParts.Item1.Add(environmentObject.Bottom(distanceTolerance, angleTolerance, numericTolerance));
            finalParts.Item2.AddRange(environmentObject.Sides(distanceTolerance, angleTolerance, numericTolerance));
            finalParts.Item3.Add(environmentObject.Top(distanceTolerance, angleTolerance, numericTolerance));
            return finalParts;
        }
    }
}




