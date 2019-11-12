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
using BH.oM.Base;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Profiles                 ****/
        /***************************************************/

        public static string Description(this ISectionProfile profile)
        {
            return "I " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.FlangeThickness;// + "x" + profile.RootRadius + "x" + profile.ToeRadius;
        }

        /***************************************************/

        public static string Description(this BoxProfile profile)
        {
            return "Box " + profile.Height + "x" + profile.Width + "x" + profile.Thickness;
        }

        /***************************************************/

        public static string Description(this AngleProfile profile)
        {
            return "Angle " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.FlangeThickness;// + "x" + profile.RootRadius + "x" + profile.ToeRadius;
        }

        /***************************************************/

        public static string Description(this ChannelProfile profile)
        {
            return "Channel " + profile.Height + "x" + profile.FlangeWidth + "x" + profile.WebThickness + "x" + profile.FlangeThickness;// + "x" + profile.RootRadius + "x" + profile.ToeRadius;
        }

        /***************************************************/

        public static string Description(this CircleProfile profile)
        {
            return "Circle " + profile.Diameter;
        }

        /***************************************************/

        public static string Description(this FabricatedBoxProfile profile)
        {
            return "FabBox " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.TopFlangeThickness + "x" + profile.BotFlangeThickness;// + "x" + profile.WeldSize;
        }

        /***************************************************/

        public static string Description(this GeneralisedFabricatedBoxProfile profile)
        {
            return "GenFabBox " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.TopFlangeThickness + "x" + profile.BotFlangeThickness + "x" + profile.TopLeftCorbelWidth + "x" + profile.TopRightCorbelWidth;
        }

        /***************************************************/

        public static string Description(this KiteProfile profile)
        {
            return "Kite " + profile.Angle1 + "x" + profile.Width1 + "x" + profile.Thickness;
        }

        /***************************************************/

        public static string Description(this FabricatedISectionProfile profile)
        {
            return "FabI " + profile.Height + "x" + profile.WebThickness + "x" + profile.TopFlangeWidth + "x" + profile.BotFlangeWidth + "x" + profile.TopFlangeThickness + "x" + profile.BotFlangeThickness;// + "x" + profile.WeldSize;
        }

        /***************************************************/

        public static string Description(this FreeFormProfile profile)
        {
            return "FreeForm";
        }

        /***************************************************/

        public static string Description(this RectangleProfile profile)
        {
            return "Rectangle " + profile.Height + "x" + profile.Width + "x" + profile.CornerRadius;
        }

        /***************************************************/

        public static string Description(this TSectionProfile profile)
        {
            return "T " + profile.Height + "x" + profile.Width + "x" + profile.WebThickness + "x" + profile.FlangeThickness; //+ "x" + profile.RootRadius + "x" + profile.ToeRadius;
        }

        /***************************************************/

        public static string Description(this GeneralisedTSectionProfile profile)
        {
            return "GenT " + profile.Height + "x" + profile.WebThickness + "x" + profile.LeftOutstandWidth + "x" + profile.LeftOutstandThickness + "x" + profile.RightOutstandWidth + "x" + profile.RightOutstandThickness;
        }

        /***************************************************/
        /**** Public Methods - Sections                 ****/
        /***************************************************/

        public static string Description(this SteelSection section)
        {
            return "Steel " + section.SectionProfile.IDescription() + " - " + section.Material.Name;
        }

        /***************************************************/

        public static string Description(this ConcreteSection section)
        {
            return "Concrete " + section.SectionProfile.IDescription() + " - " + section.Material.Name;
        }

        /***************************************************/

        public static string Description(this CableSection section)
        {
            return "Cable " + section.NumberOfCables + "x dia " + section.CableDiameter + " - " + section.Material.Name;
        }

        /***************************************************/

        public static string Description(this ExplicitSection section)
        {
            return "Explicit A: " + section.Area + " Iy: " + section.Iy + " Iz: " + section.Iz + " J: " + section.J + " - " + section.Material.Name;
        }        

        /***************************************************/
        /**** Public Methods - Surface Properties       ****/
        /***************************************************/

        public static string Description(this ConstantThickness property)
        {
            return "THK " + property.Thickness + " - " + property.Material.Name;            
        }



        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static string IDescription(this ISectionProperty section)
        {
            return Description(section as dynamic);
        }

        /***************************************************/

        public static string IDescription(this IProfile profile)
        {
            return Description(profile as dynamic);
        }

        /***************************************************/

        public static string IDescription(this ISurfaceProperty property)
        {
            return Description(property as dynamic);
        }


        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private static string Description(IObject obj)
        {
            return "";
        }

        /***************************************************/

    }
}
