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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Base;
using Engine_Explore.Engine;
using Engine_Explore.BHoM.Structural.Elements;
using Engine_Explore.BHoM.Structural.Properties;


namespace Engine_Explore.Engine.Convert
{
    public static class GsaCommand
    {
        public static string Write(Node node, string index)
        {
            string constraint = Write(node.Constraint);
            return  "NODE.2, " + index + ", " + node.Name + " , NO_RGB, " + 
                    node.Point.X + " , " + node.Point.Y + " , " + node.Point.Z + 
                    ", NO_GRID, " + 0 + ", REST," + constraint + ", STIFF,0,0,0,0,0,0";
        }

        /***************************************************/

        public static string Write(NodeConstraint constraint)
        {
            int X = ((constraint.UX == DOFType.Fixed) ? 1 : 0);
            int Y = ((constraint.UY == DOFType.Fixed) ? 1 : 0);
            int Z = ((constraint.UZ == DOFType.Fixed) ? 1 : 0);
            int XX = ((constraint.RX == DOFType.Fixed) ? 1 : 0);
            int YY = ((constraint.RY == DOFType.Fixed) ? 1 : 0);
            int ZZ = ((constraint.RZ == DOFType.Fixed) ? 1 : 0);

            return X + "," + Y + "," + Z + "," + XX + "," + YY + "," + ZZ;
        }
    }
}
