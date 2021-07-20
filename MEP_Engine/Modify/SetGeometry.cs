/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.MEP.System;
using BH.oM.MEP.Fixtures;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Base;
using BH.oM.MEP.System.Fittings;

namespace BH.Engine.MEP
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Updates geometry of an IFlow Object by updating the positions of its end Nodes.")]
        [Input("flowObj", "The IFlow Object to update.")]
        [Input("curve", "The new centerline curve of the pipe.")]
        [Output("object", "The IFlow Object with updated geometry.")]
        public static IFlow SetGeometry(this IFlow flowObj, ICurve curve)
        {
            if (curve.IIsLinear() == false)
            {
                Engine.Reflection.Compute.RecordError("IFlow objects are not linear.");
                return null;
            }

            IFlow clone = flowObj.ShallowClone();
            clone.StartPoint = clone.StartPoint.SetGeometry(curve.IStartPoint());
            clone.EndPoint = clone.EndPoint.SetGeometry(curve.IEndPoint());
            return clone;
        }        

        
        /***************************************************/
        
        public static Fitting SetGeometry(this Fitting fittingObj, Point location, List<Point> connectionLocations)
        {
            if (connectionLocations.Count < 2)
            {
                Engine.Reflection.Compute.RecordError("A fitting requires at least 2 connections, e.g. an elbow fitting has 2.");
                return null;
            }
            Fitting clone = fittingObj.ShallowClone();
            clone.Location = location;
            clone.ConnectionsLocation = connectionLocations;
            return clone;
        }        

        /***************************************************/
    }
}
