/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
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

        [Description("Returns the length of the ICurve on SpatialRelations or distance between source and target entity if no ICurve has been defined.")]
        [Input("graph", "The Graph that owns the SpatialRelation.")]
        [Input("spatialRelation", "The SpatialRelation to query.")]
        [Output("length", "The length of the SpatialRelation.")]

        public static double RelationLength(this Graph graph, SpatialRelation spatialRelation)
        {
            double length = 0;
            if (spatialRelation.Curve != null)
                length = spatialRelation.Curve.ILength();
            else
            {
                IElement0D source = m_SpatialGraph.Entities[spatialRelation.Source] as IElement0D;
                IElement0D target = m_SpatialGraph.Entities[spatialRelation.Target] as IElement0D;
                length = source.IGeometry().Distance(target.IGeometry());
            }
            return length;
        }
    }
}
