/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Graphics;
using System.Collections.Generic;
using BH.Engine.Geometry;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes and returns the BoundingBox the SVGObject which is the concatenated BoudingBox of all Shapes contained within the object.")]
        public static BoundingBox Bounds(this SVGObject svg)
        {
            if(svg == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the bounding box of a null SVG object.");
                return null;
            }

            BoundingBox bb = new BoundingBox();
            List<IGeometry> geometry = svg.Shapes;

            for (int i = 0; i < svg.Shapes.Count; i++)
                bb += Engine.Geometry.Query.IBounds(svg.Shapes[i]);

            return bb;
        }

        /***************************************************/

        [Description("Computes and returns the full BoundingBox of all SVGObject in the provided list.")]
        public static BoundingBox Bounds(this List<SVGObject> svg)
        {
            if (svg == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the bounding box of a null SVG object.");
                return null;
            }

            BoundingBox bb = new BoundingBox();

            for (int i = 0; i < svg.Count; i++)
                bb += Bounds(svg[i]);

            return bb;
        }

        /***************************************************/

        [Description("Returns the BoundingBox the SVGDocuments which is the Canvas.")]
        public static BoundingBox Bounds(this SVGDocument svg)
        {
            if (svg == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the bounding box of a null SVG document.");
                return null;
            }

            return svg.Canvas;
        }

        /***************************************************/

        [Description("Computes and returns the full BoundingBox of all SVGDocuments in the provided list.")]
        public static BoundingBox Bounds(this List<SVGDocument> svg)
        {
            if (svg == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the bounding box of a null SVG document.");
                return null;
            }

            BoundingBox bb = new BoundingBox();

            for (int i = 0; i < svg.Count; i++)
                bb += Bounds(svg[i]);

            return bb;
        }

        /***************************************************/

        [Description("Computes and returns the BoundingBox of the RenderMesh as the bounding box of all Vertices.")]
        public static BoundingBox Bounds(this RenderMesh renderMesh)
        {
            if(renderMesh == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the bounding box of a null RenderMesh.");
                return null;
            }

            return renderMesh.Vertices.Select(x => x.Point).ToList().Bounds();
        }

        /***************************************************/

        [Description("Computes and returns the BoundingBox of the RenderPoint as the bounding box of its Point.")]
        public static BoundingBox Bounds(this RenderPoint renderPoint)
        {
            if (renderPoint == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the bounding box of a null RenderPoint.");
                return null;
            }

            return renderPoint.Point.Bounds();
        }

        /***************************************************/

        [Description("Computes and returns the BoundingBox of the RenderGeometry as the bounding box of the its Geometry.")]
        public static BoundingBox Bounds(this RenderGeometry renderGeometry)
        {
            if (renderGeometry == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the bounding box of a null RenderGeometry.");
                return null;
            }

            return renderGeometry.Geometry.IBounds();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Computes and returns the BoundingBox of the IRender.")]
        public static BoundingBox IBounds(this IRender render)
        {
            if (render == null)
            {
                Base.Compute.RecordError("Cannot query the bounding box of a null IRender.");
                return null;
            }
            return Bounds(render as dynamic);
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static BoundingBox Bounds(this IRender render)
        {
            object bounds;
            if (Base.Compute.TryRunExtensionMethod(render, "Bounds", out bounds) && bounds is BoundingBox)
                return bounds as BoundingBox;

            Base.Compute.RecordError($"Bounds is not implemented for IRender of type: {render.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}




