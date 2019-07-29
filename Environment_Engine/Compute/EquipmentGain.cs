/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Gains;
using BH.Engine.Geometry;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods                   ****/
        /***************************************************/

        [Description("Compute the sensible or latent equipment gain from the watts per meter squared and the area of the space")]
        [Input("sensibleWattsPerMeterSquared", "The sensible watts per meter squared from building code, default 0.0")]
        [Input("latentWattsPerMeterSquared", "The latent watts per meter squared from building code, default 0.0")]
        [Input("area", "The area of the space, default 0.0")]
        [Output("equipmentGain", "The calculated sensible or latent equipment gain with the sensible or latent watts for the space")]
        public static Equipment EquipmentGain(double sensibleWattsPerSquareMeter = 0.0, double latentWattsPerSquareMeter = 0.0, double area = 0.0)
        {
            return Create.Equipment(sensibleWattsPerSquareMeter * area, latentWattsPerSquareMeter * area);
        }
    }
}