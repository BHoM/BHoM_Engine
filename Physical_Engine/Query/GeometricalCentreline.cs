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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Geometry;
using BH.oM.Physical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.oM.Reflection.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using System.ComponentModel;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        [Description("Returns the geometrical centreline of an IFramingElement.")]
        [Input("element", "The IFramingElement to query the geometrical centreline of.")]
        [Output("curve", "The geometrical centreline of the IFramingElement.")]
        public static ICurve GeometricalCentreline(this IFramingElement element)
        {   
            ICurve location = element.Location;

            Vector normal = null;

            try
            {
                normal = BH.Engine.Physical.Query.Normal(element);
            }
            catch
            {
                Engine.Reflection.Compute.RecordError("IFramingElement must have linear location line.");
                return null;
            }

            if (normal == null)
            {
                Engine.Reflection.Compute.RecordError("Was not able to compute element normal.");
                return null;
            }

            if (element.Property is ConstantFramingProperty)
            {
                ConstantFramingProperty constantProperty = element.Property as ConstantFramingProperty;

                IProfile profile = constantProperty.Profile;

                BoundingBox profileBounds = profile.Edges.Bounds();

                Point profileCentre = profileBounds.Centre();

                Point locationPoint = element.Location.Centroid();

                ICurve geometricalCentreline = location.ITranslate(profileCentre - locationPoint);

                return geometricalCentreline;
            }
            else
            {
                Engine.Reflection.Compute.RecordError("Element does not have a suitable framing property, was not able to extract section bounding box.");
                return null;
            }
        }
    }
}
