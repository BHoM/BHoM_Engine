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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Materials;
using BH.oM.Spatial.ShapeProfiles;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Physical
{
    public static partial class Create
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a constant framing property to be used with the FramingElements. This property will represent a constant section across the full length of the element")]
        [Input("profile", "Profile of the cross section of the proeprty")]
        [Input("material", "Material of the profile")]
        [Input("orientationAngle", "Defines the rotation of the section around the axis of the element")]
        [Input("name", "The name of the constant framing property, default empty string")]
        public static ConstantFramingProperty ConstantFramingProperty(IProfile profile, Material material, double orientationAngle = 0, string name = "")
        {
            return new ConstantFramingProperty
            {
                Profile = profile,
                Material = material,
                OrientationAngle = orientationAngle,
                Name = name
            };
        }

        /***************************************************/
    }
}




