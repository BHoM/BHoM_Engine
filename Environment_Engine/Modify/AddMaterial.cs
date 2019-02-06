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
        [Description("BH.Engine.Environment.Modify.AddMaterial => adds a single material to a given construction")]
        [Input("construction", "Construction - the environmental construction object to add the material to")]
        [Input("material", "The material to add")]
        [Output("Construction - modified construction to include the added material")]
        public static Construction AddMaterial(this Construction construction, IMaterial material)
        {
            return construction.AddMaterials(new List<IMaterial> { material });
        }

        [Description("BH.Engine.Environment.Modify.AddMaterial => adds a collection of materials to a given construction")]
        [Input("construction", "Construction - the environmental construction object to add the material to")]
        [Input("materials", "The materials to add")]
        [Output("Construction - modified construction to include the added materials")]
        public static Construction AddMaterials(this Construction construction, List<IMaterial> materials)
        {
            if (construction.Materials == null) construction.Materials = new List<IMaterial>();

            construction.Materials.AddRange(materials);
            return construction;
        }
    }
}
