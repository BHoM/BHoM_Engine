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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.Geometry;
using BH.oM.Spatial;
using BH.oM.Lighting.Elements;

using BH.Engine.Geometry;
using BH.Engine.Spatial;

namespace BH.Engine.Lighting
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToRadianceString(this Luminaire luminaire, string addlRot = "-rx 0", string finalMvmt = "-t 0 0 0",int digits = 4)
        {
            double rx;
            double ry;
            double rz;
            string pointsString = "";
            Point pt = luminaire.Position;
            Vector dir = luminaire.Direction.Normalise();
            Vector lumTypeDir = -1*Vector.ZAxis;

            // Solve ZYX rotations based on dir relative to lum type's base dir
            Vector cross = lumTypeDir.CrossProduct(dir).Normalise();
            double angle = Math.Sqrt((dir.Length() * dir.Length()) * (lumTypeDir.Length() * lumTypeDir.Length())) + dir.DotProduct(lumTypeDir);
            if (cross.Length() == 0 && angle == Math.PI)
            {
                rx = 180;
                ry = 0;
                rz = 0;
            }
            else
            {
                Quaternion q = BH.Engine.Geometry.Create.Quaternion(cross.X, cross.Y, cross.Z, angle).Normalise();
                rx = Math.Atan2(2.0 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z) * (180 / Math.PI);
                ry = Math.Asin(-2.0 * (q.X * q.Z - q.W * q.Y)) * (180 / Math.PI);
                rz = Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z) * (180 / Math.PI);
            }
            pointsString += String.Format("{0} {1} {2} ", Math.Round(pt.X, digits), Math.Round(pt.Y, digits), Math.Round(pt.Z, digits));
            return String.Format("!xform {0} -rx {1} -ry {2} -rz {3} {4} -t {5} {6}", addlRot, Math.Round(rx, digits).ToString(), Math.Round(ry, digits).ToString(), Math.Round(rz, digits).ToString(), 
                finalMvmt, pointsString, "ies/" + luminaire.LuminaireType.Model + ".rad");
        }

    }
}


