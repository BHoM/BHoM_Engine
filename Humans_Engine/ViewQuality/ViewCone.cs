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
using BH.Engine.Geometry;
using BH.oM.Humans.ViewQuality;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a ViewCone")]
        [Input("up", "Vertical Vector for the ViewCone")]
        [Input("horiz", "Horizontal Vector for the ViewCone")]
        [Input("origin", "Origin point for the ViewCone")]
        [Input("scale", "Scaling the head outline if not using metres")]
        [Input("conetype", "Type of ViewCone to use")]
        public static ViewCone ViewCone(Vector up = null, Vector horiz = null,Point origin =null, double scale = 1.0, ViewConeEnum conetype = ViewConeEnum.ViewFrameArea)
        {
            //should check up  and horiz are in plane
            if (up == null) up = Vector.YAxis;
            if (horiz == null) horiz = Vector.XAxis;
            if (origin == null) origin = Point.Origin;
            List<Polyline> cones = SetViewCone(scale, conetype, up, horiz, origin);
            double area = cones.Sum(x => x.Area());
            return new ViewCone
            {

                ConeBoundary = cones,

                ConeArea = area,

            };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Polyline> SetViewCone(double scale, ViewConeEnum conetype, Vector up, Vector horiz,Point origin)
        {
            List<double[]> xycoords = GetConeCoords(conetype);

            List<Point> pointsL = OrientatePoints(up, horiz, origin, xycoords[0], xycoords[1],0);

            List<Point> pointsR = OrientatePoints(up, horiz, origin, xycoords[0], xycoords[1],1);

            return new List<Polyline> { Geometry.Create.Polyline(pointsL), Geometry.Create.Polyline(pointsR) };
        }

        /***************************************************/
        
        private static List<double[]> GetConeCoords(ViewConeEnum conetype)
        {
            //all coords in metres
            //coords should be in dataset namespace
            //each coordinate set is later mirrored to create a complete view cone
            double[] coneX;
            double[] coneY;
            switch (conetype)
            {
                case ViewConeEnum.DynamicConeArea:

                    coneX = new double[] { 0, 0.0632, 0.084721, 0.105844, 0.125472, 0.142374, 0.155985, 0.166052, 0.17192, 0.171582, 0.16254, 0.146858, 0.127821, 0.106978, 0.085479, 0.064005, 0 };

                    coneY  = new double[] { -0.079476, -0.0928, -0.092786, -0.088735, -0.079981, -0.066695, -0.05004, -0.031027, -0.010346, 0.011084, 0.030438, 0.045089, 0.055073, 0.060349, 0.061347, 0.059641, 0.050511 };

                    return new List<double[]> { coneX, coneY };

                case ViewConeEnum.StaticConeArea:

                    coneX = new double[] { -8.136e-6, 0.007548, 0.016998, 0.027217, 0.034549, 0.041881, 0.048213, 0.052435, 0.056213, 0.05777, 0.056994, 0.054996, 0.051222, 0.046225, 0.040895, 0.034232, 0.027347, 0.019461, 0.009577, 0.003937, 0.000013 };

                    coneY = new double[] { 0.050302, 0.050038, 0.048089, 0.044425, 0.040427, 0.034764, 0.027989, 0.02077, 0.01144, 0, -0.010552, -0.020105, -0.031879, -0.042099, -0.050986, -0.059984, -0.066983, -0.073204, -0.077982, -0.079265, -0.079476};

                    return new List<double[]> { coneX, coneY };

                case ViewConeEnum.ViewFrameArea:

                    coneX = new double[] { 0, 0.029, 0.029, 0 };

                    coneY = new double[] { 0.029, 0.029, -0.029, -0.029 };

                    return new List<double[]> { coneX, coneY };

                default:

                    return null;
                
            }
            
        }

        /***************************************************/

        private static List<Point> OrientatePoints(Vector up, Vector horiz, Point origin, double[] xcoords, double[] ycoords, int dir)
        {
            List<Point> clipPoly = new List<Point>();
            up = up.Normalise();
            horiz = horiz.Normalise();
            Point c1 = new Point();
            Vector shiftV = new Vector();
            if (dir == 1)
            {
                for (int i = 0; i < xcoords.Length; i++)
                {
                    shiftV = (horiz * xcoords[i] + up * ycoords[i]);
                    c1 = origin + shiftV;
                    clipPoly.Add(c1);
                }
            }
            else
            {
                for (int i = xcoords.Length - 1; i >= 0; i--)
                {
                    shiftV = (horiz * xcoords[i] * -1 + up * ycoords[i]);
                    c1 = origin + shiftV;
                    clipPoly.Add(c1);
                }
            }
            clipPoly.Add(clipPoly[0]);
            return clipPoly;
        }

        /***************************************************/

      



    }
}
