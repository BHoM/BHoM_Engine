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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Dimensional;
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Physical.Constructions;

using BH.oM.Base.Attributes;
 
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Returns the name of an elements primary defining construction property")]
        [Input("elem", "IElement2d to get construction property name for")]
        [Output("propName", "The name of the element's construction property")]
        public static string IPrimaryPropertyName(this IElement2D elem)
        {
            if (elem == null)
                return null;

            return PrimaryPropertyName(elem as dynamic);
        }

        [Description("Returns the name of an elements primary defining property")]
        [Input("elem", "IElement1d to get construction property name for")]
        [Output("propName", "The name of the element's construction property")]
        public static string IPrimaryPropertyName(this IElement1D elem)
        {
            if (elem == null)
                return null; 
            
            return PrimaryPropertyName(elem as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Returns the name of a frameEdge's primary defining property")]
        [Input("frameEdge", "FrameEdges to get primary property name for")]
        [Output("propName", "The name of the element's construction property")]
        public static string PrimaryPropertyName(this FrameEdge frameEdge)
        {
            if (frameEdge == null || frameEdge.FrameEdgeProperty == null)
                return "";

            return frameEdge.FrameEdgeProperty.Name;
        }

        [Description("Returns the name of a frameEdge's primary defining property")]
        [Input("edge", "edge to get primary property name for")]
        [Output("propName", "The name of the element's construction property")]
        public static string PrimaryPropertyName(this IEdge edge)
        {
            if (edge == null)
                return "";

            return edge.Name;
        }

        [Description("Returns the name of a panel's construction property")]
        [Input("panel", "Panel to get construction property name for")]
        [Output("propName", "The name of the element's construction property")]
        public static string PrimaryPropertyName(this Panel panel)
        {
            if (panel == null || panel.Construction == null)
                return "";

            return panel.Construction.Name;
        }

        [Description("Returns the name of an opening's construction property")]
        [Input("opening", "Opening to get construction property name for")]
        [Output("propName", "The name of the element's construction property")]
        public static string PrimaryPropertyName(this Opening opening)
        {
            if (opening == null || opening.OpeningConstruction == null)
                return "";

            return opening.OpeningConstruction.Name;
        }

        private static string PrimaryPropertyName(this IElement2D elem)
        {
            string nameConst = Base.Query.PropertyValue(elem, "Construction.Name").ToString();
            string openNameConst = Base.Query.PropertyValue(elem, "OpeningConstruction.Name").ToString();
            string nameProp = Base.Query.PropertyValue(elem, "Property.Name").ToString();

            if (nameConst != null)
                return nameConst;
            else if (openNameConst != null)
                return openNameConst;
            else if (nameProp != null)
                return nameProp;
            else
                return "";
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static string PrimaryPropertyName(this IElement1D elem)
        {
            return "";
        }

        /***************************************************/
    }
}



