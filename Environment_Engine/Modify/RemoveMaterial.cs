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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Interface;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("BH.Engine.Environment.Modify.AddMaterial => removes a single material from a given construction")]
        [Input("construction", "Construction - the environmental construction object to remove the material from")]
        [Input("material", "The material to remove")]
        [Output("Construction - modified construction to remove the material")]
        public static Construction RemoveMaterial(this Construction construction, IMaterial material)
        {
            return construction.RemoveMaterials(new List<IMaterial> { material });
        }

        [Description("BH.Engine.Environment.Modify.AddMaterial => removes a collection of materials from a given construction")]
        [Input("construction", "Construction - the environmental construction object to remove the material from")]
        [Input("materials", "The materials to remove")]
        [Output("Construction - modified construction to remove the materials")]
        public static Construction RemoveMaterials(this Construction construction, List<IMaterial> materials)
        {
            if (construction.Materials == null) return construction;

            foreach(IMaterial mat in materials)
                construction.Materials.Remove(mat);

            return construction;
        }
    }
}
