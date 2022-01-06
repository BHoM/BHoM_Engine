/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Loads;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this AreaUniformTemperatureLoad load)
        {
            return oM.Structure.Loads.LoadType.AreaTemperature;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this AreaUniformlyDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.AreaUniformLoad;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this BarPointLoad load)
        {
            return oM.Structure.Loads.LoadType.BarPointLoad;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this BarPrestressLoad load)
        {
            return oM.Structure.Loads.LoadType.Pressure;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this BarUniformTemperatureLoad load)
        {
            return oM.Structure.Loads.LoadType.BarTemperature;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this BarUniformlyDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.BarUniformLoad;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this BarVaryingDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.BarVaryingLoad;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this GravityLoad load)
        {
            return oM.Structure.Loads.LoadType.Selfweight;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this PointAcceleration load)
        {
            return oM.Structure.Loads.LoadType.PointAcceleration;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this PointDisplacement load)
        {
            return oM.Structure.Loads.LoadType.PointDisplacement;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this PointLoad load)
        {
            return oM.Structure.Loads.LoadType.PointForce;
        }

        /***************************************************/

        [Deprecated("3.1", "Old method for enum not used in any part of the code. Purpose to easy type switching which is better solved by dynamic casting. To be deleted.")]
        public static LoadType LoadType(this PointVelocity load)
        {
            return oM.Structure.Loads.LoadType.PointVelocity;
        }

        /***************************************************/

    }

}



