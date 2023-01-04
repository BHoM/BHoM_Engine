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

using BH.oM.Data.Collections;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        internal static double PMSquareDistance(this Point point1, Point point2)
        {
            double dx = point1.X - point2.X;
            double dy = point1.Y - point2.Y;
            double dz = point1.Z - point2.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        [Description("Queries the square distance between two DomainBoxes.")]
        [Input("box1", "Box to evaluate square distance from.")]
        [Input("box2", "Box to evaluate square distance from.")]
        [Output("sqDist", "Square distance between the two DomainBoxes.")]
        public static double SquareDistance(this DomainBox box1, DomainBox box2)
        {
            if(box1 == null || box2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the square distance if either domain box is null.");
                return 0;
            }

            return box1.Domains.Zip(box2.Domains, (a, b) => Math.Pow(Distance(a, b), 2)).Sum();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double Distance(Domain a, Domain b)
        {
            if (a.Max < b.Min)
                return a.Max - b.Min;
            else if (a.Min > b.Max)
                return a.Min - b.Max;
            else
                return 0;
        }

        /***************************************************/
    }
}




