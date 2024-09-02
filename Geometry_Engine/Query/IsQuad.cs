/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;


namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Determines whether a Face is a quadilaterial.")]
        [Input("face", "The Face to check if it is quadilaterial.")]
        [Output("bool", "True for Faces that are quadilaterial or false for Faces that are non-quadilaterial.")]
        public static bool IsQuad(this Face face)
        {
            return face.D != -1;
        }

        /***************************************************/
        [Description("Determines whether a Polycurve is a quadilaterial.")]
        [Input("polycurve", "The Polycurve to check if it is quadilaterial.")]
        [Output("bool", "True for Polycurves that are quadilaterial or false for Polycurves that are non-quadilaterial.")]
        public static bool IsQuad(this PolyCurve polycurve)
        {
            if (polycurve == null)
                return false;

            if (polycurve.SubParts().Any(x => !x.IIsLinear()))
                return false;

            List<Point> points = polycurve.DiscontinuityPoints();
            if (points.Count != 4)
                return false;

            return points.IsCoplanar();
        }

        /***************************************************/
    }
}





