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

            //Create the physical element
            BHPE.Floor surfaceElement = Physical.Create.Floor(construction, externalEdges, internalEdges);

            string name = panel.Name ?? "";
            surfaceElement.Name = name;

            if (panel.FindFragment<PanelRebarIntent>() != null || panel.Property.FindFragment<ReinforcementDensity>() != null)
            {
                Base.Compute.RecordWarning("The panel has reinforcement, but embedding this information in the physical element is not yet implemented.");
            }

            return surfaceElement;
        }
        
        /***************************************************/
    }
}

