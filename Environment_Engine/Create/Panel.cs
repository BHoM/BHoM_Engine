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
using BH.oM.Physical.Constructions;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Create a collection of Environment Panels which form a space from a BHoM Boundary Representation (Brep)")]
        [Input("brep", "A BHoM Boundary Representation to convert into a collection of Environment Panels")]
        [Input("connectedSpaceName", "A name for the space which these panels are connected to. If no name is provided, a randomised default will be generated")]
        [Input("angleTolerance", "The angle tolerance for collapsing to polylines used when generating the external edges of the surfaces")]
        [Input("roofType", "The panel type to assign to the panels where the panels are identified as roof. If no input is added roof types are assigned by default.")]
        [Input("ceilingType", "The panel type to assign to the panels where the panels are identified as ceiling. If no input is added ceiling types are assigned by default.")]
        [Input("internalFloorType", "The panel type to assign to the panels where the panels are identified as internal flooring. If no input is added FloorInternal types are assigned by default.")]
        [Input("externalGrounding", "The panel type to assign to the panels where the panels are identified as external grounding. If no input is added SlabOnGrade types are assigned by default.")]
        [Input("externalWallType", "The panel type to assign to the panels where the panels are identified as external walls. If no input is added WallExternal types are assigned by default.")]
        [Input("internalWallType", "The panel type to assign to the panels where the panels are identified as internal walls. If no input is added WallInternal types are assigned by default.")]
        [Output("panelsAsSpace", "A collection of Environment Panels representing a closed space generated from the provided Brep geometry")]
        public static List<Panel> Panels(this BoundaryRepresentation brep, string connectedSpaceName = null, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, PanelType roofType = PanelType.Roof, PanelType ceilingType = PanelType.Ceiling, PanelType internalFloorType = PanelType.FloorInternal, PanelType externalGrounding = PanelType.SlabOnGrade, PanelType externalWallType = PanelType.WallExternal, PanelType internalWallType = PanelType.WallInternal)
        {
            if(brep == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create panels from a null brep.");
                return null;
            }

            return brep.Surfaces.ToList().Panels(connectedSpaceName, angleTolerance, roofType, ceilingType, internalFloorType, externalGrounding, externalWallType, internalWallType);
        }

        [Description("Create a collection of Environment Panels from a collection of BHoM Surfaces")]
        [Input("surfaces", "A collection of BHoM surfaces to convert into a Environment Panels. The surfaces should be grouped as a single space as all panels generated from the surfaces will have the same connectedSpaceName")]
        [Input("connectedSpaceName", "A name for the space which these panels are connected to. If no name is provided, a randomised default will be generated")]
        [Input("angleTolerance", "The angle tolerance for collapsing to polylines used when generating the external edges of the surfaces")]
        [Input("roofType", "The panel type to assign to the panels where the panels are identified as roof. If no input is added roof types are assigned by default.")]
        [Input("ceilingType", "The panel type to assign to the panels where the panels are identified as ceiling. If no input is added ceiling types are assigned by default.")]
        [Input("internalFloorType", "The panel type to assign to the panels where the panels are identified as internal flooring. If no input is added FloorInternal types are assigned by default.")]
        [Input("externalGrounding", "The panel type to assign to the panels where the panels are identified as external grounding. If no input is added SlabOnGrade types are assigned by default.")]
        [Input("externalWallType", "The panel type to assign to the panels where the panels are identified as external walls. If no input is added WallExternal types are assigned by default.")]
        [Input("internalWallType", "The panel type to assign to the panels where the panels are identified as internal walls. If no input is added WallInternal types are assigned by default.")]
        [Output("panel", "An Environment Panels representing a closed space generated from the provided surfaces")]
        public static List<Panel> Panels(this List<ISurface> surfaces, string connectedSpaceName = null, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, PanelType roofType = PanelType.Roof, PanelType ceilingType = PanelType.Ceiling, PanelType internalFloorType = PanelType.FloorInternal, PanelType externalGrounding = PanelType.SlabOnGrade, PanelType externalWallType = PanelType.WallExternal, PanelType internalWallType = PanelType.WallInternal)
        {
            if(surfaces == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create panels from null surfaces.");
                return null;
            }

            if (connectedSpaceName == null)
                connectedSpaceName = "auto" + Guid.NewGuid().ToString();

            List<Panel> panels = surfaces.Select(x => x.Panel(connectedSpaceName, angleTolerance)).Where(x => x != null).ToList();
                                
            panels = panels.SetRoofPanels(roofType, ceilingType, internalFloorType);

            panels = panels.SetFloorPanels(externalGrounding);

            panels = panels.SetWallPanels(externalWallType, internalWallType);
                                       
            return panels;
        }

        [Description("Create an Environments Panel from a BHoM Surface")]
        [Input("surface", "A BHoM surface to convert into an Environment Panel")]
        [Input("connectedSpaceName", "A name for the space which this panel is connected to. If no name is provided, a randomised default will be generated")]
        [Input("angleTolerance", "The angle tolerance for collapsing to polylines used when generating the external edges of the surfaces")]
        [Input("panelType", "The panel type to assign to the panel, default type is Undefined")]
        [Output("panel", "An Environment Panels representing a closed space generated from the provided Brep geometry")]
        public static Panel Panel(this ISurface surface, string connectedSpaceName = null, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, PanelType panelType = PanelType.Undefined)
        {
            if(surface.GetType() == typeof(NurbsSurface))
            {
                BH.Engine.Base.Compute.RecordError($"Creating Environmental Panels from surfaces of type NurbsSurface is not supported. Please extract the geometry manually and assign to panels using other create methods.");
                return null;
            }

            if (connectedSpaceName == null)
                connectedSpaceName = Guid.NewGuid().ToString();

            List<Polyline> openingLines = new List<Polyline>();
            if (surface.IInternalEdges() != null)
                openingLines = surface.IInternalEdges().Select(x => x.ICollapseToPolyline(angleTolerance)).ToList();

            List <Opening> openings = new List<Opening>();

            foreach(Polyline p in openingLines)
            {
                openings.Add(new Opening
                {
                    Edges = p.ToEdges(),
                    Type = OpeningType.Window,
                });
            }

            var externalEdges = surface.IExternalEdges();
            if(externalEdges == null)
            {
                BH.Engine.Base.Compute.RecordWarning($"Surface could not query external edges for surface being converted to panel with space name {connectedSpaceName}. Surface was of type {surface.GetType().Name}.");
                externalEdges = new List<ICurve>();
            }

           return new Panel
                {
                    ExternalEdges = externalEdges.Select(x => x.ICollapseToPolyline(angleTolerance)).ToList().Join().ToEdges(),
                    ConnectedSpaces = new List<string> { connectedSpaceName },
                    Openings = openings,
                    Type = panelType,
                };         
        }
    }
}




