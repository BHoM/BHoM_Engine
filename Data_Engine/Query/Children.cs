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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the child nodes of this node.")]
        [Input("node", "The node to query for its children.")]
        [Output("nodes", "Child nodes of the input node.")]
        public static IEnumerable<TNode> IChildren<TNode, T>(this TNode node) where TNode : INode<T>
        {
            return Children(node as dynamic) ?? new List<TNode>();
        }

        /***************************************************/

        [Description("Gets the child nodes of this node.")]
        [Input("node", "The node to query for its children.")]
        [Output("nodes", "Child nodes of the input node.")]
        public static List<DomainTree<T>> Children<T>(this DomainTree<T> node)
        {
            return node?.Children ?? new List<DomainTree<T>>();
        }

        /***************************************************/

        [Description("Gets the child nodes of this node.")]
        [Input("node", "The node to query for its children.")]
        [Output("nodes", "Child nodes of the input node.")]
        public static List<Tree<T>> Children<T>(this Tree<T> node)
        {
            return node?.Children?.Values?.ToList() ?? new List<Tree<T>>();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IEnumerable<INode<T>> Children<T>(this INode<T> node)
        {
            Base.Compute.RecordError("The method Values is not implemented for " + node.GetType().Name);
            return null;
        }

        /***************************************************/

    }
}



