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
using BH.oM.Physical.Reinforcement.BS8666;
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
        public static bool IsShapeCodeCompliant(this Reinforcement reinforcement)
        {
            return reinforcement.IsNull() ? false : IIsShapeCodeCompliant(reinforcement.ShapeCode, reinforcement.Diameter);

        }

        /***************************************************/

        [Description("Verifies the dimensions of the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Input("diameter", "The diameter of the reinforcement bar.", typeof(Length))]
        [Output("bool", "True if the shape code is compliant with BS 8666:2020.")]
        public static bool IIsShapeCodeCompliant(this IShapeCode shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }

            return IsShapeCodeCompliant(shapeCode as dynamic, diameter);


        }

        /***************************************************/
        /****    Private Methods                    ********/
        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode00 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The A parameter must be greater than 0 for ShapeCode00.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode11 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The A and B parameters of ShapeCode11 must be greater than the minimum general end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode12 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and B of ShapeCode12 must be greater than (R + d) + Max(5d, 90).");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode13 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameter B of ShapeCode13 must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B > 0.4 + 2 * diameter)
            {
                Reflection.Compute.RecordError("The parameter B of ShapeCode13 shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.A < shapeCode.B / 2 + Math.Max(5 * diameter, 0.09)
                || shapeCode.C < shapeCode.B / 2 + Math.Max(5 * diameter, 0.09))
            {
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode13 shall not be less than B/2 + the greater of 5d or 0.09.");
            }


            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode14 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode14 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode15 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.A, 2) - Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters A, B and D of ShapeCode15 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode15 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode21 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode21 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode22 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameter C of ShapeCode22 must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > 0.4 + 2 * diameter)
            {
                Reflection.Compute.RecordError("The parameter C of ShapeCode22 shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.D < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and D of ShapeCode22 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.D < shapeCode.C / 2 + Math.Max(5 * diameter, 0.09))
            {
                Reflection.Compute.RecordError("The parametrs D of ShapeCode22 shall not be less than C/2 + the greater of 5d or 0.09.");
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode23 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode23 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode24 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode24 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters B, D and E of ShapeCode24 do not form a right angled triangle within tolerance.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode25 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and B of ShapeCode25 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            if (Math.Asin(shapeCode.C / shapeCode.A) / Math.PI * 180 > 89 ||
                Math.Asin(shapeCode.D / shapeCode.B) / Math.PI * 180 > 89)
            {
                Reflection.Compute.RecordWarning("The bends are close to 90 degrees, consider scheduling a ShapeCode99 bar" +
                    "with horizontal offers as per BS 8666:2020 Table 2.");
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode26 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode26 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters B, D and E of ShapeCode26 do not form a right angled triangle within tolerance.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode27 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.A, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters A, D and E of ShapeCode27 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode27 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode28 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.A, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters A, D and E of ShapeCode28 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode28 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode29 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters B, D and Eof ShapeCode29 do not form a right angled triangle within tolerance.");
                return false;
            }
            if (shapeCode.A < diameter.GeneralEndProjection() || shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters A and C of ShapeCode29 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode31 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and D of ShapeCode31 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode32 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and D of ShapeCode32 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode33 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameter B of ShapeCode33 must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B > 0.4 + 2 * diameter)
            {
                Reflection.Compute.RecordError("The parameter B of ShapeCode33 shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.C < shapeCode.B + Math.Max(5 * diameter, 0.09))
            {
                Reflection.Compute.RecordError("The parameter C of ShapeCode33 must not be less B/2 plus the greater of 5d or 0.09.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode34 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and E of ShapeCode34 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.F, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters B, D and F of ShapeCode34 do not form a right angled triangle within tolerance.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode35 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and E of ShapeCode35 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.F, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters B, D and F of ShapeCode35 do not form a right angled triangle within tolerance.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode36 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and D of ShapeCode36 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.A, 2) - Math.Pow(shapeCode.E, 2) - Math.Pow(shapeCode.F, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters A, E and F of ShapeCode36 do not form a right angled triangle within tolerance.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode41 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and E of ShapeCode41 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode44 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and E of ShapeCode44 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode46 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and E of ShapeCode46 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.F, 2)) > Tolerance.MacroDistance)
            {
                Reflection.Compute.RecordError("The parameters A, E and F of ShapeCode36 do not form a right angled triangle within tolerance.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode47 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode47 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A)
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode47 must be greater than the parameter A.");
                return false;
            }
            else if (shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode47 must be greater than the minimum link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B < 2 * diameter.HookDiameter())
            {
                Reflection.Compute.RecordError("The parameter B of ShapeCode47 must be greater than two times the anticipated hook diameter (for segments C and D).");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode48 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode48 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A)
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode48 must be less than the parameter A defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < diameter.LinksEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode48 must be greater than the minimum link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode51 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode51 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A || shapeCode.D > shapeCode.B)
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode51 must be less than the parameters A and B respectively as defined in BS 8666:2020 Table 2.");
                return false;
            }
            if (shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode51 must be greater than the link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode52 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode52 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.B)
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode52 must be less than the parameter B as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < diameter.LinksEndProjection() || shapeCode.D < diameter.LinksEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode52 must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode56 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters E and F of ShapeCode56 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.E > shapeCode.A || shapeCode.F > shapeCode.B)
            {
                Reflection.Compute.RecordError("The parameters E and F of ShapeCode56 must be less than the parameter B as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.E < diameter.GeneralEndProjection() || shapeCode.F < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters E and F of ShapeCode56 must be greater than the general end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode63 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters E and F of ShapeCode63 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A || shapeCode.D > shapeCode.A)
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode63 must be less than the parameter A as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < diameter.GeneralEndProjection())
            {
                Reflection.Compute.RecordError("The parameters C and D of ShapeCode63 must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode64 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters A and F of ShapeCode64 must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode67 shapeCode, double diameter)
        {
            if (shapeCode.IsNull())
                return false;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }
            else if(Math.Abs(shapeCode.A - 2*shapeCode.R*Math.Asin(shapeCode.B/(2*shapeCode.R))) > Tolerance.Distance)
            {
                Reflection.Compute.RecordError("The parameter A of ShapeCode67 must be equal to the arc length formed by the segment constructed from the width B and centre R.");
                return false;
            }
            else if(shapeCode.R > diameter.MaximumRadius())
            {
                Reflection.Compute.RecordError("The parameter R of ShapeCode67 must be less than the maximum preformed radius defined in BS 8666:2020 Table 8.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode75 shapeCode, double diameter)
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

        private static bool IsShapeCodeCompliant(this ShapeCode77 shapeCode, double diameter)
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

        private static bool IsShapeCodeCompliant(this ShapeCode98 shapeCode, double diameter)
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
                Reflection.Compute.RecordError("The parameters C and D  of ShapeCode98 must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsShapeCodeCompliant(this ShapeCode99 shapeCode, double diameter)
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

        private static bool IsShapeCodeCompliant(IShapeCode shapeCode, double diameter, double bendingRadius)
        {
            if (shapeCode.IsNull())
                return false;

            Reflection.Compute.RecordError("The ShapeCode is not recognised.");
            return false;
        }

        /***************************************************/


    }
}

