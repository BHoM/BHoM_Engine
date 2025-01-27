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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Returns the Frame Factor of an opening as a decimal representing the percentage of the opening which is made up of frame. Result is between 0 (0% frame) to 1 (100% frame)")]
        [Input("opening", "The Environments opening to query the frame factor from")]
        [Output("frameFactor", "The Frame Factor of the opening represented as a decimal")]
        public static double FrameFactor(this Opening opening)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the frame factor of a null opening.");
                return 0;
            }

            if(opening.InnerEdges == null || opening.InnerEdges.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning($"This opening with ID {opening.BHoM_Guid} does not contain any inner edges for the calculation. Assuming a 0% frame factor");
                return 0;
            }

            double outerArea = opening.Polyline().Area();
            double innerArea = opening.InnerEdges.Polyline().Area();

            double frameArea = outerArea - innerArea;

            return frameArea / outerArea;
        }
    }
}





