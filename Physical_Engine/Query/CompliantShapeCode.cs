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

        [Description("Verifies the dimensions of the ShapeCode to verify their compliance with BS 8666:2020.")]
        [Input("reinforcement", "The reinforcement containing the ShapeCode, reinforcement and bending radius to be verified.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(Reinforcement reinforcement)
        {
            if (reinforcement.IsNull())
                return false;

            return ICompliantShapeCode(reinforcement.ShapeCode, reinforcement.Diameter);

        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool ICompliantShapeCode(IShapeCode shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            return CompliantShapeCode(shapeCode as dynamic, diameter);


        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode00 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A <= 0)
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
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode11 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

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
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode12 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < shapeCode.R + diameter + Math.Max(5 * diameter, 0.09) || shapeCode.B < shapeCode.R + diameter + Math.Max(5 * diameter, 0.09))
            {
                Reflection.Compute.RecordError("The parameters A and B must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode13 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.B < HookDiameter(diameter))
            {
                Reflection.Compute.RecordError("The parameter B must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B > 0.4 + 2 * diameter)
            {
                Reflection.Compute.RecordError("The parameter B shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.A < shapeCode.B / 2 + Math.Max(5 * diameter, 0.09)
                || shapeCode.C < shapeCode.B / 2 + Math.Max(5 * diameter, 0.09))
            {
                Reflection.Compute.RecordError("The parametrs A and C shall not be less than B/2 + the greater of 5d or 0.09.");
            }


            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode14 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode15 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode21 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode22 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.C < HookDiameter(diameter))
            {
                Reflection.Compute.RecordError("The parameter C must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > 0.4 + 2 * diameter)
            {
                Reflection.Compute.RecordError("The parameter C shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.D < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and D must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.D < shapeCode.C / 2 + Math.Max(5 * diameter, 0.09))
            {
                Reflection.Compute.RecordError("The parametrs D shall not be less than C/2 + the greater of 5d or 0.09.");
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode23 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode24 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters B, D and E do not form a right angled triangle within tolerance.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode25 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.B < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and B must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            if (Math.Asin(shapeCode.C / shapeCode.A) / Math.PI * 180 > 89 ||
                Math.Asin(shapeCode.D / shapeCode.B) / Math.PI * 180 > 89)
            {
                Reflection.Compute.RecordError("The bends are close to 90 degrees, schedule a ShapeCode99 bar" +
                    "with horizontal offers as per BS 8666:2020 Table 2.");
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode26 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode27 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode28 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode29 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode31 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.D < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and D must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode32 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.D < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and D must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode33 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.B < HookDiameter(diameter))
            {
                Reflection.Compute.RecordError("The parameter B must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B > 0.4 + 2 * diameter)
            {
                Reflection.Compute.RecordError("The parameter B shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.C < shapeCode.B + Math.Max(5 * diameter, 0.09))
            {
                Reflection.Compute.RecordError("The parameter C must not be less B/2 plus the greater of 5d or 0.09.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode34 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.E < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and E must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode35 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.E < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and E must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode36 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.D < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and D must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode41 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.E < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and E must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode44 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.E < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and E must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode46 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.E < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and E must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode47 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.C != shapeCode.D)
            {
                Reflection.Compute.RecordError("The parameters C and D must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A)
            {
                Reflection.Compute.RecordError("The parameters C and D must be greater than the parameter A.");
                return false;
            }
            else if (shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D must be greater than the minimum link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B < 2 * diameter.HookDiameter())
            {
                Reflection.Compute.RecordError("The parameter B must be greater than two times the anticipated hook diameter (for segments C and D).");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode48 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.C != shapeCode.D)
            {
                Reflection.Compute.RecordError("The parameters C and D must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A)
            {
                Reflection.Compute.RecordError("The parameters C and D must be less than the parameter A defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < diameter.LinksEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D must be greater than the minimum link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode51 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.C != shapeCode.D)
            {
                Reflection.Compute.RecordError("The parameters C and D must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A || shapeCode.D > shapeCode.B)
            {
                Reflection.Compute.RecordError("The parameters C and D must be less than the parameters A and B respectively as defined in BS 8666:2020 Table 2.");
                return false;
            }
            if (shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D must be greater than the link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode52 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.C != shapeCode.D)
            {
                Reflection.Compute.RecordError("The parameters C and D must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.B)
            {
                Reflection.Compute.RecordError("The parameters C and D must be less than the parameter B as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < diameter.LinksEndProjection() || shapeCode.D < diameter.LinksEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode56 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.E != shapeCode.F)
            {
                Reflection.Compute.RecordError("The parameters E and F must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.E > shapeCode.A || shapeCode.F > shapeCode.B)
            {
                Reflection.Compute.RecordError("The parameters E and F must be less than the parameter B as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < diameter.GeneralEndProjection() || shapeCode.D < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode63 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.C != shapeCode.D)
            {
                Reflection.Compute.RecordError("The parameters E and F must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A || shapeCode.D > shapeCode.A)
            {
                Reflection.Compute.RecordError("The parameters C and D must be less than the parameter A as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode64 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.F < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and F must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode67 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            //Table 10

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode75 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode77 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode98 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            if (shapeCode.C < diameter.GeneralEndProjection() || shapeCode.D < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.")]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool CompliantShapeCode(ShapeCode99 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            return true;
        }

        /***************************************************/
        /****    Private Fallback Method            ********/
        /***************************************************/

        [Description("Verifies  the dimensions of the ShapeCode to verify their compliance with BS 8666:2020.")]
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

