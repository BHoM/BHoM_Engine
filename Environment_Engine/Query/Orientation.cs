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
using BH.oM.Environment;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the orientation of a given environmental object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its orientation queried")]
        [Input("northAngle", "The angle in degrees for north. Default is 90.0.")]
        [Input("azimuthAngle", "Set to true to return the azimuth angle from north instead of the angle between north and the object normal. Default is false.")]
        [Output("orientation", "The orientation of the Environment Object")]
        public static double Orientation(this IEnvironmentObject environmentObject, double northAngle = 90.0, bool azimuthAngle = false)
        {
            double northAngleRadians = northAngle * (Math.PI / 180);
            Vector northVector = BH.Engine.Geometry.Create.Vector((double)Math.Cos(northAngleRadians), (double)Math.Sin(northAngleRadians));
            Polyline pLine = environmentObject.Polyline();
            List<Point> pts = pLine.DiscontinuityPoints();
            Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);
            Vector normal = plane.Normal;
            Vector up = BH.Engine.Geometry.Create.Vector(0, 0, 1);
            Vector down = BH.Engine.Geometry.Create.Vector(0, 0, -1);
            Vector right = BH.Engine.Geometry.Create.Vector(1, 0, 0);


            if (normal == up || normal == down)
            {
                BH.Engine.Reflection.Compute.RecordError("This method cannot successfully evaluate the orientation");
                return (-1);
            }

            else if (normal.X > 0 && northAngle == 90.0 && azimuthAngle == true)
            {
                return (360 - ((BH.Engine.Geometry.Query.Angle(normal, northVector) * (180 / Math.PI))));
            }

            else
            {
                double normalAngle = 0;

                if (normal.Y <= 0)
                {
                    normalAngle = (360 - ((BH.Engine.Geometry.Query.Angle(normal, right) * (180 / Math.PI))));
                }
                else
                {
                    normalAngle = (BH.Engine.Geometry.Query.Angle(normal, right) * (180 / Math.PI));
                }

                if ((normalAngle - northAngle) >= 180 && northAngle != 90.0 && northAngle <= 180 && azimuthAngle == true)
                {
                    return (360 - ((BH.Engine.Geometry.Query.Angle(normal, northVector) * (180 / Math.PI))));
                }

                else if (normalAngle < northAngle && normalAngle >= 0 && northAngle != 90.0 && northAngle <= 180 && azimuthAngle == true)
                {
                    return (360 - ((BH.Engine.Geometry.Query.Angle(normal, northVector) * (180 / Math.PI))));
                }

                else if (normalAngle < northAngle && normalAngle >= (northAngle - 180) && northAngle != 90.0 && northAngle >= 180 && azimuthAngle == true)
                {
                    return (360 - ((BH.Engine.Geometry.Query.Angle(normal, northVector) * (180 / Math.PI))));
                }

                return (BH.Engine.Geometry.Query.Angle(normal, northVector) * (180 / Math.PI));

            }
        }
    }
}
