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

using BH.oM.Facade.Elements;
using BH.oM.Physical.FramingProperties;

using BH.Engine.Physical;
using BH.Engine.Spatial;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Quantities.Attributes;

using BH.Engine.Geometry;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a CurtainWall's solid volume.")]
        [Input("curtainWall", "The curtainWall to get the volume from")]
        [Output("volume", "The CurtainWall's solid volume", typeof(Volume))]
        public static double SolidVolume(this CurtainWall curtainWall)
        {
            if (curtainWall == null || curtainWall.Openings == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the solid volume of a null curtain wall");
                return 0;
            }

            double volume = 0;

            foreach (Opening opening in curtainWall.Openings)
                volume += opening.SolidVolume();

            if (curtainWall.ExternalEdges != null)
            {
                foreach (FrameEdge frameEdge in curtainWall.ExternalEdges)
                    volume += frameEdge.SolidVolume(); 
            }

            return volume;
        }


        /***************************************************/

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
            return volume + panel.Openings.Sum(x => x.SolidVolume()) + panel.ExternalEdges.Sum(x => x.SolidVolume());
        }


        /***************************************************/

        [Description("Returns an Openings solid volume based on its area, and construction thickness")]
        [Input("opening", "The Opening to get the volume from")]
        [Output("volume", "The Opening solid volume", typeof(Volume))]
        public static double SolidVolume(this Opening opening)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the solid volume of a null opening.");
                return 0;
            }

            double glazedVolume = 0;
            double frameVolume = 0;

            if (opening.Edges != null && opening.Edges.Count != 0)
                foreach (FrameEdge edge in opening.Edges)
                {
                    frameVolume += edge.SolidVolume();
                }

            if (opening.OpeningConstruction != null)
            {
                if (frameVolume>0)
                {
                    double glazedArea = opening.ComponentAreas().Item1;
                    glazedVolume = glazedArea * opening.OpeningConstruction.IThickness();
                }
                else
                    glazedVolume = opening.Area() * opening.OpeningConstruction.IThickness();
            }

            return glazedVolume + frameVolume;
        }


        /***************************************************/

        [Description("Returns a FrameEdges solid volume based on its length and applied section properties")]
        [Input("frameEdge", "The FrameEdge to get the volume from")]
        [Output("volume", "The Opening solid volume", typeof(Volume))]
        public static double SolidVolume(this FrameEdge frameEdge)
        {
            if(frameEdge == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the solid volume of a null frame edge.");
                return 0;
            }

            double frameVolume = 0;

            if (frameEdge.FrameEdgeProperty != null)
            {
                double totalArea = 0;
                double frameLength = frameEdge.Length();
                List<ConstantFramingProperty> props = frameEdge.FrameEdgeProperty.SectionProperties;
                foreach (ConstantFramingProperty prop in props)
                {
                    double profArea = prop.AverageProfileArea();
                    totalArea += profArea;
                }

                frameVolume = frameLength * totalArea;
            }

            return frameVolume;
        }

        /***************************************************/
    }
}



