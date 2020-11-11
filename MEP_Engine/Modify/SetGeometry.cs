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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.oM.Reflection.Attributes;
using BH.oM.MEP.Elements;
using BH.oM.Geometry;

using BH.Engine.Geometry;

using BH.Engine.Base;

namespace BH.Engine.MEP
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        [Description("Updates the position of a Node.")]
        [Input("node", "The Node to set the postion to.")]
        [Input("point", "The new position of the Node used to define an object.")]
        [Output("node", "The Node with updated geometry.")]
        public static BH.oM.MEP.Elements.Node SetGeometry(this BH.oM.MEP.Elements.Node node, Point point)
        {
            BH.oM.MEP.Elements.Node clone = node.ShallowClone(true);
            clone.Position = point.DeepClone();
            return clone;
        }
        
        /***************************************************/
        
        [Description("Updates geometry of an IFlow Object by updating the positions of its end Nodes.")]
        [Input("obj", "The IFlow Object to update.")]
        [Input("curve", "The new centerline curve of the pipe.")]
        [Output("object", "The IFlow Object with updated geometry.")]
        public static IFlow SetGeometry(this IFlow obj, ICurve curve)
        {
            if(curve.IIsLinear() == false)
            {
                Engine.Reflection.Compute.RecordError("IFlow objects are not linear.");
                return null;
            }
            
            IFlow clone = obj.ShallowClone(true);
            clone.StartNode = clone.StartNode.SetGeometry(curve.IStartPoint());
            clone.EndNode = clone.EndNode.SetGeometry(curve.IEndPoint());
            return clone;
        }
        
        /***************************************************/
    }
}
