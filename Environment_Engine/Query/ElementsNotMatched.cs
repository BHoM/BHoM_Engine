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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Base;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels which are not yet associated to spaces and are not shading panels")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("panelsAsSpaces", "A nested collection of Environment Panels representing spaces currently built")]
        [Output("panels", "A collection of Environment Panel objects which are not associated to a space")]
        public static List<Panel> ElementsNotMatched(this List<Panel> panels, List<List<Panel>> panelsAsSpaces)
        {
            //Find the panels that haven't been mapped yet
            List<Panel> notYetMapped = new List<Panel>();

            foreach (Panel p in panels)
            {
                if (!panelsAsSpaces.IsContaining(p))
                    notYetMapped.Add(p);
            }

            return notYetMapped;
        }
    }
}





