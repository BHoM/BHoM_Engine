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

using BH.oM.Architecture.BuildersWork;
using BH.oM.Architecture.Elements;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Architecture
{
    public static partial class Query
    {
        /******************************************/
        /****        Public Methods            ****/
        /******************************************/

        [Description("Evaluates if the two Rooms are equal to the point that they could be merged into one object, as Rooms only have geometrical data this is always true.")]
        [Input("element", "A Room to compare the properties of with an other Room.")]
        [Input("other", "The Room to compare with the other Room.")]
        [Output("equal", "True if the Rooms non-geometrical property is equal to the point that they could be merged into one object, always true for rooms.")]
        public static bool HasMergeablePropertiesWith(this Room element, Room other)
        {
            return true;
        }

        /******************************************/

        [Description("Evaluates if the two Openings are equal to the point that they could be merged into one object, as Openings only have geometrical data this is always true.")]
        [Input("element", "A Opening to compare the properties of with an other Opening.")]
        [Input("other", "The Opening to compare with the other Opening.")]
        [Output("equal", "True if the Openings non-geometrical property is equal to the point that they could be merged into one object, always true for openings.")]
        public static bool HasMergeablePropertiesWith(this Opening element, Opening other)
        {
            return true;
        }

        /******************************************/

    }
}




