/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of BHoM Geometry Points that are at the centre of each space")]
        [Input("panelsAsSpaces", "The nested collection of Environment Panels that represent the spaces to get the centre of")]
        [Output("centrePoints", "A collection of points at the centre of each space")]
        public static List<Point> SpaceCentres(this List<List<Panel>> panelsAsSpaces)
        {
            List<Point> centrePts = new List<Point>();
            foreach (List<Panel> space in panelsAsSpaces)
                centrePts.Add(space.SpaceCentre());

            return centrePts;
        }

        [Description("Returns a BHoM Geometry Points that is at the centre of the provided space")]
        [Input("panelsAsSpace", "The collection of Environment Panels that represent a single space to get the centre of")]
        [Output("centrePoint", "A point at the centre of the space")]
        public static Point SpaceCentre(this List<Panel> panelsAsSpace)
        {
            //Calculate the centre point of the space comprised of the building elements
            //Done using the centre of mass of the vertices

            List<Point> vertexPoints = new List<Point>();
            foreach (Panel be in panelsAsSpace)
                vertexPoints.AddRange(be.ToPolyline().IControlPoints());

            double centreX = 0;
            double centreY = 0;
            double centreZ = 0;

            foreach(Point p in vertexPoints)
            {
                centreX += p.X;
                centreY += p.Y;
                centreZ += p.Z;
            }

            centreX /= vertexPoints.Count;
            centreY /= vertexPoints.Count;
            centreZ /= vertexPoints.Count;

            return BH.Engine.Geometry.Create.Point(centreX, centreY, centreZ);
        }
    }
}

