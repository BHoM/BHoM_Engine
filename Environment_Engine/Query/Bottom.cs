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
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the bottom of a given environment object.")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have a geometrical bottom.")]
        [Input("distanceTolerance", "Distance tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Distance.")]
        [Input("angleTolerance", "Angle tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Angle.")]
        [Input("numericTolerance", "Tolerance for determining whether a calulated number is within a range defined by the tolerance, default is set to the value defined by BH.oM.Geometry.Tolerance.Distance.")]
        [Output("curve", "An ICurve representation of the bottom of the object.")]
        public static ICurve Bottom(this IEnvironmentObject environmentObject, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, double numericTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (environmentObject == null) 
                return null;

            double tilt = environmentObject.Tilt(distanceTolerance, angleTolerance);
            if ((tilt >= 0 - numericTolerance && tilt <= 0 + numericTolerance) || (tilt >= 180 - numericTolerance && tilt <= 180 + numericTolerance))
            {
                BH.Engine.Base.Compute.RecordWarning("Cannot find the bottom of a horizontal IEnvironmentObject");
                return null;
            }

            Polyline workingCurves = environmentObject.Polyline();

            if (workingCurves == null)
                return null;

            double aZ = double.MaxValue;
            ICurve aResult = null;
            foreach (ICurve aCurve in workingCurves.SplitAtPoints(workingCurves.DiscontinuityPoints()))
            {
                Point aPoint_Start = aCurve.IStartPoint();
                Point aPoint_End = aCurve.IEndPoint();

                if (aPoint_End.Z <= aZ && aPoint_Start.Z <= aZ)
                {
                    aZ = Math.Max(aPoint_End.Z, aPoint_Start.Z);
                    aResult = aCurve;
                }
            }

            return aResult;
        }
    }
}






