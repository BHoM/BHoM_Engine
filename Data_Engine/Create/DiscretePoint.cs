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
using System;

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static DiscretePoint DiscretePoint(int x, int y, int z)
        {
            return new DiscretePoint { X = x, Y = y, Z = z };
        }

        /***************************************************/

        public static DiscretePoint DiscretePoint(Point point, double step = 1.0)
        {
            if(point == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a discrete point from a null geometry point.");
                return null;
            }

            return new DiscretePoint
            {
                X = (int)Math.Floor(point.X / step),
                Y = (int)Math.Floor(point.Y / step),
                Z = (int)Math.Floor(point.Z / step)
            };
        }

        /***************************************************/
    }
}




