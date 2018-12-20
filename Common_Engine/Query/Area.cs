/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Common;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static double Area(this IElement2D element2D)
        {
            //TODO: make this work for PolyCurves (Booleans needed)

            double result = element2D.IOutlineCurve().Area();

            List<Polyline> openings = new List<Polyline>();
            foreach (PolyCurve o in element2D.IInternalOutlineCurves())
            {
                Polyline p = o.ToPolyline();
                if (p == null)
                    throw new NotImplementedException();

                openings.Add(p);
            }

            foreach (Polyline p in openings.BooleanUnion())
            {
                result -= p.Area();
            }

            return result;
        }

        /******************************************/
    }
}
