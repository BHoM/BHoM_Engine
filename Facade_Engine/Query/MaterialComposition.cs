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
using BH.oM.Physical.Constructions;
using BH.oM.Physical.FramingProperties;
using BH.oM.Facade.Elements;

using BH.Engine.Matter;
using BH.Engine.Physical;
using BH.Engine.Spatial;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all the Materials a CurtainWall is composed of and in which ratios.")]
        [Input("curtainWall", "The CurtainWall to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the CurtainWall is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this CurtainWall curtainWall)
        {
            if (curtainWall == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the Material Composition of a null curtain wall.");
                return null;
            }

            if (curtainWall.Openings == null || curtainWall.Openings.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError("CurtainWall has no openings. Please confirm the CurtainWall is valid and try again.");
                return null;
            }


            List<MaterialComposition> matComps = new List<MaterialComposition>() {};
            List<double> volumes = new List<double>() {};
            foreach (Opening opening in curtainWall.Openings)
            {
                matComps.Add(opening.MaterialComposition());
                volumes.Add(opening.SolidVolume());
            }
            foreach (FrameEdge extEdge in curtainWall.ExternalEdges)
            {
                MaterialComposition matComp = extEdge.MaterialComposition();
                if (matComp != null)
                {
                    matComps.Add(matComp);
                    volumes.Add(extEdge.SolidVolume());
                }
            }

            return Matter.Compute.AggregateMaterialComposition(matComps.Where(x => x != null).ToList(), volumes);
        }


        /***************************************************/

        [Description("Gets all the Materials a Panel is composed of and in which ratios.")]
        [Input("panel", "The Panel to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the Panel is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this Panel panel)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null panel.");
                return null;
            }

            if (panel.Construction == null)
            {
                BH.Engine.Base.Compute.RecordError("Panel " + panel.BHoM_Guid + " does not have a construction assigned.");
                return null;
            }

            if (panel.Construction.IThickness() < oM.Geometry.Tolerance.Distance)
            {
                BH.Engine.Base.Compute.RecordError("Panel " + panel.BHoM_Guid + "'s construction has a thickness of 0. MaterialComposition requires a panel construction with a thickness to work.");
                return null;
            }

            List<Layer> layers = (List<Layer>)Base.Query.PropertyValue(panel.Construction, "Layers");

            if (layers != null && layers.Any(x => x.Material == null))
            {
                Engine.Base.Compute.RecordError("Panel " + panel.BHoM_Guid + " has a layer with no material assigned. MaterialComposition only works for panels with materials assigned to all layers of their Construction.");
                return null;
            }

            MaterialComposition pMat = panel.Construction.IMaterialComposition();
            List<MaterialComposition> matComps = new List<MaterialComposition>() { pMat };
            List<double> volumes = new List<double>() { panel.SolidVolume() };

            if (panel.ExternalEdges != null)
            {
                foreach (FrameEdge extEdge in panel.ExternalEdges)
                {
                    if (extEdge.FrameEdgeProperty != null)
                    {
                        MaterialComposition matComp = extEdge.MaterialComposition();
                        if (matComp != null)
                        {
                            matComps.Add(matComp);
                            double edgeVol = extEdge.SolidVolume();
                            volumes.Add(edgeVol);
                            volumes[0] -= edgeVol;
                        }
                    }
                }
            }

            if (panel.Openings != null && panel.Openings.Count != 0)
            {
                foreach (Opening opening in panel.Openings)
                {
                    MaterialComposition matComp = opening.MaterialComposition();
                    if (matComp != null)
                    {
                        matComps.Add(matComp);
                        double tempVolume = opening.SolidVolume();
                        volumes.Add(tempVolume);
                        volumes[0] -= tempVolume;
                    }
                }  
            }

            return Matter.Compute.AggregateMaterialComposition(matComps, volumes);
        }


        /***************************************************/
        
        [Description("Gets all the Materials an Opening is composed of and in which ratios.")]
        [Input("opening", "The Opening to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the Opening is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this Opening opening)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null opening.");
                return null;
            }

            if (opening.OpeningConstruction == null || (opening.OpeningConstruction.IThickness() < oM.Geometry.Tolerance.Distance))
            {
                if (opening.Edges == null || !opening.Edges.Any(x => x.FrameEdgeProperty != null) || !opening.Edges.Any(x=>x.FrameEdgeProperty.SectionProperties.Count() != 0))
                {
                    Engine.Base.Compute.RecordWarning("Opening " + opening.BHoM_Guid + " does not have a frame edge property assigned to get material composition from.");
                    return null;
                }
                else
                    Engine.Base.Compute.RecordWarning("Opening " + opening.BHoM_Guid + " does not have a valid opening construction assigned. Material Composition is being calculated based on frame edges only.");
            }

            List<Layer> layers = (List<Layer>)Base.Query.PropertyValue(opening.OpeningConstruction, "Layers");

            if (layers != null && layers.Any(x => x.Material == null))
            {
                Engine.Base.Compute.RecordError("Opening " + opening.BHoM_Guid + " has a layer with no material assigned. MaterialComposition only works for openings with materials assigned to all layers of their Construction.");
                return null;
            }

            List<MaterialComposition> comps = new List<MaterialComposition>();
            List<double> ratios = new List<double>();

            double glazedVolume = 0;

            if (opening.OpeningConstruction != null && opening.OpeningConstruction.IThickness() >= oM.Geometry.Tolerance.Distance && layers != null)
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
                MaterialComposition matComp = opening.OpeningConstruction.IMaterialComposition();
                if (matComp != null && glazedVolume != 0)
                {
                    comps.Add(matComp);
                    ratios.Add(glazedVolume);
                }          
            }

            foreach (FrameEdge edge in opening.Edges)
            {
                if (edge.FrameEdgeProperty != null)
                {
                    MaterialComposition matComp = edge.MaterialComposition();
                    double vol = edge.SolidVolume();
                    if (matComp != null && vol != 0)
                    {
                        comps.Add(matComp);
                        ratios.Add(edge.SolidVolume());
                    }                   
                }
            }

            if (comps.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError("The Opening does not have any constructions assigned to get an aggregated material composition from");
                return null;
            }

            return BH.Engine.Matter.Compute.AggregateMaterialComposition(comps.Where(x => x != null).ToList(), ratios);
        }


        /***************************************************/

        [Description("Gets all the Materials a Opening is composed of and in which ratios.")]
        [Input("frameEdge", "The FrameEdge to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the Opening is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this FrameEdge frameEdge)
        {
            if (frameEdge == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null frame edge.");
                return null;
            }

            List<MaterialComposition> matComps = new List<MaterialComposition>();
            List<double> vols = new List<double>();

            if (frameEdge.FrameEdgeProperty == null || frameEdge.FrameEdgeProperty.SectionProperties.Count() == 0)
            {
                Engine.Base.Compute.RecordWarning("FrameEdge " + frameEdge.BHoM_Guid + " does not have a frame edge property assigned to get material composition from, so the material composition returned is empty.");
                return new MaterialComposition(new List<Material>(), new List<double>()); ;
            }

            if (frameEdge.FrameEdgeProperty.SectionProperties.Any(x => x.Material == null))
            {
                Engine.Base.Compute.RecordError("FrameEdge " + frameEdge.BHoM_Guid + " has a property with no material assigned. MaterialComposition only works for FrameEdges with materials assigned.");
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
            return BH.Engine.Matter.Compute.AggregateMaterialComposition(matComps.Where(x => x != null).ToList(), vols);
        }

        /***************************************************/
    }
}



