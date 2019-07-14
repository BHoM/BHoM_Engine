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

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ViewCone ViewCone(Vector up = null, Vector horiz = null,Point origin =null, double scale = 1.0, ViewConeEnum conetype = ViewConeEnum.ViewFrameArea)
        {
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
            //all coords in mm
            //coords should be in dataset namespace
            //each coordinate set is later mirrored to create a complete view cone
            double[] coneX;
            double[] coneY;
            switch (conetype)
            {
                case ViewConeEnum.DynamicConeArea:

                    coneX = new double[] { 0, 63.19951, 84.72067, 105.844444, 125.471906, 142.374011, 155.985056, 166.052471, 171.920272, 171.582365, 162.540237, 146.857892, 127.820782, 106.97819, 85.478666, 64.005213, 0 };

                    coneY  = new double[] { -79.476365, -92.800481, -92.786387, -88.734578, -79.98072, -66.695225, -50.040009, -31.026623, -10.346131, 11.084337, 30.438085, 45.089297, 55.073263, 60.348579, 61.347173, 59.641446, 50.511209 };

                    return new List<double[]> { coneX, coneY };

                case ViewConeEnum.StaticConeArea:

                    coneX = new double[] { -0.008136, 7.548412, 16.997918, 27.217361, 34.548925, 41.88076, 48.213106, 52.435109, 56.213162, 57.770071, 56.994269, 54.996491, 51.221881, 46.225199, 40.895078, 34.232081, 27.34661, 19.461345, 9.576501, 3.936608, 0.013036 };

                    coneY = new double[] { 50.301575, 50.037897, 48.088549, 44.424773, 40.427291, 34.763692, 27.989191, 20.770042, 11.440409, -3.433e-11, -10.552197, -20.104918, -31.879422, -42.099083, -50.985901, -59.984016, -66.982826, -73.204279, -77.982093, -79.265102, -79.476365 };

                    return new List<double[]> { coneX, coneY };

                case ViewConeEnum.ViewFrameArea:

                    coneX = new double[] { 0, 29, 29, 0 };

                    coneY = new double[] { 29, 29, -29, -29};

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
                    shiftV = (horiz * xcoords[i] + up * ycoords[i] * -1);
                    c1 = origin + shiftV;
                    clipPoly.Add(c1);
                }
            }
            else
            {
                for (int i = xcoords.Length - 2; i > 0; i--)
                {
                    shiftV = (horiz * xcoords[i] * -1 + up * ycoords[i] * -1);
                    c1 = origin + shiftV;
                    clipPoly.Add(c1);
                }
            }
            
            return clipPoly;
        }

        /***************************************************/

      



    }
}
