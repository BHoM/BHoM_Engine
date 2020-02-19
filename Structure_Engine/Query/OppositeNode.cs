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

using BH.oM.Structure.Elements;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("If the provided node belongs to the Bar, this method returns the Node on the end of the Bar. The node if checked if it belongs to the bar by comparing its Guid. If the provided node is neither start or end node, null is returned.")]
        [Input("bar", "The bar to get the oposite node from.")]
        [Input("node", "The node used to check for the oposite. This should be either the start or the end node of the Bar for the method to return anything.")]
        [Output("node", "The oposite node of the bar in relation to the one provided.")]
        public static Node OppositeNode(this Bar bar, Node node)
        {
            if (bar.EndNode.BHoM_Guid == node.BHoM_Guid)
                return bar.StartNode;
            else if (bar.StartNode.BHoM_Guid == node.BHoM_Guid)
                return bar.EndNode;
            else
            {
                Reflection.Compute.RecordError("The bar does not contain the provided node.");
                return null;
            }
        }

        /***************************************************/
    }

}

