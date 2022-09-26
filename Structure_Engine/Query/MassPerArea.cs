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

        [PreviousInputNames("property","constantThickness")]
        [Description("Calculates the mass per area for the property as its thickness mutiplied by the density.")]
        [Input("property", "The ConstantThickness property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this ConstantThickness property)
        {
            if (property.IsNull())
                return 0;

            return property.Material.IsNull() ? 0 : property.Thickness * property.Material.Density;
        }

        /***************************************************/

        [Description("Calculates the mass per area for the property as its average thickness mutiplied by the density.")]
        [Input("property", "The Ribbed property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this Ribbed property)
        {
            if (property.IsNull())
                return 0;

            return property.Material.IsNull() ? 0 : property.VolumePerArea() * property.Material.Density;
        }

        /***************************************************/

        [Description("Calculates the mass per area for the property as its average thickness mutiplied by the density.")]
        [Input("property", "The waffle property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this Waffle property)
        {
            if (property.IsNull())
                return 0;

            return property.Material.IsNull() ? 0 : property.VolumePerArea() * property.Material.Density;
        }

        /***************************************************/

        [Description("Calculates the mass per area for the property as the sum of each layer's thickness multiplied by its density.")]
        [Input("property", "The Layered property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this Layered property)
        {
            if (property.IsNull())
                return 0;

            double density = 0;
            bool nullMat = false;
            foreach (Layer layer in property.Layers)
            {
                if (layer.Material == null)
                    nullMat = true;
                else
                    density += layer.Thickness * layer.Material.Density;
            }

            if (nullMat)
                Base.Compute.RecordWarning("At least one Material in a Layered surface property was null. Mass has been reported assuming this is a void.");

            return density;
        }

        /***************************************************/

        [Description("Calculates the mass per area for the property as the sum of the masses of the slab and the deck.")]
        [Input("property", "The Layered property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this SlabOnDeck property)
        {
            if (property.IsNull())
                return 0;

            double deckThickness = property.DeckThickness * property.DeckVolumeFactor;
            double slabThickness = property.VolumePerArea() - deckThickness;

            double deckDensity = property.DeckMaterial.IsNull() ? 0 : deckThickness * property.DeckMaterial.Density;
            double slabDensity = property.Material.IsNull() ? 0 : slabThickness * property.Material.Density;

            return slabDensity + deckDensity;
        }

        /***************************************************/

        [Description("Calculates the mass per area for the property as the sum of the masses of the slab and the deck.")]
        [Input("property", "The Layered property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this CorrugatedDeck property)
        {
            if (property.IsNull())
                return 0;

            return property.Material.IsNull() ? 0 : property.Thickness * property.VolumeFactor * property.Material.Density;
        }

        /***************************************************/

        [PreviousInputNames("property","loadingPanelProperty")]
        [Description("Gets the mass per area for a LoadingPanelProperty. This will always return 0.")]
        [Input("property", "The LoadingPanelProperty property to calculate the mass per area for.")]
        [Output("massPerArea", "The mass per area for the property. THis will always return 0 for a LoadingPanelProperty.", typeof(MassPerUnitArea))]
        public static double MassPerArea(this LoadingPanelProperty property)
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



