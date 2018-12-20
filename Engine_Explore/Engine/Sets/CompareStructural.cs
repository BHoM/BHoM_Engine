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

using BHoM.Base;
using Engine_Explore.BHoM.Materials;
using Engine_Explore.BHoM.Structural.Elements;
using Engine_Explore.BHoM.Structural.Properties;
using Engine_Explore.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Engine.Sets
{
    public static partial class Compare
    {
        public static double Identity(BHoMObject a, BHoMObject b)
        {
            if (a.BHoM_Guid == b.BHoM_Guid)
                return 1;
            else if (a.Name == b.Name && a.Name.Length > 0)
                return 0.75;
            else
                return 0;
        }

        /***************************************************/

        public static double Value(Node a, Node b) //TODO
        {
            return 1 - Measure.Distance(a.Point, b.Point) / BHoM.Base.Tolerance.MIN_DIST;
        }

        /***************************************************/

        public static double Value(Material a, Material b) //TODO
        {
            double weight = 0;

            if (a.Type == b.Type) weight += 1; 

            return weight;
        }

        /***************************************************/

        public static double Value(SectionProperty a, SectionProperty b)  //TODO
        {
            double weight = 0;

            if (a.Shape == b.Shape) weight += 0.5;
            weight += 0.5 * Value(a.Material, b.Material);

            return weight;
        }

        /***************************************************/

        public static double Value(Bar a, Bar b) //TODO
        {
            double weight = 0;

            weight += 0.35 * Value(a.StartNode, b.StartNode);
            weight += 0.35 * Value(a.EndNode, b.EndNode);
            weight += 0.30 * Value(a.SectionProperty, b.SectionProperty);

            return weight;
        }
    }
}
