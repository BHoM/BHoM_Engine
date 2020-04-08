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
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Queries the IElement2Ds area defined as the area confined by the outline curves subtracting the area of the internal elements.")]
        [Input("element2D", "The IElement2D to query the area of.")]
        [Output("area", "The area of the region confined by the IElement2Ds outline elements subtracting the area of the internal elements", typeof(Area))]
        public static double Area(this IElement2D element2D)
        {
            double result = element2D.OutlineCurve().IArea();

            List<PolyCurve> openings = element2D.InternalOutlineCurves().BooleanUnion();

            foreach (PolyCurve o in openings)
                result -= o.Area();

            return result;
        }

        /******************************************/
    }
}

