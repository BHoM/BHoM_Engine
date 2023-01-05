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
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the geometry of a ConcreteSection as its profile outlines and reinforcement in the global XY plane. Method required for automatic display in UI packages.")]
        [Input("section", "ConcreteSection to get outline and reinforcement geometry from.")]
        [Output("outlines", "The geometry of the ConcreteSection as its outline and reinforment curves in the global XY.")]
        public static CompositeGeometry Geometry(this ConcreteSection section)
        {
            if (section?.SectionProfile?.Edges == null)
                return null;

            if (section.SectionProfile.Edges.Count == 0)
                return null;

            CompositeGeometry geom = Engine.Geometry.Create.CompositeGeometry(section.SectionProfile.Edges);
            //if(section.Reinforcement != null)
            //geom.Elements.AddRange(section.Layout().Elements);

            return geom;
        }

        /***************************************************/

        [PreviousVersion("6.1", "BH.Engine.Structure.Query.Geometry(BH.oM.Structure.SectionProperties.SteelSection)")]
        [Description("Gets the geometry of a GeometricalSection as its profile outlines the global XY plane. Method required for automatic display in UI packages.")]
        [Input("section", "GeometricalSection to get outline geometry from.")]
        [Output("outlines", "The geometry of the GeometricalSection as its outline in the global XY plane.")]
        public static IGeometry Geometry(this IGeometricalSection section)
        {
            if (section?.SectionProfile?.Edges == null)
                return new CompositeGeometry();
            else
                return new CompositeGeometry { Elements = section.SectionProfile.Edges.ToList<IGeometry>() };
        }

        /***************************************************/

        [Description("Gets the geometry of a RigidLink as a list of lines between the primary node and the secondary nodes. Method required for automatic display in UI packages.")]
        [Input("link", "RigidLink to get the line geometry from.")]
        [Output("lines", "The geometry of the RigidLink as a list of primary-secondary lines.")]
        public static IGeometry Geometry(this RigidLink link)
        {
            List<IGeometry> lines = new List<IGeometry>();

            if (link?.SecondaryNodes != null && link?.PrimaryNode?.Position != null)
            {
                foreach (Node sn in link.SecondaryNodes)
                {
                    if (sn?.Position != null)
                        lines.Add(new Line() { Start = link.PrimaryNode.Position, End = sn.Position });
                }
            }

            return new CompositeGeometry() { Elements = lines };
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets the geometry of a SectionProperty, generally as its profile outlines the global XY plane. Method required for automatic display in UI packages.")]
        [Input("section", "SectionProperty to get outline geometry from.")]
        [Output("outlines", "The geometry of the SectionProperty.")]
        public static IGeometry IGeometry(this ISectionProperty section)
        {
            return section.IsNull() ? null : Geometry(section as dynamic);
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static IGeometry Geometry(this object section)
        {
            return null;
        }

        /***************************************************/
    }

}



