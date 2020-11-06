/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Elements;

using BH.Engine.Base;
using BH.oM.Physical.Materials;
using BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Constructions;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all the Materials a IFramingElement is composed of and in which ratios")]
        [Input("framingElement", "The IFramingElement to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the IFramingElement is composed of and in which ratios")]
        public static MaterialComposition MaterialComposition(this IFramingElement framingElement)
        {
            if (framingElement.Property == null)
            {
                Engine.Reflection.Compute.RecordError("The IFramingElement MaterialComposition could not be calculated as no Property has been assigned.");
                return null;
            }
            return framingElement.Property.IMaterialComposition();
        }

        /***************************************************/

        [Description("Gets all the Materials a ISurface is composed of and in which ratios")]
        [Input("surface", "The ISurface to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the ISurface is composed of and in which ratios")]
        public static MaterialComposition MaterialComposition(this ISurface surface)
        {
            if (surface.Construction == null)
            {
                Engine.Reflection.Compute.RecordError("The ISurface MaterialComposition could not be calculated as no IConstruction has been assigned.");
                return null;
            }
            return surface.Construction.IMaterialComposition();
        }

        /***************************************************/

        [Description("Gets all the Materials a BulkMaterial is composed of and in which ratios")]
        [Input("bulkMaterial", "The BulkMaterial to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the BulkMaterial is composed of and in which ratios", typeof(Ratio))]
        public static MaterialComposition MaterialComposition(this BulkMaterial bulkMaterial)
        {
            if (bulkMaterial.MaterialComposition == null)
            {
                Engine.Reflection.Compute.RecordError("The BulkMaterial MaterialComposition could not be calculated as no Materials have been assigned.");
                return null;
            }
            return Matter.Create.MaterialComposition(bulkMaterial.MaterialComposition().Materials, bulkMaterial.MaterialComposition().Ratios);
        }

        /******************************************************/
        /**** IConstruction Methods                        ****/
        /******************************************************/

        public static MaterialComposition IMaterialComposition(this IConstruction prop)
        {
            return MaterialComposition(prop as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static MaterialComposition MaterialComposition(this Construction prop)
        {
            if (prop.Layers.Any(x => x.Material == null))
            {
                Engine.Reflection.Compute.RecordError("The Construction MaterialComposition could not be calculated as no Material has been assigned.");
                return null;
            }
            return Matter.Create.MaterialComposition(prop.Layers.Select(x => x.Material), prop.Layers.Select(x => x.Thickness));
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static MaterialComposition MaterialComposition(this IConstruction prop)
        {
            throw new NotImplementedException();
        }

        /******************************************************/
        /**** IFramingElementProperty Methods              ****/
        /******************************************************/

        private static MaterialComposition IMaterialComposition(this IFramingElementProperty prop)
        {
            return MaterialComposition(prop as dynamic);
        }

        /***************************************************/

        private static MaterialComposition MaterialComposition(this ConstantFramingProperty prop)
        {
            if (prop.Material == null)
            {
                Engine.Reflection.Compute.RecordError("The ConstantFramingProperty MaterialComposition could not be calculated as no Material has been assigned.");
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
