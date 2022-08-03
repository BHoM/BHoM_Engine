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

        [Description("Creates a physical Construction from a structural Layered SurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this Layered surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            List<oM.Physical.Constructions.Layer> layers = surfaceProperty.Layers.Select(x => Physical.Create.Layer(x.Name, Physical.Create.Material(x.Material) , x.Thickness)).ToList();

            return Physical.Create.Construction(surfaceProperty.Name, layers);
        }

        /***************************************************/
        /**** Fallback Method                           ****/
        /***************************************************/

        [Description("Creates a physical Construction from a structural ISurfaceProperty. Extracts the Structural MaterialFragment and creates a physical material with the same name.")]
        [Input("surfaceProperty", "Structural surface property to convert.")]
        [Output("construction", "The physical Construction to be used with ISurface such as Walls and Floors.")]
        public static Construction Construction(this ISurfaceProperty surfaceProperty)
        {
            if (surfaceProperty.IsNull())
                return null;

            MaterialComposition comp = surfaceProperty.IMaterialComposition();
            double volume = surfaceProperty.IVolumePerArea();
            double thickness = surfaceProperty.ITotalThickness();

            List<oM.Physical.Constructions.Layer> layers = new List<oM.Physical.Constructions.Layer>();

            for (int i = 0; i < comp.Materials.Count(); i++)
                layers.Add(new oM.Physical.Constructions.Layer
                {
                    Material = comp.Materials[i],
                    Thickness = volume * comp.Ratios[i],
                    Name = comp.Materials[i].Name
                });

            if (volume < thickness)
            {
                layers.Add(new oM.Physical.Constructions.Layer 
                {
                    Material = null, 
                    Thickness = thickness - volume, 
                    Name = "void"
                });

                Base.Compute.RecordNote("Void space was found in the makeup of the SurfaceProperty; This is often the result of ribbed or waffle properties, or layered properties with null materials. A void layer has been added to the Construction to maintain the total thickness.");
            }

            return Physical.Create.Construction(surfaceProperty.Name, layers);
        }

        /***************************************************/
    }
}