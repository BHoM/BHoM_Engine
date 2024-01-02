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

using BH.oM.Environment.Elements;

using BH.Engine.Physical;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Quantities.Attributes;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Returns an Openings solid volume based on its area, and construction thickness")]
        [Input("panel", "The Panel to get the volume from")]
        [Output("volume", "The Panel solid volume", typeof(Volume))]
        public static double SolidVolume(this Panel panel)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the solid volume of a null panel.");
                return 0;
            }

            double constructionThickness = 0;
            if (panel.Construction != null)
                constructionThickness = panel.Construction.IThickness();

            double volume = panel.Area() * constructionThickness;
            return volume + panel.Openings.Sum(x => x.SolidVolume());
        }

        [Description("Returns an Openings solid volume based on its area, and construction thickness")]
        [Input("opening", "The Opening to get the volume from")]
        [Output("volume", "The Opening solid volume", typeof(Volume))]
        public static double SolidVolume(this Opening opening)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the solid volume of a null opening.");
                return 0.0;
            }

            double glazedVolume = 0;
            double frameVolume = 0;

            if (opening.OpeningConstruction != null)
            {
                if (opening.InnerEdges != null && opening.InnerEdges.Count != 0)
                {
                    double innerArea = opening.InnerEdges.Polyline().Area();
                    glazedVolume = innerArea * opening.OpeningConstruction.IThickness();
                    
                    if(opening.FrameConstruction != null)
                        frameVolume = (opening.Polyline().Area() - innerArea) * opening.FrameConstruction.IThickness();
                }
                else
                    glazedVolume = opening.Polyline().Area() * opening.OpeningConstruction.IThickness();
            }

            return glazedVolume + frameVolume;
        }
    }
}




