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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Evaluates if the two Edges Supports and Releases are equal with the Support and Release comparers.")]
        [Input("element", "An Structure Edge to compare the properties of with an other Structure Edge.")]
        [Input("other", "The Structure Edge to compare with the other Structure Edge.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object.")]
        public static bool HasMergeablePropertiesWith(this Edge element, Edge other)
        {
            return element.IsNull() && other.IsNull() ? false :
                new Constraint4DOFComparer().Equals(element.Release, other.Release) &&
                   new Constraint6DOFComparer().Equals(element.Support, other.Support);
        }

        /***************************************************/

    }
}



