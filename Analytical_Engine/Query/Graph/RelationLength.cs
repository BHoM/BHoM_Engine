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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Dimensional;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the length of the ICurve on IRelations or distance between source and target entity if no ICurve has been defined.")]
        [Input("graph", "The Graph that owns the IRelation.")]
        [Input("relation", "The IRelation to query.")]
        [Output("length", "The length of the IRelation.")]
        public static double RelationLength(this Graph graph, IRelation relation)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the relation length of a null graph.");
                return 0;
            }

            if(relation == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the relation length for a graph if the relation to query is null.");
                return 0;
            }

            double length = 0;
            if (relation.Curve != null)
                length = relation.Curve.ILength();
            else
            {
                IElement0D source = graph.Entities[relation.Source] as IElement0D;
                IElement0D target = graph.Entities[relation.Target] as IElement0D;
                length = source.IGeometry().Distance(target.IGeometry());
            }
            return length;
        }
    }
}




