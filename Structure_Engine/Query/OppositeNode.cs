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

using BH.oM.Structure.Elements;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("If the provided node belongs to the Bar, this method returns the Node on the opposite end of the Bar. The Node is checked if it belongs to the Bar by comparing its Guid. If the provided Node is neither StartNode or EndNode, null is returned.")]
        [Input("bar", "The Bar to get the oposite Node from.")]
        [Input("node", "The Node used to check for the oposite. This should be either the StartNode or the EndNode of the Bar for the method to return anything.")]
        [Output("node", "The oposite Node of the Bar in relation to the one provided.")]
        public static Node OppositeNode(this Bar bar, Node node)
        {
            if (bar.IsNull() || node.IsNull())
                return null;

            if (bar.End.BHoM_Guid == node.BHoM_Guid)
                return bar.Start;
            else if (bar.Start.BHoM_Guid == node.BHoM_Guid)
                return bar.End;
            else
            {
                Base.Compute.RecordError("The Bar does not contain the provided Node.");
                return null;
            }
        }

        /***************************************************/
    }

}





