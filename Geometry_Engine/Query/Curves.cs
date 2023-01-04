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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the inner curves of the PolyCurve.")]
        [Input("curve", "The PolyCurve to extract the inner curves from.")]
        [Output("curves", "The inner curves fo the PolyCurve.")]
        public static List<ICurve> Curves(this PolyCurve curve)
        {
            return curve?.Curves;
        }

        /***************************************************/

        [Description("Returns the inner curves of the BoundaryCurve.")]
        [Input("curve", "The BoundaryCurve to extract the inner curves from.")]
        [Output("curves", "The inner curves fo the BoundaryCurve.")]
        public static List<ICurve> Curves(this BoundaryCurve curve)
        {
            return curve?.Curves?.ToList();
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Returns the inner curves of the IPolyCurve.")]
        [Input("curve", "The IPolyCurve to extract the inner curves from.")]
        [Output("curves", "The inner curves fo the IPolyCurve.")]
        public static List<ICurve> ICurves(this IPolyCurve curve)
        {
            if (curve == null)
                return null;

            return Curves(curve as dynamic);
        }

        /***************************************************/
    }
}

