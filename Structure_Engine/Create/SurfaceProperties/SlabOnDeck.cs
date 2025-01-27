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

using BH.oM.Structure.SurfaceProperties;
using BH.oM.Base.Attributes;
using BH.oM.Structure.MaterialFragments;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        [Description("Creates a SlabOnDeck SurfaceProperty based on a CorrugatedDeck and some quantity of material cast on top of it.")]
        [Input("deck", "The corrugated deck on which the slab is cast.")]
        [Input("thicknessOverFlutes", "The thickness of material above the top flute of the deck. This must be positive.", typeof(Length))]
        [Input("slabMaterial", "The material to cast on the deck, generally concrete.")]
        [Input("name", "A name for the slab on deck property.")]
        [Input("type", "The usage for the property, default is slab.")]
        [Output("property", "The resulting SlabOnDeck property.")]

        public static SlabOnDeck SlabOnDeck(CorrugatedDeck deck, double thicknessOverFlutes, IMaterialFragment slabMaterial = null, string name = "", PanelType type = PanelType.Slab)
        {
            if (deck.IsNull())
                return null;

            if (thicknessOverFlutes <= 0)
            {
                Base.Compute.RecordError("Thickness over flutes must be greater than zero.");
                return null;
            }

            return new SlabOnDeck
            {
                Name = name,
                SlabThickness = thicknessOverFlutes,
                Material = slabMaterial,
                DeckName = deck.Name,
                DeckMaterial = deck.Material,
                DeckHeight = deck.Height,
                DeckThickness = deck.Thickness,
                DeckBottomWidth = deck.BottomWidth,
                DeckTopWidth = deck.TopWidth,
                DeckSpacing = deck.Spacing,
                DeckVolumeFactor = deck.VolumeFactor,
                Direction = deck.Direction,
                PanelType = (type == PanelType.Undefined ? deck.PanelType : type)
            };
        }

        /***************************************************/
    }
}






