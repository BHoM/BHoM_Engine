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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Constructions;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction IConstruction(this ISurfaceProperty property)
        {
            if (property.IsNull())
                return null;

            return Construction(property as dynamic);
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this ConstantThickness property)
        {
            if (property.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material material = property.GetMaterial();

            oM.Physical.Constructions.Layer physicalLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = property.Thickness, Name = property.Name };

            return Physical.Create.Construction(property.Name, new List<oM.Physical.Constructions.Layer>() { physicalLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this CorrugatedDeck property)
        {
            if (property.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material material = property.GetMaterial();

            double thickness = property.Thickness * property.VolumeFactor;

            oM.Physical.Constructions.Layer physicalLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = thickness, Name = property.Name };

            return Physical.Create.Construction(property.Name, new List<oM.Physical.Constructions.Layer>() { physicalLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this Layered property)
        {
            if (property.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material material = property.GetMaterial();

            List<oM.Physical.Constructions.Layer> layers = property.Layers.Select(x => Physical.Create.Layer(x.Name, x.GetMaterial(), x.Thickness)).ToList();

            return Physical.Create.Construction(property.Name, layers);
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this Ribbed property)
        {
            if (property.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material material = property.GetMaterial();

            double topThickness = property.Thickness;
            double bottomThickness = property.TotalDepth - topThickness;

            oM.Physical.Constructions.Layer topLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = topThickness, Name = property.Name };
            oM.Physical.Constructions.Layer bottomLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = bottomThickness, Name = property.Name };

            return Physical.Create.Construction(property.Name, new List<oM.Physical.Constructions.Layer>() { topLayer, bottomLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this SlabOnDeck property)
        {
            if (property.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material slabMaterial = property.GetMaterial();
            oM.Physical.Materials.Material deckMaterial = Physical.Create.Material(property.DeckMaterial.Name, new List<oM.Physical.Materials.IMaterialProperties>() { property.DeckMaterial });

            double topThickness = property.SlabThickness;
            double bottomThickness = property.DeckHeight;
            double deckThickness = property.DeckThickness * property.DeckVolumeFactor;

            oM.Physical.Constructions.Layer topLayer = new oM.Physical.Constructions.Layer() { Material = slabMaterial, Thickness = topThickness, Name = property.Name };
            oM.Physical.Constructions.Layer bottomLayer = new oM.Physical.Constructions.Layer() { Material = slabMaterial, Thickness = bottomThickness, Name = property.Name };
            oM.Physical.Constructions.Layer deckLayer = new oM.Physical.Constructions.Layer() { Material = deckMaterial, Thickness = deckThickness, Name = property.Name };

            return Physical.Create.Construction(property.Name, new List<oM.Physical.Constructions.Layer>() { topLayer, bottomLayer, deckLayer});
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this Waffle property)
        {
            if (property.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material material = property.GetMaterial();

            double topThickness = property.Thickness;
            double bottomThickness = Math.Max(property.TotalDepthX, property.TotalDepthY);

            oM.Physical.Constructions.Layer topLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = topThickness, Name = property.Name };
            oM.Physical.Constructions.Layer bottomLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = bottomThickness, Name = property.Name };

            return Physical.Create.Construction(property.Name, new List<oM.Physical.Constructions.Layer>() { topLayer, bottomLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this ISurfaceProperty property)
        {
            Base.Compute.RecordError($"Construction() not implemented for type {property.GetType()}.");
            return null;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static oM.Physical.Materials.Material GetMaterial(this ISurfaceProperty property)
        {
            //Set Material
            oM.Physical.Materials.Material material = null;

            if (property.Material != null)
            {
                string matName = property.Material.DescriptionOrName();
                material = Physical.Create.Material(matName, new List<oM.Physical.Materials.IMaterialProperties> { property.Material });
            }
            else
            {
                Base.Compute.RecordWarning("Material from the SurfaceProperty is null.");
            }

            return material;
        }

        /***************************************************/

        private static oM.Physical.Materials.Material GetMaterial(this oM.Structure.SurfaceProperties.Layer layer)
        {
            //Set Material
            oM.Physical.Materials.Material material = null;

            if (layer.Material != null)
            {
                string matName = layer.Material.DescriptionOrName();
                material = Physical.Create.Material(matName, new List<oM.Physical.Materials.IMaterialProperties> { layer.Material });
            }
            else
            {
                Base.Compute.RecordWarning("Material from the Layer is null.");
            }

            return material;
        }
    }
}



