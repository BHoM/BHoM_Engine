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
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the outline 1D elements of an Environment Opening")]
        [Input("opening", "An Environment Opening")]
        [Output("outlineElements", "A collection of outline 1D elements")]
        public static List<IElement1D> OutlineElements1D(this Opening opening)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the outline 1D elements of a null opening.");
                return null;
            }

            return opening.Polyline().ISubParts().Cast<IElement1D>().ToList();
        }

        [Description("Returns the outline 1D elements of an Environment Panel")]
        [Input("panel", "An Environment Panel")]
        [Output("outlineElements", "A collection of outline 1D elements")]
        public static List<IElement1D> OutlineElements1D(this Panel panel)
        {
            if (panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the outline 1D elements of a null panel.");
                return null;
            }

            return panel.Polyline().ISubParts().Cast<IElement1D>().ToList();
        }

        [Description("Returns the outline 1D elements of an Environments Space")]
        [Input("space", "An Environments Space with perimeter curve")]
        [Output("outlineElements", "A collection of outline 1D elements")]
        public static List<IElement1D> OutlineElements1D(this Space space)
        {
            if (space == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the outline 1D elements of a null space.");
                return null;
            }

            return space.Perimeter.ISubParts().Cast<IElement1D>().ToList();
        }
    }
}





