/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BHPE = BH.oM.Physical.Elements;
using BHPC = BH.oM.Physical.Constructions;
using BH.oM.Physical.Reinforcement;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Reinforcement;
using BH.oM.Structure.Fragments;
using BH.Engine.Base;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a physical Wall from a Panel. The Wall will be assigned a Construction based on the SurfaceProperty of the Panel.")]
        [Input("panel", "The Panel to use as the base for the framing element.")]
        [Output("wall", "The created physical Wall based on the Panel element provided.")]
        public static BHPE.Wall Wall(Panel panel)
        {
            if (panel.IsNull())
                return null;

            //Get Construction
            ISurfaceProperty prop = panel.Property;
            BHPC.Construction construction = null;

            if (prop == null)
                Base.Compute.RecordWarning("The panel does not contain a surfaceProperty. Can not extract profile or material");
            else
                construction = panel.Property.IConstruction();

            //Get Location
            PolyCurve externalEdges = Geometry.Create.PolyCurve(panel.ExternalEdges.Select(x => x.Curve));
            List<PolyCurve> internalEdges = panel.Openings.Select(opening => Geometry.Create.PolyCurve(opening.Edges.Select(edge => edge.Curve))).ToList();

            BHPE.Wall surfaceElement = Physical.Create.Wall(construction, externalEdges, internalEdges);

            string name = panel.Name ?? "";
            surfaceElement.Name = name;

            if (panel.FindFragment<PanelRebarIntent>() != null || panel.Property.FindFragment<ReinforcementDensity>() != null)
            {
                Base.Compute.RecordWarning("The panel has reinforcement, but this is not yet implemented.");
            }

            return surfaceElement;
        }

        /***************************************************/

        [Description("Creates a physical Floor from a Panel. The Floor will be assigned a Construction based on the SurfaceProperty of the Panel.")]
        [Input("panel", "The Panel to use as the base for the framing element.")]
        [Output("floor", "The created physical Floor based on the Panel element provided.")]
        public static BHPE.Floor Floor(Panel panel)
        {
            if (panel.IsNull())
                return null;

            //Get Construction
            ISurfaceProperty prop = panel.Property;
            BHPC.Construction construction = null;

            if (prop == null)
                Base.Compute.RecordWarning("The panel does not contain a surfaceProperty. Can not extract profile or material");
            else
                construction = panel.Property.IConstruction();

            //Get Location
            PolyCurve externalEdges = Geometry.Create.PolyCurve(panel.ExternalEdges.Select(x => x.Curve));
            List<PolyCurve> internalEdges = panel.Openings.Select(opening => Geometry.Create.PolyCurve(opening.Edges.Select(edge => edge.Curve))).ToList();

            BHPE.Floor surfaceElement = Physical.Create.Floor(construction, externalEdges, internalEdges);

            string name = panel.Name ?? "";
            surfaceElement.Name = name;

            if (panel.FindFragment<PanelRebarIntent>() != null || panel.Property.FindFragment<ReinforcementDensity>() != null)
            {
                Base.Compute.RecordWarning("The panel has reinforcement, but this is not yet implemented.");
            }

            return surfaceElement;
        }

        /***************************************************/

        [Description("Creates a physical surface element from a Panel. The Floor will be assigned a Construction based on the SurfaceProperty of the Panel, and the PanelType of the SufaceProperty will determine what type of surface element to create, unless overridden.")]
        [Input("panel", "The Panel to use as the base for the framing element.")]
        [Input("structuralUsage", "The type of surface element to create. if Undefined, the type will be based on the panel's SurfaceProperty.")]
        [Output("surfaceElement", "The created surface element based on the Panel element provided.")]
        public static BHPE.ISurface SurfaceElement(Panel panel, StructuralUsage2D structuralUsage = StructuralUsage2D.Undefined)
        {
            if (structuralUsage == StructuralUsage2D.Undefined)
            {
                object result = panel.Property.PropertyValue("PanelType");
                if (result != null)
                {
                    structuralUsage = (StructuralUsage2D)result;
                }
            }

            switch (structuralUsage)
            {
                case StructuralUsage2D.Wall:
                    return Wall(panel);
                case StructuralUsage2D.DropPanel:
                case StructuralUsage2D.PileCap:
                case StructuralUsage2D.Slab:
                case StructuralUsage2D.Undefined:
                default:
                    return Floor(panel);
            }
        }

        /***************************************************/

    }
}



