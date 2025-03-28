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

using BH.oM.Structure.Constraints;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a LinkConstraint from a list of booleans. True denotes fixity.")]
        [Input("name", "Name of the created LinkConstraint. This is required for various structural packages to create the object.")]
        [Input("fixity", "List of booleans setting the fixities of the LinkConstraint. True denotes fixity. A list of 12 booleans in the following order: XtoX, YtoY, ZtoZ, XtoYY, XtoZZ, YtoXX, YtoZZ, ZtoXX, ZtoYY, XXtoXX, YYtoYY, ZZtoZZ.")]
        [Output("linkConstraint", "The created custom LinkConstraint.")]
        public static LinkConstraint LinkConstraint(string name, List<bool> fixity)
        {
            if (fixity.IsNullOrEmpty())
                return null;

            if (fixity.Count != 12)
            {
                Base.Compute.RecordError("The list of fixities is not equal to 12 and therefore the LinkConstraint cannot be created.");
                return null;
            }

            return new LinkConstraint
            {
                XtoX = fixity[0],
                YtoY = fixity[1],
                ZtoZ = fixity[2],
                XtoYY = fixity[3],
                XtoZZ = fixity[4],
                YtoXX = fixity[5],
                YtoZZ = fixity[6],
                ZtoXX = fixity[7],
                ZtoYY = fixity[8],
                XXtoXX = fixity[9],
                YYtoYY = fixity[10],
                ZZtoZZ = fixity[11],
                Name = name
            };
        }

        /***************************************************/
    }
}






