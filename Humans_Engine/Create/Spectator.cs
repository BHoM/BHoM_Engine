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

using BH.oM.Geometry;
using BH.oM.Humans.ViewQuality;
using BH.oM.Humans.BodyParts;
using BH.Engine.Humans;
using System.Collections.Generic;
using System;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Geometry.CoordinateSystem;
using BH.Engine.Geometry;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a Spectator.")]
        [Input("location", "Point defining the Eye location.")]
        [Input("viewDirection", "Vector defining the Eye view directions.")]
        [Input("headOutline", "2d, closed, planar reference Polyline that represents the outline of the head. " +
            "The headOutline should be created in the XY plane where the origin represents the reference eye location of the spectator." +
            "If none provided the default is a simple Polyline based on an ellipse with the defined majorRadius and minorRadius.")]
        [Input("majorRadius", "Major radius of the ellipse used as a headOutline if no head outline was provided. Default value is 0.11.")]
        [Input("minorRadius", "Minor radius of the ellipse used as a headOutline if no head outline was provided. Default value is 0.078.")]
        [Output("spectator", "Spectator with location, view direction and head outline defined.")]
        public static Spectator Spectator(Point location, Vector viewDirection, Polyline headOutline = null, double majorRadius = 0.11, double minorRadius = 0.078)
        {
            if (location.IsNull() || viewDirection.IsNull())
                return null;
            if (headOutline == null)
            {
                //create basic elliptical head form
                List<Point> points = new List<Point>();
                double theta = 2 * Math.PI / 16;
                for (int i = 0; i <= 16; i++)
                {
                    double x = minorRadius * Math.Cos(theta * i);
                    double y = majorRadius * Math.Sin(theta * i);
                    points.Add(Geometry.Create.Point(x, y, 0));
                }
                headOutline = Geometry.Create.Polyline(points);
            }

            if (!headOutline.IsPlanar() || !headOutline.IsClosed())
            {
                Base.Compute.RecordError("The reference headOutline must be closed and planar.");
                return null;
            }

            //create the head
            Head head = Humans.Create.Head(location, viewDirection);

            Spectator spectator = new Spectator() { Head = head };

            //local cartesian
            Cartesian local = spectator.Cartesian();

            //transform the reference head outline
            TransformMatrix transform = Geometry.Create.OrientationMatrixGlobalToLocal(local);
            spectator.HeadOutline = headOutline.Transform(transform);

            return spectator;
        }

    }
}



