/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Fragments;
using BH.oM.Structure.MaterialFragments;
using BH.Engine.Geometry;
using BH.Engine.Reflection;
using BH.Engine.Spatial;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a ReinforcementDensity Fragment that can be owned by a Bar or IAreaElements.")]
        [Input("density", "The mass of reinforcing mateial per unit volume of reinforced material.")]
        [Input("material","The material of the reinforcement.")]
        [Output("reinforcementDensity", "The ReinforcementDensity to be added to Bar and IAreaElements.")]
        public static ReinforcementDensity ReinforcementDensity(double density, IMaterialFragment material = null)
        {
            return new ReinforcementDensity
            {
                Density = density,
                Material = material
            };
        }

    }
}


