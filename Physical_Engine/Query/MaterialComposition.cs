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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;
using BH.Engine.Base;
using BH.oM.Physical.Materials;
using BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Constructions;
using BH.oM.Quantities.Attributes;
using BH.Engine.Matter;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all the Materials a IFramingElement is composed of and in which ratios.")]
        [Input("framingElement", "The IFramingElement to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the IFramingElement is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this IFramingElement framingElement)
        {
            if (framingElement == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null framing element.");
                return null;
            }

            if (framingElement.Property == null)
            {
                Engine.Base.Compute.RecordError("The MaterialComposition could not be queried as no Property has been assigned to the IFramingElement.");
                return null;
            }

            return framingElement.Property.IMaterialComposition();
        }

        /***************************************************/

        [Description("Gets all the Materials a ISurface is composed of and in which ratios.")]
        [Input("surface", "The ISurface to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the ISurface is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this ISurface surface)
        {
            VolumetricMaterialTakeoff takeoff = surface.IVolumetricMaterialTakeoff();
            if (takeoff == null)
                return null;

            return Matter.Create.MaterialComposition(takeoff);
        }

        /***************************************************/

        [Description("Gets all the Materials an IOpening is composed of and in which ratios.")]
        [Input("opening", "The IOpening to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the IOpening is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this IOpening opening)
        {
            VolumetricMaterialTakeoff takeoff = opening.IVolumetricMaterialTakeoff();
            if (takeoff == null)
                return null;

            return Matter.Create.MaterialComposition(takeoff);
        }

        /***************************************************/

        [Description("Gets all the Materials a SolidBulk is composed of and in which ratios.")]
        [Input("solidBulk", "The SolidBulk to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the SolidBulk is composed of and in which ratios.", typeof(Ratio))]
        public static MaterialComposition MaterialComposition(this SolidBulk solidBulk)
        {
            if (solidBulk == null)
            {
                return null;
            }

            if (solidBulk.MaterialComposition == null)
            {
                Engine.Base.Compute.RecordError("The SolidBulk MaterialComposition could not be queried as no Materials have been assigned to at least one of the layers of the Construction.");
                return null;
            }

            return solidBulk.MaterialComposition;
        }

        /***************************************************/

        [Description("Gets all the Materials a ExplicitBulk is composed of and in which ratios.")]
        [Input("explicitBulk", "The ExplicitBulk to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the ExplicitBulk is composed of and in which ratios.", typeof(Ratio))]
        public static MaterialComposition MaterialComposition(this ExplicitBulk explicitBulk)
        {
            if (explicitBulk == null)
            {
                return null;
            }

            if (explicitBulk.MaterialComposition == null)
            {
                Engine.Base.Compute.RecordError("The ExplicitBulk MaterialComposition could not be queried as no Materials have been assigned to at least one of the layers of the Construction.");
                return null;
            }

            return explicitBulk.MaterialComposition;
        }

        /***************************************************/

        [Description("Gets all the Materials a PadFoundation is composed of and in which ratios.")]
        [Input("padFoundation", "The PadFoundation to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the PadFoundation is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this PadFoundation padFoundation)
        {
            if (padFoundation.IsNull())
                return null;

            VolumetricMaterialTakeoff takeoff = padFoundation.IVolumetricMaterialTakeoff();

            return takeoff == null ? null : Matter.Create.MaterialComposition(takeoff);
        }

        /***************************************************/

        [Description("Gets all the Materials a PileFoundation is composed of and in which ratios.")]
        [Input("pileFoundation", "The PadFoundation to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the PadFoundation is composed of and in which ratios.")]
        public static MaterialComposition MaterialComposition(this PileFoundation pileFoundation)
        {
            if (pileFoundation.IsNull())
                return null;

            VolumetricMaterialTakeoff pileCapTakeoff = pileFoundation.PileCap.IVolumetricMaterialTakeoff();
            List<VolumetricMaterialTakeoff> allTakeOff = pileFoundation.Piles.Select(x => x.IVolumetricMaterialTakeoff()).ToList();

            if (pileCapTakeoff != null)
                allTakeOff.Add(pileCapTakeoff);

            VolumetricMaterialTakeoff takeOff = Matter.Compute.AggregateVolumetricMaterialTakeoff(allTakeOff);

            return takeOff == null ? null : Matter.Create.MaterialComposition(takeOff);
        }

        /******************************************************/
        /**** IConstruction Methods                        ****/
        /******************************************************/

        [Description("Gets all the Materials a ExplicitBulk is composed of and in which ratios.")]
        [Input("prop", "The ExplicitBulk to get the MaterialComposition from.")]
        [Output("materialComposition", "The kind of matter the ExplicitBulk is composed of and in which ratios.", typeof(Ratio))]
        public static MaterialComposition IMaterialComposition(this IConstruction prop)
        {
            return MaterialComposition(prop as dynamic);
        }

        /******************************************************/
        /**** IFramingElementProperty Methods              ****/
        /******************************************************/

        private static MaterialComposition IMaterialComposition(this IFramingElementProperty prop)
        {
            return MaterialComposition(prop as dynamic);
        }

        /******************************************************/
        /****           IOpening Methods                   ****/
        /******************************************************/

        private static MaterialComposition IMaterialComposition(this IOpening prop)
        {
            return MaterialComposition(prop as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static MaterialComposition MaterialComposition(this Construction prop)
        {
            if (prop == null)
            {
                Base.Compute.RecordError("Cannot evaluate MaterialComposition because the Construction was null.");
                return null;
            }

            if (prop.Layers == null) //.IsNullOrEmpty raises it's own error
            {
                Base.Compute.RecordError("Cannote evaluate MaterialComposition because the layers are null.");
                return null;
            }

            if (prop.Layers.Count == 0)
            {
                Base.Compute.RecordWarning($"Construction {(string.IsNullOrEmpty(prop.Name) ? "NoName" : prop.Name)} does not conatin any layers. An empty MaterialComposition is returned in its place.");
                return new MaterialComposition(new List<Material>(), new List<double>());
            }

            if (prop.Layers.All(x => x.Material == null))
            {
                Base.Compute.RecordError("Cannote evaluate MaterialComposition because all of the materials are null.");
                return null;
            }

            if (prop.Layers.Any(x => x.Material == null))
            {
                Base.Compute.RecordWarning("At least one Material in a Layered surface property was null. MaterialConstruction excludes this layer, assuming it is void space.");
            }

            IEnumerable<Layer> layers = prop.Layers.Where(x => x.Material != null);
            return Matter.Create.MaterialComposition(layers.Select(x => x.Material), layers.Select(x => x.Thickness));
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static MaterialComposition MaterialComposition(this IConstruction prop)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static MaterialComposition MaterialComposition(this ConstantFramingProperty prop)
        {
            if (prop.Material == null)
            {
                Engine.Base.Compute.RecordError("The ConstantFramingProperty MaterialComposition could not be queried as no Material has been assigned to the ConstantFramingProperty.");
                return null;
            }

            return (MaterialComposition)prop.Material;
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static MaterialComposition MaterialComposition(this IFramingElementProperty prop)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}


