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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.MEP.System;
using BH.oM.MEP.System.Fittings;
using BH.oM.Geometry;
using BH.oM.Data.Collections;
using BH.Engine.Data;
using BH.Engine.Base;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Graph from a series of IFlow and Fitting obejcts.")]
        [Input("flows", "IFlow obejects to contruct the graph from.")]
        [Input("fittings", "Optional Fitting obejcts to contruct the grph from.")]
        [Input("snappingTolerance", "Tolerance, used to determine if two points are to be deemed to be the same.")]
        [Output("graph", "Graph consisting of flows, fittings and nodes in between.")]
        public static Graph Graph(IEnumerable<IFlow> flows, IEnumerable<Fitting> fittings = null, double snappingTolerance = Tolerance.MacroDistance)
        {
            if (flows == null)
                return null;

            fittings = fittings ?? new List<Fitting>();

            List<IRelation> relations = new List<IRelation>();

            PointMatrix<Node> matrix = Engine.Data.Create.PointMatrix<Node>(1);

            Dictionary<Guid, IBHoMObject> entities = new Dictionary<Guid, IBHoMObject>();

            foreach (IFlow flow in flows)
            {
                if (flow == null)
                    continue;

                entities[flow.BHoM_Guid] = flow;
                Point stPt = flow.StartPoint;
                Node stNode;
                if (!matrix.FindOrAdd(stPt, out stNode, snappingTolerance))
                {
                    entities[stNode.BHoM_Guid] = stNode;
                }
                Point enPt = flow.EndPoint;
                Node enNode;
                if (!matrix.FindOrAdd(enPt, out enNode, snappingTolerance))
                {
                    entities[enNode.BHoM_Guid] = enNode;
                }

                Point mid = (stPt + enPt) / 2;
                relations.Add(new Relation()
                {
                    Source = stNode.BHoM_Guid,
                    Target = flow.BHoM_Guid,
                    Curve = new Line() { Start = stPt, End = mid }
                });

                relations.Add(new Relation()
                {
                    Source = flow.BHoM_Guid,
                    Target = enNode.BHoM_Guid,
                    Curve = new Line() { Start = mid, End = enPt }
                });
            }

            foreach (Fitting fitting in fittings)
            {
                if (fitting == null || fitting.ConnectionsLocation == null || fitting.ConnectionsLocation.Count == 0)
                    continue;

                foreach (Point point in fitting.ConnectionsLocation)
                {
                    Node node;
                    if (!matrix.FindOrAdd(point, out node, snappingTolerance))
                    {
                        entities[node.BHoM_Guid] = node;
                    }

                    relations.Add(new Relation()
                    {
                        Source = node.BHoM_Guid,
                        Target = fitting.BHoM_Guid,
                        Curve = new Line() { Start = point, End = fitting.Location }
                    });
                }

                entities[fitting.BHoM_Guid] = fitting;
            }

            return new Graph
            {
                Entities = entities,
                Relations = relations
            };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool FindOrAdd(this PointMatrix<Node> pointMatrix, Point pos, out Node node, double tolerance)
        {
            LocalData<Node> closestData = pointMatrix.ClosestData(pos, tolerance);
            if (closestData != null)
            {
                node = closestData.Data;
                return true;
            }
            else
            {
                node = new Node { Position = pos };
                pointMatrix.Add(node.Position, node);
                return false;
            }
        }

        /***************************************************/
    }
}
