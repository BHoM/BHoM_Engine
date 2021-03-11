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
            string pointsString = "";
            Point pt = luminaire.Position;
            pointsString += String.Format("{0} {1} {2} ", Math.Round(pt.X, digits), Math.Round(pt.Y, digits), Math.Round(pt.Z, digits));
            return String.Format("!xform {2} {3} -t {0} {1}", pointsString, luminaire.LuminaireType.Model, addlRot, finalMvmt);
        }

    }
}


