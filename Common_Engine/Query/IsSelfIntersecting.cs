/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static bool IsSelfIntersecting(this IElement1D element1D, double tolerance = Tolerance.Distance)
        {
            return Geometry.Query.IIsSelfIntersecting(element1D.IGeometry(), tolerance);
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static bool IsSelfIntersecting(this IElement2D element2D, double tolerance = Tolerance.Distance)
        {
            if (Geometry.Query.IIsSelfIntersecting(element2D.IOutlineCurve(), tolerance))
                return true;

            foreach (PolyCurve internalOutline in element2D.IInternalOutlineCurves())
            {
                if (Geometry.Query.IIsSelfIntersecting(internalOutline, tolerance))
                    return true;
            }

            return false;
        }

        /******************************************/
    }
}

