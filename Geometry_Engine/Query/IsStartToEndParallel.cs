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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks whether two curves are parallel based on their start-to-end directions.")]
        [Input("curve1", "The first curve whose start-to-end direction is used for comparison.")]
        [Input("curve2", "The second curve whose start-to-end direction is used for comparison.")]
        [Input("angleTolerance", "The maximum angle (in radians) between the two directions for them to be considered parallel.")]
        [Output("isParallel", "True if the start-to-end directions of the two curves are parallel within the given tolerance, false otherwise.")]
        public static bool IsStartToEndParallel(this ICurve curve1, ICurve curve2, double angleTolerance)
        {
            Vector hostVect = curve1.StartToEndDirection();
            Vector elemVect = curve2.StartToEndDirection();

            if (hostVect.IsParallel(elemVect) != 0)
                return true;

            double angle = hostVect.AcuteAngle(elemVect);
            return angle < angleTolerance;
        }

        /***************************************************/
    }
}
