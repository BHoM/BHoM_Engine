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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Physical.Constructions;
using BH.oM.Physical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.Engine.Geometry;


namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a physical PadFoundation element. To generate elements compatible with structural packages refer to BH.oM.Structure.Elements.PadFoundation.")]
        [Input("location", "PlanarSurface defining the top face of the PadFoundation.")]
        [Input("construction", "Construction of the PadFoundation, containing its thickness and Material.")]
        [Input("name", "The name of the PadFoundation, default empty string.")]
        [Output("padFoundation", "The created physical PadFoundation.")]
        public static PadFoundation PadFoundation(PlanarSurface location, IConstruction construction, string name = "")
        {
            return location.IsNull() || construction.IsNull() ? null : new PadFoundation { Location = location, Construction = construction, Name = name };
        }

        /***************************************************/
    }
}






