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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the tangent Vector of a Bar as the direction Vector from StartNode to EndNode. No offsets or similar are taken into account.")]
        [Input("bar", "The Bar to get the tangent from.")]
        [Input("normalise", "If true, the tangent Vector returned will be a unit Vector, that is, a Vector with length 1.")]
        [Output("tan", "The tangent Vector of the Bar.")]
        public static Vector Tangent(this Bar bar, bool normalise = false)
        {
            Vector tan = bar.IsNull() ? null : bar.EndNode.Position - bar.StartNode.Position;
            return normalise ? tan?.Normalise() : tan;
        }

        /***************************************************/
    }
}




