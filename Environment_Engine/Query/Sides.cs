/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the sides of a given environment object.")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have geometrical sides.")]
        [Output("curves", "ICurve representations of the sides of the object.")]
        public static List<ICurve> Sides(this IEnvironmentObject environmentObject)
        {
            if (environmentObject == null) 
                return null;

            if (environmentObject.Tilt() == 0 || environmentObject.Tilt() == 180)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Cannot find the sides of a horizontal IEnvironmentObject");
                return null;
            }

            Polyline workingCurves = environmentObject.Polyline();

            if (workingCurves == null)
                return null;

            double aZMax = workingCurves.ControlPoints().Select(z => z.Z).Max();
            double aZMin = workingCurves.ControlPoints().Select(z => z.Z).Max();
            List<ICurve> aResult = new List<ICurve>();

            foreach (ICurve aCurve in workingCurves.SplitAtPoints(workingCurves.DiscontinuityPoints()))
            {
                if (aCurve.IControlPoints().Where(x => x.Z == aZMax).Count() ==1 && aCurve.IControlPoints().Where(x => x.Z == aZMin).Count() == 1)
                {
                    aResult.Add(aCurve);
                }
            }

            return aResult;
        }
    }
}
