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
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Reinforcement;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes the centreline for a Reinforcement using the ShapeCode provided according to Bs 8666:2020.")]
        [Input("reinforcement", "The reinforcement containing the ShapeCode, reinforcement and bending radius to generate the centreline.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static ICurve Centreline(Reinforcement reinforcement)
        {
            if (reinforcement.IsNull())
                return null;

            return ICentreline(reinforcement.ShapeCode, reinforcement.Diameter, reinforcement.BendRadius);
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(IShapeCode shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode00 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode11 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode12 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode13 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode14 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode15 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode21 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode22 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode23 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode24 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode25 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode26 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode27 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode28 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode29 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode31 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode32 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode33 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode34 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode35 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode36 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode41 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode44 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode46 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode47 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode48 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode51 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode52 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode56 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode63 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode64 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode67 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode75 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode77 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode98 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode99 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/
        /****    Private Fallback Method            ********/
        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(IShapeCode shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/


    }
}

