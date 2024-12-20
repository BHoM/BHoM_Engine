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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Geometry;
using BH.oM.Physical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        [Description("Returns the centreline of an IFramingElement's bounding box. The bounding box is aligned to the local coordinate system of the IFramingElement.")]
        [Input("element", "The IFramingElement to query the bounding box centreline of.")]
        [Output("curve", "The bounding box centreline of the IFramingElement.")]
        public static ICurve BoundingBoxCentreline(this IFramingElement element)
        {   
            ICurve location = element?.Location;
            if (location == null)
                return null;

            Vector normal = null;
            normal = element.Normal();
            if (normal == null)
            {
                Engine.Base.Compute.RecordError("IFramingElement must have linear location line.");
                return null;
            }

            Vector tangent = (location.IEndPoint() - location.IStartPoint()).Normalise();

            Vector localx = tangent.CrossProduct(normal);

            if (element.Property is ConstantFramingProperty)
            {
                Point centre = (element.Property as ConstantFramingProperty)?.Profile?.Edges?.Bounds()?.Centre();

                if (centre == null)
                    return null;

                Vector sectionTranslate = centre - Point.Origin;

                return location.ITranslate(- localx * sectionTranslate.X + normal * sectionTranslate.Y);

            }
            else
            {
                Engine.Base.Compute.RecordError("Only suitable framing property for the action is ConstantFramingProperty.");
                return null;
            }
        }
    }
}





