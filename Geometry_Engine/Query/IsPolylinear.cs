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

using BH.oM.Geometry;
using System.Collections.Generic;
using System;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsPolylinear(this PolyCurve curve)
        {
            foreach (ICurve c in curve.SubParts())
            {
                if (!(c is Line))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsPolylinear(this Arc curve)
        {
            return false;
        }

        /***************************************************/

        public static bool IsPolylinear(this Circle curve)
        {
            return false;
        }

        /***************************************************/

        public static bool IsPolylinear(this Line curve)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPolylinear(this Polyline curve)
        {
            return true;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        public static bool IIsPolylinear(this ICurve curve)
        {
            foreach (ICurve c in curve.ISubParts())
            {
                if (!(c is Line))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsPolylinear(this ICurve curve)
        {
            throw new NotImplementedException($"IsPolylinear is not implemented for ICurves of type: {curve.GetType().Name}.");
        }

        /***************************************************/
    }
}


