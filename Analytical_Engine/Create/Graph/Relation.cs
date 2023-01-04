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

using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.Engine.Analytical;
using BH.oM.Base;
using BH.oM.Diffing;
using BH.oM.Geometry;
using BH.Engine.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Spatial;
using BH.oM.Base.Attributes;

namespace BH.Engine.Analytical
{
    public static partial class Create
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Create a Relation from a source IBHoMObject,target IBHoMObject, subgraph, weight and curve.")]
        [Input("source", "The source IBHoMOBject.")]
        [Input("target", "The target IBHoMOBject.")]
        [Input("subgraph", "Optional sub Graph of this Relation. Default is a new Graph.")]
        [Input("weight", "Optional weight for the Relation. Default is 1.")]
        [Output("curve", "Optional Curve that represents the link between the source and target entities Default is null.")]
        public static Relation Relation(IBHoMObject source, IBHoMObject target, Graph subgraph = null, double weight = 1, ICurve curve = null)
        {
            if(source == null || target == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a relation from a null source or a null target.");
                return null;
            }

            if (subgraph == null)
                subgraph = new Graph();

            Relation relation = new Relation()
            {
                Source = source.BHoM_Guid,
                Target = target.BHoM_Guid,
                Subgraph = subgraph,
                Weight = weight,
                Curve = curve
            };
            return relation;
        }

        /***************************************************/
    }
}



