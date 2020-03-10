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

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MaterialComposition MaterialComposition(this IFramingElement framingElement)
        {
            return framingElement.Property.IMaterialComposition();
        }

        /***************************************************/

        public static MaterialComposition MaterialComposition(this ISurface surface)
        {
            List<Layer> layers = (surface.Construction as Construction).Layers;
            return Matter.Create.MaterialComposition(layers.Select(x => x.Material), layers.Select(x => x.Thickness));
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        /******************************************************/
        /**** IConstruction Methods                        ****/
        /******************************************************/

        private static MaterialComposition IMaterialComposition(this IConstruction prop)
        {
            return MaterialComposition(prop as dynamic);
        }

        /***************************************************/

        private static MaterialComposition MaterialComposition(this Construction prop)
        {
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
