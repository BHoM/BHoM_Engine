/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /******************************************************************/
        /*** Those methods were written to work with offset method only ***/
        /***        They were not tested with anything else!            ***/
        /******************************************************************/

        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        private static List<ICurve> TrimExtend(this ICurve curve, Point startPoint, Point endPoint, bool isExtend)
        {
            List<ICurve> result = new List<ICurve>();
            bool sP = false;
            bool eP = false;
            if(startPoint.IsOnCurve(curve))
            {
                curve = curve.ITrim(startPoint, curve.IEndPoint());
                sP = true;
            }
            if(endPoint.IsOnCurve(curve))
            {
                curve = curve.ITrim(curve.IStartPoint(), endPoint);
                eP = true;
            }
            if(sP && eP)
            {
                result.Add(curve);
                return result;
            }
            if(sP && !eP)
            {
                if (isExtend)
                    result.Add(curve.IExtend(curve.IStartPoint(), endPoint, isExtend));
                else
                    result = ((PolyCurve) curve.IExtend(curve.IStartPoint(), endPoint, isExtend)).Curves;
                return result;
            }
            if (!sP && !eP)
            {
                if (isExtend)
                    result.Add(curve.IExtend(startPoint, endPoint, isExtend));
                else
                    result = ((PolyCurve)curve.IExtend(startPoint, endPoint, isExtend)).Curves;
                return result;
            }
            if (!sP && eP)
            {
                if (isExtend)
                    result.Add(curve.IExtend(startPoint, curve.IEndPoint(), isExtend));
                else
                    result = ((PolyCurve)curve.IExtend(startPoint, curve.IEndPoint(), isExtend)).Curves;
                return result;
            }
            return result;
        }

        /***************************************************/
    }
}
