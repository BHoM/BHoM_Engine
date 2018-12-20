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
using Engine_Explore.BHoM.Structural.Elements;
using Engine_Explore.BHoM.Structural.Properties;
using Interop.gsa_8_7;
using System.Collections;

namespace Engine_Explore.Engine.Convert
{
    public static class GsaElement
    {
        public static string CustomKey { get; } = "GSA_id";

        public static Node Read(GsaNode gsaNode)
        {
            Node bhomNode = new Node
            {
                Name = gsaNode.Name,
                Point = new BHoM.Geometry.Point(gsaNode.Coor[0], gsaNode.Coor[1], gsaNode.Coor[2]),
                Constraint = ReadNodeConstraint("", gsaNode.Restraint, gsaNode.Stiffness)
            };

            bhomNode.CustomData[CustomKey] = gsaNode.Ref;
            return bhomNode;
        }

        /***************************************************/

        public static NodeConstraint ReadNodeConstraint(string name, int gsaRestraint, double[] gsaStiffness)
        {
            BitArray a = new BitArray(new int[] { gsaRestraint });
            bool[] fixities = { a[0], a[1], a[2], a[3], a[4], a[5] };

            return new NodeConstraint(name, fixities, gsaStiffness);
        }
    }
}
