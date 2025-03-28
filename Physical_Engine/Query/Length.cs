/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Reinforcement;
using BH.oM.Physical.Reinforcement.BS8666;
using BH.oM.Quantities.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode00 using the formulation in BS 8666:2020 Table 5.")]
        [Input("reinforcement", "The Reinforcement to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        public static double Length(this Reinforcement reinforcement)
        {
            return reinforcement.IsNull() ? 0 : Length(reinforcement.ShapeCode as dynamic);
        }

        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private static double Length(this ShapeCode00 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A;
        }

        /***************************************************/

        private static double Length(this ShapeCode11 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B - 0.5 * shapeCode.BendRadius - shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode12 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B - 0.43 * shapeCode.R - 1.2 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode13 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + 0.57 * shapeCode.B + shapeCode.C - 1.6 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode14 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.C;
        }

        /***************************************************/

        private static double Length(this ShapeCode15 shapeCode)
        {
            if (Math.Acos(shapeCode.D / shapeCode.A) > Math.PI / 4)
                Engine.Base.Compute.RecordWarning("Bend angle is greater than 45 degrees, please use the Centreline method and measure the length of the curve instead.");
            
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.C;
        }

        /***************************************************/

        private static double Length(this ShapeCode21 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C - shapeCode.BendRadius - 2 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode22 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + 0.57 * shapeCode.C + shapeCode.D - 0.5 * shapeCode.BendRadius - 2.6 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode23 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C - shapeCode.BendRadius - 2 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode24 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C;
        }

        /***************************************************/

        private static double Length(this ShapeCode25 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.E;
        }

        /***************************************************/

        private static double Length(this ShapeCode26 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C;
        }

        /***************************************************/

        private static double Length(this ShapeCode27 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C - 0.5 * shapeCode.BendRadius - shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode28 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C - 0.5 * shapeCode.BendRadius - shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode29 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C;
        }

        /***************************************************/

        private static double Length(this ShapeCode31 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D - 1.5 * shapeCode.BendRadius - 3 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode32 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D - 1.5 * shapeCode.BendRadius - 3 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode33 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : 2 * shapeCode.A + 1.7 * shapeCode.B + 2 * shapeCode.C - 4 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode34 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.E - 0.5 * shapeCode.BendRadius - shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode35 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.E - 0.5 * shapeCode.BendRadius - shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode36 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D - shapeCode.BendRadius - 2 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode41 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D + shapeCode.E - 2 * shapeCode.BendRadius - 4 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode44 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D + shapeCode.E - 2 * shapeCode.BendRadius - 4 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode46 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + 2 * shapeCode.B + shapeCode.C + shapeCode.E;
        }

        /***************************************************/

        private static double Length(this ShapeCode47 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : 2 * shapeCode.A + shapeCode.B + 2 * shapeCode.C + 2 * shapeCode.HookDiameter() - 3 * shapeCode.BendRadius - 6 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode48 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : 2 * shapeCode.A + shapeCode.B + 2 * shapeCode.C - shapeCode.BendRadius - 2 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode51 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : 2 * (shapeCode.A + shapeCode.B + shapeCode.C) - 2.5 * shapeCode.BendRadius - 5 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode52 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : 2 * (shapeCode.A + shapeCode.B + shapeCode.C) - 1.5 * shapeCode.BendRadius - 3 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode56 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D + 2 * shapeCode.E - 1.5 * shapeCode.BendRadius - 3 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode63 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : 2 * shapeCode.A + 3 * shapeCode.B + 2 * shapeCode.C - 3 * shapeCode.BendRadius - 6 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode64 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + 2 * shapeCode.D + shapeCode.E + shapeCode.F - 3 * shapeCode.BendRadius - 6 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode67 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A;
        }

        /***************************************************/

        private static double Length(this ShapeCode75 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : Math.PI * (shapeCode.A - shapeCode.Diameter) + shapeCode.B + 0.025;
        }

        /***************************************************/

        private static double Length(this ShapeCode77 shapeCode)
        {
            shapeCode.IsNull();

            if (shapeCode.B > shapeCode.A / 5)
                return shapeCode.C * Math.Pow(Math.Pow(Math.PI * (shapeCode.A - shapeCode.Diameter), 2) + Math.Pow(shapeCode.B, 2), 0.5);
            else
                return shapeCode.IsNull() ? 0 : shapeCode.C * Math.PI * (shapeCode.A - shapeCode.Diameter);
        }

        /***************************************************/

        private static double Length(this ShapeCode98 shapeCode)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + 2 * shapeCode.B + shapeCode.C + shapeCode.D - 2 * shapeCode.BendRadius - 4 * shapeCode.Diameter;
        }

        /***************************************************/

        private static double Length(this ShapeCode99 shapeCode)
        {
            return shapeCode.Curve.ILength();
        }

        /***************************************************/
        /****    Private Fallback Method            ********/
        /***************************************************/

        private static double ILength(this IShapeCode shapeCode)
        {
            Base.Compute.RecordError("The ShapeCode is not recognised and the Length has not been calculated.");
            return 0;
        }

        /***************************************************/

    }
}



