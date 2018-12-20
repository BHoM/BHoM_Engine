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

using BH.oM.Structure.Loads;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LoadType LoadType(this AreaTemperatureLoad load)
        {
            return oM.Structure.Loads.LoadType.AreaTemperature;
        }

        /***************************************************/

        public static LoadType LoadType(this AreaUniformalyDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.AreaUniformLoad;
        }

        /***************************************************/

        public static LoadType LoadType(this BarPointLoad load)
        {
            return oM.Structure.Loads.LoadType.BarPointLoad;
        }

        /***************************************************/

        public static LoadType LoadType(this BarPrestressLoad load)
        {
            return oM.Structure.Loads.LoadType.Pressure;
        }

        /***************************************************/

        public static LoadType LoadType(this BarTemperatureLoad load)
        {
            return oM.Structure.Loads.LoadType.BarTemperature;
        }

        /***************************************************/

        public static LoadType LoadType(this BarUniformlyDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.BarUniformLoad;
        }

        /***************************************************/

        public static LoadType LoadType(this BarVaryingDistributedLoad load)
        {
            return oM.Structure.Loads.LoadType.BarVaryingLoad;
        }

        /***************************************************/

        public static LoadType LoadType(this GravityLoad load)
        {
            return oM.Structure.Loads.LoadType.Selfweight;
        }

        /***************************************************/

        public static LoadType LoadType(this PointAcceleration load)
        {
            return oM.Structure.Loads.LoadType.PointAcceleration;
        }

        /***************************************************/

        public static LoadType LoadType(this PointDisplacement load)
        {
            return oM.Structure.Loads.LoadType.PointDisplacement;
        }

        /***************************************************/

        public static LoadType LoadType(this PointForce load)
        {
            return oM.Structure.Loads.LoadType.PointForce;
        }

        /***************************************************/

        public static LoadType LoadType(this PointVelocity load)
        {
            return oM.Structure.Loads.LoadType.PointVelocity;
        }

        /***************************************************/

    }

}
