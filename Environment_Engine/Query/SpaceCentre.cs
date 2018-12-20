/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> SpaceCentres(this List<List<BuildingElement>> spaces)
        {
            List<Point> centrePts = new List<Point>();
            foreach (List<BuildingElement> space in spaces)
                centrePts.Add(space.SpaceCentre());

            return centrePts;
        }

        public static Point SpaceCentre(this List<BuildingElement> space)
        {
            //Calculate the centre point of the space comprised of the building elements
            //Done using the centre of mass of the vertices

            List<Point> vertexPoints = new List<Point>();
            foreach (BuildingElement be in space)
                vertexPoints.AddRange(be.PanelCurve.IControlPoints());

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

