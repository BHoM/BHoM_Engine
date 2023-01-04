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

using BH.oM.Physical.Materials;
using BH.Engine.Physical;

using BH.oM.Environment.Elements;

using BH.Engine.Matter;

using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Gets all the Materials a Panel is composed of and in which ratios")]
        [Input("panel", "The Panel to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Panel is composed of and in which ratios")]
        public static MaterialComposition MaterialComposition(this Panel panel)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null panel.");
                return null;
            }

            if (panel.Construction == null || panel.Construction.IThickness() < oM.Geometry.Tolerance.Distance)
            {
                BH.Engine.Base.Compute.RecordError("The Panel does not have a construction assigned");
                return null;
            }

            MaterialComposition pMat = panel.Construction.IMaterialComposition();

            if (panel.Openings != null && panel.Openings.Count != 0)
            {
                List<MaterialComposition> matComps = new List<MaterialComposition>() { pMat };
                List<double> volumes = new List<double>() { panel.SolidVolume() };
                foreach (Opening opening in panel.Openings)
                {
                    matComps.Add(opening.MaterialComposition());

                    double tempVolume = opening.SolidVolume();
                    volumes.Add(tempVolume);
                    volumes[0] -= tempVolume;
                }

                return Matter.Compute.AggregateMaterialComposition(matComps, volumes);
            }

            return pMat;
        }

        [Description("Gets all the Materials a Opening is composed of and in which ratios")]
        [Input("opening", "The Opening to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Opening is composed of and in which ratios")]
        public static MaterialComposition MaterialComposition(this Opening opening)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null opening.");
                return null;
            }

            if (opening.OpeningConstruction == null && opening.FrameConstruction == null)
            {
                Engine.Base.Compute.RecordError("The Opening does not have any constructions assigned");
                return null;
            }

            List<MaterialComposition> comps = new List<MaterialComposition>();
            List<double> ratios = new List<double>();

            double glazedVolume = 0;
            double frameVolume = 0;

            if (opening.OpeningConstruction != null)
            {
                if (opening.InnerEdges != null && opening.InnerEdges.Count != 0)
                {
                    double innerArea = opening.InnerEdges.Polyline().Area();
                    glazedVolume = innerArea * opening.OpeningConstruction.IThickness();
                    frameVolume = (opening.Polyline().Area() - innerArea) * opening.FrameConstruction.IThickness();
                }
                else
                {
                    glazedVolume = opening.Polyline().Area() * opening.OpeningConstruction.IThickness();
                }
            }

            if (opening.FrameConstruction != null && opening.FrameConstruction.IThickness() > oM.Geometry.Tolerance.Distance)
            {
                comps.Add(opening.FrameConstruction.IMaterialComposition());
                ratios.Add(frameVolume);
            }

            if (opening.OpeningConstruction != null && opening.OpeningConstruction.IThickness() > oM.Geometry.Tolerance.Distance)
            {
                comps.Add(opening.OpeningConstruction.IMaterialComposition());
                ratios.Add(glazedVolume);
            }

            if(comps.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError("The Opening does not have any constructions assigned to get an aggregated material composition from");
                return null;
            }

            return BH.Engine.Matter.Compute.AggregateMaterialComposition(comps, ratios);
        }
    }
}



