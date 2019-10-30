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

using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Structure.SectionProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Query
    {


        public static string Description(this BoxProfile profile)
        {
            return "Box " + profile.Height + "x" + profile.Width + "x" + profile.Thickness;
        }

        public static string Description(this KiteProfile profile)
        {
            return "Kite " + profile.Angle1 + "x" + profile.Width1 + "x" + profile.Thickness;
        }

        public static string Description(this AngleProfile profile)
        {
            return "Angle " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.FlangeThickness + "x" + profile.RootRadius + "x" + profile.ToeRadius;
        }

        public static string Description(this ChannelProfile profile)
        {
            return "Channel " + profile.Height + "x" + profile.WebThickness "x" + profile.Thickness;
        }

        public static string IDescription(this IProfile profile)
        {
            return Description(profile as dynamic);
        }

        public static string Description(this SteelSection section)
        {
            return "Steel " + section.SectionProfile.IDescription() + "-" + section.Material.Name;
        }

        public static string Description(this ConcreteSection section)
        {
            return "Concrete " + section.SectionProfile.IDescription() + "-" + section.Material.Name;
        }

        public static string Description(this ISectionProperty profile)
        {
            return Description(profile as dynamic);
        }

    }
}
