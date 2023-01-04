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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;
using System.Collections.Generic;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [PreviousInputNames("link", "bar")]
        [Description("Gets the Element0Ds of an ILink, which for the case of a ILink means getting the StartNode and EndNode. Method necessary for IElement pattern.")]
        [Input("link", "The ILink to extract IElement0ds from.")]
        [Output("element0Ds", "The list of Elements0D of the ILink, i.e. the StartNode and EndNode.")]
        public static List<IElement0D> Elements0D<TNode>(this ILink<TNode> link)
            where TNode : INode
        {
            return new List<IElement0D> { link.StartNode, link.EndNode };
        }

        /******************************************/
    }
}




