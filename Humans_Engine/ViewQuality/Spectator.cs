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

using BH.oM.Geometry;
using BH.oM.Humans.ViewQuality;
using BH.oM.Humans.BodyParts;
using System.Collections.Generic;
using System;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Spectator Spectator(Point location, Vector viewDirection,bool createHeadOutline = false, double scale = 1)
        {
            Eye eye = Humans.Create.Eye(location, viewDirection);

            Polyline outline = new Polyline();

            if (createHeadOutline)
            {
                GetHeadOutline(eye, scale);
            }

            return new Spectator
            {
                Eye = eye,

                HeadOutline = outline,

            };
        }

        /***************************************************/

        public static Polyline GetHeadOutline(Eye eye,double scale = 1)
        {
            //data should be in datasets
            double[] xcoords = { 0.025198, 0.025841, 0.040367, 0.065967, 0.097698, 0.129429, 0.15503, 0.169555, 0.170198, 0.152041, 0.141581, 0.122141, 0.097698, 0.073255, 0.053815, 0.043355, 0.025198 };

            double[] ycoords = { 0.004467, 0.036987, 0.066089, 0.086153, 0.093301, 0.086153, 0.066089, 0.036987, 0.004467, -0.084287, -0.107096, -0.122963, -0.128638, -0.122963, -0.107096, -0.084287, 0.004467 };

            var scaledX = Array.ConvertAll(xcoords, x => x* scale);

            var scaledY= Array.ConvertAll(ycoords, x => x * scale);

            Vector horiz = Geometry.Query.CrossProduct(Vector.ZAxis,eye.ViewDirection);

            Vector up = Geometry.Query.CrossProduct(horiz, eye.ViewDirection*-1);

            List<Point> points = OrientatePoints(up, horiz, eye.Location, scaledX, scaledY, 0);

            return Geometry.Create.Polyline(points);
        }
    }
}
