/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public class NodeDistanceComparer : IEqualityComparer<Node>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public NodeDistanceComparer()
        {
            //TODO: Grab tolerance from global tolerance settings
            m_multiplier = 1000;
        }

        /***************************************************/

        public NodeDistanceComparer(int decimals)
        {
            m_multiplier = Math.Pow(10, decimals);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(Node node1, Node node2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(node1, node2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(node1, null) || Object.ReferenceEquals(node2, null))
                return false;

            //Check whether any of the compared objects nodes are null.
            if (Object.ReferenceEquals(node1.Position, null) || Object.ReferenceEquals(node2.Position, null))
                return false;

            if ((int)Math.Round(node1.Position.X * m_multiplier) != (int)Math.Round(node2.Position.X * m_multiplier))
                return false;

            if ((int)Math.Round(node1.Position.Y * m_multiplier) != (int)Math.Round(node2.Position.Y * m_multiplier))
                return false;

            if ((int)Math.Round(node1.Position.Z * m_multiplier) != (int)Math.Round(node2.Position.Z * m_multiplier))
                return false;

            return true;
        }

        /***************************************************/

        public int GetHashCode(Node node)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(node, null)) return 0;

            //Check whether the position is null
            if (Object.ReferenceEquals(node.Position, null)) return 0;

            int x = ((int)Math.Round(node.Position.X * m_multiplier)).GetHashCode();
            int y = ((int)Math.Round(node.Position.Y * m_multiplier)).GetHashCode();
            int z = ((int)Math.Round(node.Position.Z * m_multiplier)).GetHashCode();
            return x ^ y ^ z;

        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private double m_multiplier;


        /***************************************************/
    }
}






