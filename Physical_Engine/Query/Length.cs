/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Reinforcement;
using BH.oM.Quantities.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode00 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode00 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode11 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode11 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B - 0.5 * bendingRadius - diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode12 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode12 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B - 0.43 * shapeCode.R - 1.2 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode13 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode13 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + 0.57 * shapeCode.B + shapeCode.C - 1.6 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode14 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode14 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.C;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode15 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode15 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.C;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode21 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode21 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C - bendingRadius - 2 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode22 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode22 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + 0.57 * shapeCode.C + shapeCode.D - 0.5 * bendingRadius - 2.6 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode23 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode23 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C - bendingRadius - 2 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode24 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode24 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode25 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode25 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.E;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode26 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode26 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode27 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode27 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C - 0.5 * bendingRadius - diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode28 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode28 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C - 0.5 * bendingRadius - diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode29 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode29 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode31 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode31 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D - 1.5 * bendingRadius - 3 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode32 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode32 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D - 1.5 * bendingRadius - 3 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode33 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode33 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : 2 * shapeCode.A + 1.7 * shapeCode.B + 2 * shapeCode.C - 4 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode34 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode34 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.E - 0.5 * bendingRadius - diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode35 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode35 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.E - 0.5 * bendingRadius - diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode36 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode36 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D - bendingRadius - 2 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode41 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode41 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D + shapeCode.E - 2 * bendingRadius - 4 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode44 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode44 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D + shapeCode.E - 2 * bendingRadius - 4 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode46 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode46 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + 2 * shapeCode.B + shapeCode.C + shapeCode.E;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode47 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode47 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : 2 * shapeCode.A + shapeCode.B + 2 * shapeCode.C + 2 * diameter.HookDiameter(bendingRadius) - 3 * bendingRadius - 6 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode48 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode48 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : 2 * shapeCode.A + shapeCode.B + 2 * shapeCode.C - bendingRadius - 2 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode51 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode51 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : 2 * (shapeCode.A + shapeCode.B + shapeCode.C) - 2.5 * bendingRadius - 5 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode52 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode52 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : 2 * (shapeCode.A + shapeCode.B + shapeCode.C) - 1.5 * bendingRadius - 3 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode56 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode56 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + shapeCode.D + 2 * shapeCode.E - 1.5 * bendingRadius - 3 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode63 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode63 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : 2 * shapeCode.A + 3 * shapeCode.B + 2 * shapeCode.C - 3 * bendingRadius - 6 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode64 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode64 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + shapeCode.B + shapeCode.C + 2 * shapeCode.D + shapeCode.E + shapeCode.F - 3 * bendingRadius - 6 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode67 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode67 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode75 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode75 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : Math.PI * (shapeCode.A - diameter) + shapeCode.B + 25;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode77 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode77 shapeCode, double diameter, double bendingRadius)
        {
            shapeCode.IsNull();

            if (shapeCode.B > shapeCode.A / 5)
                return shapeCode.C * Math.Pow(Math.Pow(Math.PI * (shapeCode.A - diameter), 2) + Math.Pow(shapeCode.B, 2), 0.5);
            else
                return shapeCode.IsNull() ? 0 : shapeCode.C * Math.PI * (shapeCode.A - diameter);
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode98 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode98 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.IsNull() ? 0 : shapeCode.A + 2 * shapeCode.B + shapeCode.C + shapeCode.D - 2 * bendingRadius - 4 * diameter;
        }

        /***************************************************/

        [Description("Gets the total length of the Reinforcement with ShapeCode99 using the formulation in BS 8666:2020 Table 5.")]
        [Input("shapeCode", "The ShapeCode to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement determined by the ShapeCode parameters.", typeof(Length))]
        private static double Length(this ShapeCode99 shapeCode, double diameter, double bendingRadius)
        {
            return shapeCode.CentreCurve.ILength();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the total length of the Reinforcement.")]
        [Input("reinforcement", "The Reinforcement to calculate the total length for.")]
        [Output("length", "The total length of the Reinforcement.", typeof(Length))]
        public static double Length(this Reinforcement reinforcement)
        {
            return reinforcement.IsNull() ? 0 : Length(reinforcement.ShapeCode as dynamic, reinforcement.Diameter, reinforcement.BendRadius);
        }

        /***************************************************/
        /****    Private Fallback Method            ********/
        /***************************************************/

        private static double ILength(this IShapeCode shapeCode)
        {
            Reflection.Compute.RecordError("The ShapeCode is not recognised and the Length has not been calculated.");
            return 0;
        }

        /***************************************************/

    }
}

