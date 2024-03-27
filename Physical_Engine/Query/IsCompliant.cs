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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
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

        [Description("Verifies the dimensions to the standard as determined by the ShapeCode namespace.")]
        [Input("reinforcement", "The reinforcement containing the ShapeCode, reinforcement and bending radius to be verified.")]
        [Output("bool", "True if the shape code is compliant with the relevant standard as determined by the ShapeCode namespace.")]
        public static bool IsCompliant(this Reinforcement reinforcement)
        {
            return reinforcement.IsNull() ? false : IIsCompliant(reinforcement.ShapeCode);
        }

        /***************************************************/

        [Description("Verifies the dimensions to the standard as determined by the ShapeCode namespace.")]
        [Input("shapeCode", "The ShapeCode to be verified.")]
        [Output("bool", "True if the shape code is compliant with the relevant standard as determined by the ShapeCode namespace.")]
        public static bool IIsCompliant(this IShapeCode shapeCode)
        {
            if (shapeCode.IsNull())
                return false;
            else if (shapeCode.Diameter <= Tolerance.Distance)
            {
                Base.Compute.RecordError("The diameter must be greater than zero.");
                return false;
            }
            else if (shapeCode.BendRadius < shapeCode.SchedulingRadius())
            {
                Base.Compute.RecordError("The bend radius must be greater than the minimum scheduling radius.");
                return false;
            }

            return IsCompliant(shapeCode as dynamic);
        }

        /***************************************************/
        /****    Private Methods                    ********/
        /***************************************************/

        private static bool IsCompliant(this ShapeCode00 shapeCode)
        {
            if (shapeCode.A <= 0)
            {
                Base.Compute.RecordError("The A parameter must be greater than 0 for ShapeCode00.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode11 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.B < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A and B parameters of ShapeCode11 must be greater than the minimum general end projection.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A and B parameters of ShapeCode11 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }    

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode12 shapeCode)
        {
            if (shapeCode.A < shapeCode.R + shapeCode.Diameter + Math.Max(5 * shapeCode.Diameter, 0.09) || shapeCode.B < shapeCode.R + shapeCode.Diameter + Math.Max(5 * shapeCode.Diameter, 0.09))
            {
                Base.Compute.RecordError("The parameters A and B of ShapeCode12 must be greater than (R + d) + Max(5d, 90).");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode13 shapeCode)
        {
            if (shapeCode.B < shapeCode.HookDiameter())
            {
                Base.Compute.RecordError("The parameter B of ShapeCode13 must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B > 0.4 + 2 * shapeCode.Diameter)
            {
                Base.Compute.RecordError("The parameter B of ShapeCode13 shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.A < shapeCode.B / 2 + Math.Max(5 * shapeCode.Diameter, 0.09)
                || shapeCode.C < shapeCode.B / 2 + Math.Max(5 * shapeCode.Diameter, 0.09))
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode13 shall not be less than B/2 + the greater of 5d or 0.09.");
            }


            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode14 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode14 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A and C parameters of ShapeCode14 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode15 shapeCode)
        {
            //if (Math.Abs(Math.Pow(shapeCode.A, 2) - Math.Pow(shapeCode.B - shapeCode.Diameter, 2) - Math.Pow(shapeCode.D, 2)) > Tolerance.MacroDistance)
            //{
            //    Base.Compute.RecordError("The parameters A, B and D of ShapeCode15 do not form a right angled triangle within tolerance.");
            //    return false;
            //}
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode15 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A and C parameters of ShapeCode15 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode21 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode21 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B and C parameters of ShapeCode21 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode22 shapeCode)
        {
            if (shapeCode.C < shapeCode.HookDiameter())
            {
                Base.Compute.RecordError("The parameter C of ShapeCode22 must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > 0.4 + 2 * shapeCode.Diameter)
            {
                Base.Compute.RecordError("The parameter C of ShapeCode22 shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.D < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and D of ShapeCode22 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.D < shapeCode.C / 2 + Math.Max(5 * shapeCode.Diameter, 0.09))
            {
                Base.Compute.RecordError("The parametrs D of ShapeCode22 shall not be less than C/2 + the greater of 5d or 0.09.");
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A and B parameters of ShapeCode22 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode23 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode23 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B and C parameters of ShapeCode23 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode24 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode24 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Base.Compute.RecordError("The parameters B, D and E of ShapeCode24 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B and C parameters of ShapeCode24 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode25 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.B < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and B of ShapeCode25 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            if (Math.Asin((shapeCode.C - shapeCode.Diameter) / shapeCode.A) / Math.PI * 180 > 89 ||
                Math.Asin((shapeCode.D - shapeCode.Diameter) / shapeCode.B) / Math.PI * 180 > 89)
            {
                Base.Compute.RecordWarning("The bends are close to 90 degrees, consider scheduling a ShapeCode99 bar" +
                    "with horizontal offers as per BS 8666:2020 Table 2.");
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.E - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B and E parameters of ShapeCode25 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode26 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode26 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
    || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B and C parameters of ShapeCode26 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode27 shapeCode)
        {
            if (Math.Abs(Math.Pow(shapeCode.A, 2) - Math.Pow(shapeCode.D - shapeCode.Diameter, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Base.Compute.RecordError("The parameters A, D and E of ShapeCode27 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode27 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B and C parameters of ShapeCode27 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode28 shapeCode)
        {
            if (Math.Abs(Math.Pow(shapeCode.A, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.E, 2)) > Tolerance.MacroDistance)
            {
                Base.Compute.RecordError("The parameters A, D and E of ShapeCode28 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode28 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B and C parameters of ShapeCode28 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode29 shapeCode)
        {
            double angle = Math.Atan(shapeCode.E / (shapeCode.D - shapeCode.Diameter - 2 * shapeCode.BendRadius));
            if (Math.Pow(shapeCode.B - shapeCode.BendRadius - shapeCode.Diameter,2) - Math.Pow(shapeCode.E,2) - Math.Pow(shapeCode.D - 2*shapeCode.BendRadius - shapeCode.Diameter,2)  > Tolerance.MacroDistance)
            {
                Base.Compute.RecordError("The parameters B, D and Eof ShapeCode29 do not form a right angled triangle within tolerance.");
                return false;
            }
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and C of ShapeCode29 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B and C parameters of ShapeCode29 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode31 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.D < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and D of ShapeCode31 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode31 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode32 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.D < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and D of ShapeCode32 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode33 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode33 shapeCode)
        {
            if (shapeCode.B < shapeCode.HookDiameter())
            {
                Base.Compute.RecordError("The parameter B of ShapeCode33 must be at least the hook diameter in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B > 0.4 + 2 * shapeCode.Diameter)
            {
                Base.Compute.RecordError("The parameter B of ShapeCode33 shall not exceed 0.4 + 2d.");
                return false;
            }
            else if (shapeCode.C < shapeCode.B + Math.Max(5 * shapeCode.Diameter, 0.09))
            {
                Base.Compute.RecordError("The parameter C of ShapeCode33 must not be less B/2 plus the greater of 5d or 0.09.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode34 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.E < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and E of ShapeCode34 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.F, 2)) > Tolerance.MacroDistance)
            {
                Base.Compute.RecordError("The parameters B, D and F of ShapeCode34 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.E - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and E parameters of ShapeCode34 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode35 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.E < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and E of ShapeCode35 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.B, 2) - Math.Pow(shapeCode.D, 2) - Math.Pow(shapeCode.F, 2)) > Tolerance.MacroDistance)
            {
                Base.Compute.RecordError("The parameters B, D and F of ShapeCode35 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.E - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and E parameters of ShapeCode35 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode36 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.D < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and D of ShapeCode36 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (Math.Abs(Math.Pow(shapeCode.A, 2) - Math.Pow(shapeCode.E, 2) - Math.Pow(shapeCode.F, 2)) > Tolerance.MacroDistance)
            {
                Base.Compute.RecordError("The parameters A, E and F of ShapeCode36 do not form a right angled triangle within tolerance.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode36 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode41 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.E < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and E of ShapeCode41 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.E - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C, D and E parameters of ShapeCode41 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode44 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.E < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and E of ShapeCode44 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.E - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C, D and E parameters of ShapeCode44 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode46 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.E < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and E of ShapeCode46 must be greater than the minimum general end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.E - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and E parameters of ShapeCode46 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode47 shapeCode)
        {
            if (shapeCode.C != shapeCode.D)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode47 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode47 must be greater than the parameter A.");
                return false;
            }
            else if (shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode47 must be greater than the minimum link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.B < 2 * shapeCode.HookDiameter())
            {
                Base.Compute.RecordError("The parameter B of ShapeCode47 must be greater than two times the anticipated hook diameter (for segments C and D).");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode47 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode48 shapeCode)
        {
            if (shapeCode.C != shapeCode.D)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode48 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode48 must be less than the parameter A defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < shapeCode.LinksEndProjection())
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode48 must be greater than the minimum link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode48 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode51 shapeCode)
        {
            if (shapeCode.C != shapeCode.D)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode51 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A || shapeCode.D > shapeCode.B)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode51 must be less than the parameters A and B respectively as defined in BS 8666:2020 Table 2.");
                return false;
            }
            if (shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode51 must be greater than the link end projection defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode51 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode52 shapeCode)
        {
            if (shapeCode.C != shapeCode.D)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode52 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.B)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode52 must be less than the parameter B as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < shapeCode.LinksEndProjection() || shapeCode.D < shapeCode.LinksEndProjection())
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode52 must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode52 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode56 shapeCode)
        {
            if (shapeCode.E != shapeCode.F)
            {
                Base.Compute.RecordError("The parameters E and F of ShapeCode56 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.E > shapeCode.A || shapeCode.F > shapeCode.B)
            {
                Base.Compute.RecordError("The parameters E and F of ShapeCode56 must be less than the parameter B as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.E < shapeCode.GeneralEndProjection() || shapeCode.F < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters E and F of ShapeCode56 must be greater than the general end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.E - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.F - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C, D, E and F parameters of ShapeCode56 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode63 shapeCode)
        {
            if (shapeCode.C != shapeCode.D)
            {
                Base.Compute.RecordError("The parameters E and F of ShapeCode63 must be equal as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C > shapeCode.A || shapeCode.D > shapeCode.A)
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode63 must be less than the parameter A as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.C < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters C and D of ShapeCode63 must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode63 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode64 shapeCode)
        {
            if (shapeCode.A < shapeCode.GeneralEndProjection() || shapeCode.F < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters A and F of ShapeCode64 must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.E - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.F - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C, D, E and F parameters of ShapeCode64 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode67 shapeCode)
        {
            if(shapeCode.C > shapeCode.R)
            {
                Base.Compute.RecordError("The parameter C must be less than or equal to the parameter R.");
                return false;
            }
            else if(Math.Abs(shapeCode.A - shapeCode.R*2*Math.Acos((shapeCode.R-shapeCode.C)/shapeCode.R)) > Tolerance.Distance)
            {
                Base.Compute.RecordError("The parameter A of ShapeCode67 must be equal to the arc length formed by the segment constructed from the width B and centre R.");
                return false;
            }
            else if(shapeCode.R > shapeCode.MaximumRadius())
            {
                Base.Compute.RecordError("The parameter R of ShapeCode67 must be less than the maximum preformed radius defined in BS 8666:2020 Table 8.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode75 shapeCode)
        {
            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode77 shapeCode)
        {
            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode98 shapeCode)
        {
            if (shapeCode.C < shapeCode.GeneralEndProjection() || shapeCode.D < shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The parameters C and D  of ShapeCode98 must be greater than the link end projection as defined in BS 8666:2020 Table 2.");
                return false;
            }
            else if (shapeCode.BendRadius > shapeCode.A - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.B - shapeCode.GeneralEndProjection()
                || shapeCode.BendRadius > shapeCode.C - shapeCode.GeneralEndProjection() || shapeCode.BendRadius > shapeCode.D - shapeCode.GeneralEndProjection())
            {
                Base.Compute.RecordError("The A, B, C and D parameters of ShapeCode98 must be greater than the bending radius plus the minimum end projection.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool IsCompliant(this ShapeCode99 shapeCode)
        {
            return true;
        }

        /***************************************************/
        /****    Private Fallback Method            ********/
        /***************************************************/

        private static bool IsCompliant(IShapeCode shapeCode)
        {
            Base.Compute.RecordError("The ShapeCode is not recognised.");
            return false;
        }

        /***************************************************/


    }
}


