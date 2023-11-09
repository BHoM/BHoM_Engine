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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Constructions;
using BH.oM.Physical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Reinforcement;


namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Reinforcement is null and outputs relevant error message.")]
        [Input("reinforcement", "The Reinforcement to test for null.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the Reinforcement or its defining properties are null.")]
        public static bool IsNull(this Reinforcement reinforcement, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (reinforcement == null)
            {
                ErrorMessage(methodName, "Reinforcement", msg);
                return true;
            }
            else if (reinforcement.ShapeCode == null)
            {
                ErrorMessage(methodName, "Reinforcement ShapeCode", msg);
                return true;
            }
            else if (reinforcement.CoordinateSystem.IsNull())
            {
                ErrorMessage(methodName, "Reinforcement CoordinateSystem", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if a ShapeCode is null and outputs relevant error message.")]
        [Input("shapeCode", "The ShapeCode to test for null.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the ShapeCode or its defining properties are null.")]
        public static bool IsNull(this IShapeCode shapeCode, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (shapeCode == null)
            {
                ErrorMessage(methodName, "ShapeCode", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if a IFramingElementProperty is null and outputs relevant error message.")]
        [Input("property", "The IFramingElementProperty to test for null.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the IFramingElementProperty or its defining properties are null.")]
        public static bool IsNull(this IFramingElementProperty property, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (property == null)
            {
                ErrorMessage(methodName, "IFramingElementProperty", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if a IConstruction is null and outputs relevant error message.")]
        [Input("construction", "The IConstruction to test for null.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the IConstruction or its defining properties are null.")]
        public static bool IsNull(this IConstruction construction, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (construction == null)
            {
                ErrorMessage(methodName, "IConstruction", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if a Pile is null and outputs relevant error message.")]
        [Input("pile", "The Pile to test for null.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the Pile or its defining properties are null.")]
        public static bool IsNull(this Pile pile, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (pile == null)
            {
                ErrorMessage(methodName, "Pile", msg);
                return true;
            }
            else if (pile.Location == null)
            {
                ErrorMessage(methodName, "Pile Location", msg);
                return true;
            }
            else if (pile.Property == null)
            {
                ErrorMessage(methodName, "Pile Property", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if a PadFoundation is null and outputs relevant error message.")]
        [Input("padFoundation", "The PadFoundation to test for null.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the PadFoundation or its defining properties are null.")]
        public static bool IsNull(this PadFoundation padFoundation, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (padFoundation == null)
            {
                ErrorMessage(methodName, "PadFoundation", msg);
                return true;
            }
            else if (padFoundation.Location == null)
            {
                ErrorMessage(methodName, "PadFoundation Location", msg);
                return true;
            }
            else if (padFoundation.Construction == null)
            {
                ErrorMessage(methodName, "PadFoundation Construction", msg);
                return true;
            }

            return false;
        }


        /***************************************************/

        [Description("Checks if a PileFoundation is null and outputs relevant error message.")]
        [Input("pileFoundation", "The PileFoundation to test for null.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the PileFoundation or its defining properties are null.")]
        public static bool IsNull(this PileFoundation pileFoundation, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (pileFoundation == null)
            {
                ErrorMessage(methodName, "PileFoundation", msg);
                return true;
            }
            else if (pileFoundation.PileCap == null)
            {
                ErrorMessage(methodName, "PileFoundation PileCap", msg);
                return true;
            }
            else if (pileFoundation.Piles.IsNullOrEmpty())
                return true;
            else if (pileFoundation.Piles.Any(x => x.IsNull()))
            {
                ErrorMessage(methodName, "PileFoundation Piles", "At least one Pile is null." + msg);
            }

            return false;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ErrorMessage(string methodName = "Method", string type = "type", string msg = "")
        {
            Base.Compute.RecordError($"Cannot evaluate {methodName} because the {type} is null. {msg}");
        }

    }
}