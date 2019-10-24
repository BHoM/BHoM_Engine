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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Deprecated("3.0", "Deprecated to remove name input", null, "LightReflectanceFragment(redReflectance, greenReflectance, blueReflectance)")]
        public static LightReflectanceFragment LightReflectanceFragment(string name = "", double redReflectance = 0.0, double greenReflectance = 0.0, double blueReflectance = 0.0)
        {
            return Create.LightReflectanceFragment(redReflectance, greenReflectance, blueReflectance);
        }

        [Description("Returns an Environment Light Reflectance Fragment object")]
        [Input("redReflectance", "The red reflectance of the light reflectance, default 0.0")]
        [Input("greenReflectance", "The green reflectance of the light reflectance, default 0.0")]
        [Input("blueReflectance", "The blue reflectance of the light reflectance, default 0.0")]
        [Output("lightReflectanceFragment", "A Light Reflectance Fragment object - this can be added to an Environmental Material fragment object")]
        public static LightReflectanceFragment LightReflectanceFragment(double redReflectance = 0.0, double greenReflectance = 0.0, double blueReflectance = 0.0)
        {
            return new LightReflectanceFragment
            {
                RedReflectance = redReflectance,
                GreenReflectance = greenReflectance,
                BlueReflectance = blueReflectance,
            };
        }
    }
}
