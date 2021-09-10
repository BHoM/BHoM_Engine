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

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to verify their compliance with BS 8666:2020.")]
        [Input("reinforcement", "The reinforcement containing the ShapeCode, reinforcement and bending radius to be verified.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(Reinforcement reinforcement)
        {
            if (reinforcement.IsNull())
                return false;

            return ICompliantShapeCode(reinforcement.ShapeCode, reinforcement.Diameter, reinforcement.BendRadius);


        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Input("bendingRadius", "The bending radius for the reinforcement bar, this is used as an override for the minimum.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool ICompliantShapeCode(IShapeCode shapeCode, double diameter, double bendingRadius)
        {
            if (shapeCode.IsNull())
                return false;

            return CompliantShapeCode(shapeCode as dynamic, diameter, bendingRadius);


        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Input("bendingRadius", "The bending radius for the reinforcement bar, this is used as an override for the minimum.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode00 shapeCode, double diameter, double bendingRadius)
        {
            if (shapeCode.IsNull())
                return false;

            if(shapeCode.A <= 0)
            {
                Reflection.Compute.RecordError("The A parameter must be greater than 0.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Input("bendingRadius", "The bending radius for the reinforcement bar, this is used as an override for the minimum.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode11 shapeCode, double diameter, double bendingRadius)
        {
            if (shapeCode.IsNull())
                return false;

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.B < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The A and B parameters must be greater than the minimum general end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Input("bendingRadius", "The bending radius for the reinforcement bar, this is used as an override for the minimum.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode12 shapeCode, double diameter, double bendingRadius)
        {
            if (shapeCode.IsNull())
                return false;

            if (shapeCode.A < shapeCode.R + diameter + Math.Max(5*diameter, 0.09) || shapeCode.B < shapeCode.R + diameter + Math.Max(5 * diameter, 0.09))
            {
                Reflection.Compute.RecordError("The A and B parameters must be greater than the minimum general end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/
        /****    Private Fallback Method            ********/
        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to verify their compliance with BS 8666:2020.")]
        [Input("shapeCode", "The reinforcement containing the ShapeCode, reinforcement and bending radius to be verified.")]
        [Output("diameter", "The anticipated hook dianeter based on the diameter of the reinforcement bar", typeof(Length))]
        private static bool CompliantShapeCode(IShapeCode shapeCode, double diameter, double bendingRadius)
        {
            if (shapeCode.IsNull())
                return false;

            Reflection.Compute.RecordError("The ShapeCode is not recognised.");
                return false;
        }

        /***************************************************/


    }
}

