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

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Returns a short string representation (label) of an object.")]
        [Input("obj", "Object to get a label for.")]
        [Output("label", "Label of the input object.")]
        public static string ILabel(this object obj)
        {
            if (obj == null)
                return "null";

            object label;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(Label), new object[] { }, out label))
                label = obj.Label();

            return (string)label;
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static string Label(this object obj)
        {
            return obj?.GetType().Name ?? "null";
        }

        /***************************************************/
    }
}
