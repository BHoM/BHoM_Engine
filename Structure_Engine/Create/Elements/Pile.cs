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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural Pile. This object can be used with a PileFoundation or as a standalone foundation.")]
        [Input("topNode", "The node at the top of the pile.")]
        [Input("bottomNode", "The node at the bottom of the pile.")]
        [InputFromProperty("pileSection")]
        [InputFromProperty("orientationAngle")]
        [Output("pile", "The created Pile with a centreline defined by the provided nodes.")]
        public static Pile Pile(Node topNode, Node bottomNode, ISectionProperty pileSection = null, double orientationAngle = 0)
        {
            return topNode.IsNull() || bottomNode.IsNull() ? null : new Pile() { TopNode = topNode, BottomNode = bottomNode, Section = pileSection, OrientationAngle = orientationAngle };
        }

        /***************************************************/

        [Description("Creates a structural Pile. This object can be used with a PileFoundation or as a standalone foundation.")]
        [Input("line", "The definining geometry for the pile.")]
        [InputFromProperty("pileSection")]
        [InputFromProperty("orientationAngle")]
        [Output("pile", "The created Pile with a centreline matching the provided geometrical Line.")]
        public static Pile Pile(Line line, ISectionProperty pileSection = null, double orientationAngle = 0)
        {
            return Pile((Node)line.Start, (Node)line.End, pileSection, orientationAngle);
        }

        /***************************************************/

    }
}