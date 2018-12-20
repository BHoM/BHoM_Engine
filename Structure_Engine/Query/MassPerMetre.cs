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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Section;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double MassPerMetre(this ISectionProperty section)
        {
            return section.Area * section.Material.Density;
        }

        /***************************************************/

        public static double MassPerMetre(this ConcreteSection section)
        {
            //TODO: Handle reinforcement
            return section.Area * section.Material.Density;
        }

        /***************************************************/

        public static double MassPerMetre(this CompositeSection section)
        {
            //TODO: Handle embedment etc..
            return section.ConcreteSection.MassPerMetre() + section.SteelSection.MassPerMetre();
        }

        /***************************************************/

        public static double MassPerMetre(this CableSection section)
        {
            //TODO: Add property for kg/m as part of the cable section?
            return section.Area * section.Material.Density;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IMassPerMetre(this ISectionProperty section)
        {
            return MassPerMetre(section as dynamic);
        }

        /***************************************************/
    }
}
