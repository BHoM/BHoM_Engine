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

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using System;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Normal(this Bar bar)
        {

            Point p1 = bar.StartNode.Position;
            Point p2 = bar.EndNode.Position;

            Vector tan = (p2 - p1).Normalise();
            Vector normal;

            if (!IsVertical(p1, p2))
            {
                normal = Vector.ZAxis;              
                normal = (normal - tan.DotProduct(normal) * tan).Normalise();
            }
            else
            {
                Vector locY = Vector.YAxis;
                locY = (locY - tan.DotProduct(locY) * tan).Normalise();
                normal = tan.CrossProduct(locY);
            }


            return normal.Rotate(bar.OrientationAngle, tan);
        }

        /***************************************************/

        public static Vector Normal(this PanelPlanar panel)
        {
            return panel.AllEdgeCurves().SelectMany(x => x.IControlPoints()).ToList().FitPlane().Normal;
        }

        /***************************************************/

        public static Vector Normal(this MeshFace face)
        {
            return face.Nodes.Select(x => x.Position).ToList().FitPlane().Normal;
        }

        /***************************************************/
        /**** Public Methods - Interface methods        ****/
        /***************************************************/

        public static Vector INormal(this IAreaElement areaElement)
        {
            return Normal(areaElement as dynamic);
        }

        /***************************************************/

    }
}