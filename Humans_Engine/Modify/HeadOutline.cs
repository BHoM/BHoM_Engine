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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Humans.ViewQuality;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Humans.Modify
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Update the HeadOutline of a Spectator.")]
        [Input("spectator", "Spectator to update.")]
        [Input("newHeadOutline", "2d, closed, planar reference Polyline that represents the outline of the head. " +
            "The headOutline should be created in the XY plane where the origin represents the reference eye location of the spectator." +
            "If none provided the default is a simple Polyline based on an ellipse with major radius of 0.11 and minor radius of 0.078.")]
        public static void HeadOutline(this Spectator spectator, Polyline newHeadOutline)
        {
            if (spectator == null || newHeadOutline.IsNull())
                return;

            if (!newHeadOutline.IsPlanar() || !newHeadOutline.IsClosed())
            {
                Base.Compute.RecordError("The reference headOutline must be closed and planar.");
                return;
            }
            //local cartesian
            Cartesian local = spectator.Cartesian();

            //transform the reference head outline
            TransformMatrix transform = Geometry.Create.OrientationMatrixGlobalToLocal(local);
            spectator.HeadOutline = newHeadOutline.Transform(transform);

        }

        /***************************************************/

        [Description("Update the HeadOutlines of all Spectators in an Audience.")]
        [Input("audience", "Audience to update.")]
        [Input("newHeadOutline", "2d, closed, planar reference Polyline that represents the outline of the head. " +
        "The headOutline should be created in the XY plane where the origin represents the reference eye location of the spectator." +
        "If none provided the default is a simple Polyline based on an ellipse with major radius of 0.11 and minor radius of 0.078.")]
        public static void HeadOutline(this Audience audience, Polyline newHeadOutline)
        {
            if (audience == null || newHeadOutline.IsNull())
                return;

            foreach (Spectator spectator in audience.Spectators)
                spectator.HeadOutline(newHeadOutline);
        }
    }
}

