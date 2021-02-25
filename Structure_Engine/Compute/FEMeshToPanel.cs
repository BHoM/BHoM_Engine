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


using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Converting FEmesh to a panel")]
        [Input("FEMesh", "FEMesh to be converted to an Panel")]
        [Output("Panel", "Panel converted from a FEMesh.")]

        public static Panel FEMeshToPanel(FEMesh mesh)
        {
            if (mesh.Nodes == null)
            {
                Engine.Reflection.Compute.RecordWarning("Checks identify null nodes");
                return null;
            }
                List<Polyline> polylines = new List<Polyline>();

            List<Point> points = new List<Point>();

            foreach (Node node in mesh.Nodes)
            {
                points.Add(node.Position);
            }

            points.Add(mesh.Nodes.First().Position);
            polylines.Add(BH.Engine.Geometry.Create.Polyline(points));

            List<Panel> panels = new List<Panel>();
            if (mesh.Property != null)
            {
                panels = BH.Engine.Structure.Create.Panel(polylines.Cast<ICurve>().ToList(), mesh.Property);
                return panels[0];
            }
            else
            {
                panels = BH.Engine.Structure.Create.Panel(polylines.Cast<ICurve>().ToList());
                Engine.Reflection.Compute.RecordWarning("Meshs don't have any Section Property input");
                return panels[0];
            }

        }

        /***************************************************/

    }
}

