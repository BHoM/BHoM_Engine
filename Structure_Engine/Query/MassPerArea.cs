/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Structure.SurfaceProperties;
using System;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the mass per area for the property as its thickness mutiplied by the density.")]
        [Input("constantThickness", "The ConstantThickness property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this ConstantThickness constantThickness)
        {
            return constantThickness.IsNull() ? 0 : constantThickness.Thickness * constantThickness.Material.Density;
        }

        /***************************************************/

        [NotImplemented]
        public static double MassPerArea(this Ribbed ribbedProperty)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static double MassPerArea(this Waffle ribbedProperty)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [Description("Calculates the mass per area for the property as its thickness mutiplied by the density.")]
        [Input("property", "The Layered property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this Layered property)
        {
            double density = 0;
            foreach (Layer layer in property.Layers)
                density += layer == null ? 0 : layer.Thickness * layer.Material.Density;

            return density;
        }

        /***************************************************/

        [NotImplemented]
        [Description("Gets the mass per area for a LoadingPanelProperty. This will always return 0.")]
        [Input("loadingPanelProperty", "The LoadingPanelProperty property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property. THis will always return 0 for a LoadingPanelProperty.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this LoadingPanelProperty loadingPanelProperty)
        {
            return 0;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Calculates the mass per area for the property.")]
        [Input("property", "The ISurfaceProperty property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double IMassPerArea(this ISurfaceProperty property)
        {
            return property.IsNull() ? 0 : MassPerArea(property as dynamic);
        }

        /***************************************************/
    }
}



