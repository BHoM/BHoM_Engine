/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
    public class BarEndNodesDistanceComparer : IEqualityComparer<Bar>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public BarEndNodesDistanceComparer()
        {
            m_nodeComparer = new NodeDistanceComparer();
        }

        /***************************************************/

        public BarEndNodesDistanceComparer(int decimals)
        {
            m_nodeComparer = new NodeDistanceComparer(decimals);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(Bar bar1, Bar bar2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(bar1, bar2)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(bar1, null) || Object.ReferenceEquals(bar2, null))
                return false;

            if (m_nodeComparer.Equals(bar1.StartNode, bar2.StartNode))
            {
                return m_nodeComparer.Equals(bar1.EndNode, bar2.EndNode);
            }
            else if (m_nodeComparer.Equals(bar1.StartNode, bar2.EndNode))
            {
                return m_nodeComparer.Equals(bar1.EndNode, bar2.StartNode);
            }

            return false;
        }

        /***************************************************/

        public int GetHashCode(Bar bar)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(bar, null)) return 0;

            return m_nodeComparer.GetHashCode(bar.StartNode) ^ m_nodeComparer.GetHashCode(bar.EndNode);
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private NodeDistanceComparer m_nodeComparer;


        /***************************************************/
    }
}




