/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Physical.FramingProperties;
using BH.oM.Facade.Elements;

using BH.Engine.Matter;
using BH.Engine.Physical;
using BH.Engine.Spatial;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all the Materials a Panel is composed of and in which ratios")]
        [Input("panel", "The Panel to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Panel is composed of and in which ratios")]
        public static MaterialComposition MaterialComposition(this Panel panel)
        {
            if (panel.Construction == null || panel.Construction.IThickness() < oM.Geometry.Tolerance.Distance)
            {
                BH.Engine.Reflection.Compute.RecordError("The Panel does not have a construction assigned");
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


        /***************************************************/

        [Description("Gets all the Materials a Opening is composed of and in which ratios")]
        [Input("opening", "The Opening to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Opening is composed of and in which ratios")]
        public static MaterialComposition MaterialComposition(this Opening opening)
        {
            if (opening.OpeningConstruction == null && opening.Edges == null)
            {
                Engine.Reflection.Compute.RecordError("The Opening does not have any constructions assigned");
                return null;
            }

            List<MaterialComposition> comps = new List<MaterialComposition>();
            List<double> ratios = new List<double>();

            double glazedVolume = 0;

            if (opening.OpeningConstruction != null && opening.OpeningConstruction.IThickness() > oM.Geometry.Tolerance.Distance)
            {
                if (opening.Edges != null && opening.Edges.Count != 0)
                {
                    double glazedArea = opening.ComponentAreas().Item1;
                    glazedVolume = glazedArea * opening.OpeningConstruction.IThickness();
                }
                else
                {
                    glazedVolume = opening.Area() * opening.OpeningConstruction.IThickness();
                }
                comps.Add(opening.OpeningConstruction.IMaterialComposition());
                ratios.Add(glazedVolume);
            }

            foreach (FrameEdge edge in opening.Edges)
            {
                if (edge.FrameEdgeProperty != null)
                {
                    comps.Add(edge.MaterialComposition());
                    ratios.Add(edge.SolidVolume());
                }
            }

            if (comps.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("The Opening does not have any constructions assigned to get an aggregated material composition from");
                return null;
            }

            return BH.Engine.Matter.Compute.AggregateMaterialComposition(comps, ratios);
        }


        /***************************************************/

        [Description("Gets all the Materials a Opening is composed of and in which ratios")]
        [Input("opening", "The Opening to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Opening is composed of and in which ratios")]
        public static MaterialComposition MaterialComposition(this FrameEdge frameEdge)
        {
            List<MaterialComposition> matComps = new List<MaterialComposition>();
            List<double> vols = new List<double>();

            if (frameEdge.FrameEdgeProperty == null)
            {
                Engine.Reflection.Compute.RecordError("The frame edge does not have a frame edge property assigned to get material composition from");
                return null;
            }

            List<ConstantFramingProperty> props = frameEdge.FrameEdgeProperty.SectionProperties;
            foreach (ConstantFramingProperty prop in props)
            {
                double profVolume = prop.AverageProfileArea()*frameEdge.Length();
                vols.Add(profVolume);
                List<Material> mats = new List<Material> { prop.Material };
                List<double> profVols = new List<double> { 1 };
                MaterialComposition matComp = new MaterialComposition(mats, profVols);
                matComps.Add(matComp);           
            }
            return BH.Engine.Matter.Compute.AggregateMaterialComposition(matComps, vols);
        }

        /***************************************************/
    }
}

