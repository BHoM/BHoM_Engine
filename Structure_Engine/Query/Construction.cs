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
using BH.oM.Physical.Materials;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction IConstruction(this ISurfaceProperty surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            return Construction(surfaceProperty as dynamic);
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this ConstantThickness surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material material = surfaceProperty.GetMaterial();

            oM.Physical.Constructions.Layer physicalLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = surfaceProperty.Thickness, Name = surfaceProperty.Name };

            return Physical.Create.Construction(surfaceProperty.Name, new List<oM.Physical.Constructions.Layer>() { physicalLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this CorrugatedDeck surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material material = surfaceProperty.GetMaterial();

            double thickness = surfaceProperty.Thickness * surfaceProperty.VolumeFactor;
            double voidThickness = surfaceProperty.Height - thickness;

            oM.Physical.Constructions.Layer physicalLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = thickness, Name = surfaceProperty.Name };
            oM.Physical.Constructions.Layer voidLayer = new oM.Physical.Constructions.Layer() { Material = null, Thickness = voidThickness, Name = "void" };

            return Physical.Create.Construction(surfaceProperty.Name, new List<oM.Physical.Constructions.Layer>() { physicalLayer, voidLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this Layered surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            //Set Material
            oM.Physical.Materials.Material material = surfaceProperty.GetMaterial();

            List<oM.Physical.Constructions.Layer> layers = surfaceProperty.Layers.Select(x => Physical.Create.Layer(x.Name, x.GetMaterial(), x.Thickness)).ToList();

            return Physical.Create.Construction(surfaceProperty.Name, layers);
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this Ribbed surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            //Set Material
            Material material = surfaceProperty.GetMaterial();

            double physicalThickness = surfaceProperty.VolumePerArea();
            double voidThickness = surfaceProperty.TotalThickness() - physicalThickness;

            oM.Physical.Constructions.Layer physicalLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = physicalThickness, Name = "Slab" };
            oM.Physical.Constructions.Layer voidLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = voidThickness, Name = "Void" };

            return Physical.Create.Construction(surfaceProperty.Name, new List<oM.Physical.Constructions.Layer>() { physicalLayer, voidLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this SlabOnDeck surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            //Set Material
            MaterialComposition matComp = surfaceProperty.MaterialComposition();

            Material slabMaterial = matComp.Materials[0];
            Material deckMaterial = matComp.Materials[1];
            Material rebarMaterial = matComp.Materials[2];

            double slabThickness = surfaceProperty.VolumePerArea() * matComp.Ratios[0];
            double deckThickness = surfaceProperty.VolumePerArea() * matComp.Ratios[1];
            double rebarThickness = surfaceProperty.VolumePerArea() * matComp.Ratios[2];
            double voidThickness = surfaceProperty.TotalThickness() - (slabThickness + deckThickness + rebarThickness);

            oM.Physical.Constructions.Layer slabLayer = new oM.Physical.Constructions.Layer() { Material = slabMaterial, Thickness = slabThickness, Name = "Slab" };
            oM.Physical.Constructions.Layer deckLayer = new oM.Physical.Constructions.Layer() { Material = deckMaterial, Thickness = deckThickness, Name = "Deck" };
            oM.Physical.Constructions.Layer rebarLayer = new oM.Physical.Constructions.Layer() { Material = rebarMaterial, Thickness = rebarThickness, Name = "Rebar" };
            oM.Physical.Constructions.Layer voidLayer = new oM.Physical.Constructions.Layer() { Material = deckMaterial, Thickness = voidThickness, Name = "Void" };

            return Physical.Create.Construction(surfaceProperty.Name, new List<oM.Physical.Constructions.Layer>() { slabLayer, rebarLayer, deckLayer, voidLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this Waffle surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            //Set Material
            Material material = surfaceProperty.GetMaterial();

            double physicalThickness = surfaceProperty.VolumePerArea();
            double voidThickness = surfaceProperty.TotalThickness() - physicalThickness;

            oM.Physical.Constructions.Layer physicalLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = physicalThickness, Name = "Slab" };
            oM.Physical.Constructions.Layer voidLayer = new oM.Physical.Constructions.Layer() { Material = material, Thickness = voidThickness, Name = "Void" };

            return Physical.Create.Construction(surfaceProperty.Name, new List<oM.Physical.Constructions.Layer>() { physicalLayer, voidLayer });
        }

        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this ISurfaceProperty surfaceProperty)
        {
            Base.Compute.RecordError($"Construction() not implemented for type {surfaceProperty.GetType()}.");
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



