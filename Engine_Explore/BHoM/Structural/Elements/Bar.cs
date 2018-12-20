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

using Engine_Explore.BHoM.Base;
using Engine_Explore.BHoM.Geometry;
using Engine_Explore.BHoM.Structural.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Structural.Elements
{
    public class Bar : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Node StartNode { get; set; } = new Node();

        public Node EndNode { get; set; } = new Node();

        public SectionProperty SectionProperty { get; set; }

        /*public BarRelease Release { get; set; }

        public BarConstraint Spring { get; set; }

        public Offset Offset { get; set; }

        public BarStructuralUsage StructuralUsage { get; set; }

        public BarFEAType FEAType { get; set; }*/


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Bar() { }

        /***************************************************/

        public Bar(Point start, Point end, string name = "")
        {
            StartNode = new Node(start);
            EndNode = new Node(end);
            Name = name;
        }

        /***************************************************/

        public Bar(Node start, Node end, string name = "")
        {
            StartNode = start;
            EndNode = end;
            Name = name;
        }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
