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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates physical wall based on given construction, bottom curve and height")]
        [Input("construction", "Construction of the wall")]
        [Input("bottomEdge", "Curve representing bottom edge of the wall")]
        [Input("height", "Wall height")]
        [Output("wall", "A physical wall")]
        public static Wall Wall(IConstruction construction, ICurve bottomEdge, double height)
        {
            if (construction == null || bottomEdge == null || height <= 0)
            {
                Reflection.Compute.RecordError("Physical Wall could not be created because some input data are null");
                return null;
            }

            if (Geometry.Query.IIsClosed(bottomEdge))
            {
                Reflection.Compute.RecordError("Physical Wall could not be created because bottom edge cannot be closed curve");
                return null;
            }

            Point aPoint_1 = Geometry.Query.IStartPoint(bottomEdge);
            Point aPoint_2 = Geometry.Query.IEndPoint(bottomEdge);

            ICurve aICurve = Geometry.Modify.ITranslate(bottomEdge, Geometry.Create.Vector(0, 0, height));

            Line aLine_1 = Geometry.Create.Line(Geometry.Query.IEndPoint(bottomEdge), Geometry.Query.IStartPoint(aICurve));
            Line aLine_2 = Geometry.Create.Line(Geometry.Query.IEndPoint(aICurve), Geometry.Query.IStartPoint(bottomEdge));

            PolyCurve aPolyCurve = Geometry.Create.PolyCurve(new ICurve[] { bottomEdge, aLine_1, aICurve, aLine_2 });

            return new Wall()
            {
                Construction = construction,
                Location = Geometry.Create.PlanarSurface(aPolyCurve)
            };
        }

        [Description("Creates a physical Wall element. For elements for structral analytical applications look at BH.oM.Structure.Elements.Panel. For elements for environmental analytical applications look at BH.oM.Environments.Elements.Panel")]
        [Input("line", "Base line of the wall")]
        [Input("height", "Height of the wall")]
        [Input("construction", "Construction representing the thickness and materiality of the Wall")]
        [Input("offset", "Represents the positioning of the construction in relation to the location surface of the Wall")]
        [Input("name", "The name of the wall, default empty string")]
        [Output("Wall", "The created physical Wall")]
        public static Wall Wall(Line line, double height, IConstruction construction, Offset offset = Offset.Undefined, string name = "")
        {
            Polyline boundary = new Polyline();

            Vector move = Vector.ZAxis * height;

            boundary.ControlPoints.Add(line.Start);
            boundary.ControlPoints.Add(line.End);
            boundary.ControlPoints.Add(line.End + move);
            boundary.ControlPoints.Add(line.Start + move);
            boundary.ControlPoints.Add(line.Start);

            return new Wall
            {
                Location = Geometry.Create.PlanarSurface(boundary),
                Construction = construction,
                Offset = offset,
                Name = name
            };
        }

        /***************************************************/

        [Description("Creates physical wall object")]
        [Input("construction", "Construction of the wall")]
        [Input("edges", "External edges of the wall (Profile - planar closed curve)")]
        [Input("internalEdges", "Internal edges of wall (profile)")]
        [Output("wall", "A physical wall")]
        public static Wall Wall(IConstruction construction, ICurve edges, IEnumerable<ICurve> internalEdges)
        {
            if (construction == null || edges == null)
            {
                Reflection.Compute.RecordError("Physical Wall could not be created because some input data are null");
                return null;
            }

            PlanarSurface aPlanarSurface = Geometry.Create.PlanarSurface(edges);
            if (aPlanarSurface == null)
            {
                Reflection.Compute.RecordError("Physical Wall could not be created because invalid geometry of edges");
                return null;
            }

            return new Wall()
            {
                Construction = construction,
                Location = aPlanarSurface
            };
        }

        [Description("Creates a physical Wall element. For elements for structral analytical applications look at BH.oM.Structure.Elements.Panel. For elements for environmental analytical applications look at BH.oM.Environments.Elements.Panel")]
        [Input("location", "Location surface which represents the outer geometry of the Wall. Should not contain any openings")]
        [Input("construction", "Construction representing the thickness and materiality of the Wall")]
        [Input("openings", "Openings of the Wall. Could be simple voids or more detailed obejcts")]
        [Input("offset", "Represents the positioning of the construction in relation to the location surface of the Wall")]
        [Input("name", "The name of the wall, default empty string")]
        [Output("Wall", "The created physical Wall")]
        public static Wall Wall(oM.Geometry.ISurface location, IConstruction construction, List<IOpening> openings = null, Offset offset = Offset.Undefined, string name = "")
        {
            openings = openings ?? new List<IOpening>();

            return new Wall
            {
                Location = location,
                Construction = construction,
                Openings = openings,
                Offset = offset,
                Name = name
            };
        }

        /***************************************************/

        [Description("Creates physical wall object")]
        [Input("construction", "Construction of the wall")]
        [Input("edges", "External edges of the wall (Profile - planar closed curve)")]
        [Output("wall", "A physical wall")]
        public static Wall Wall(IConstruction construction, ICurve edges)
        {
            return Wall(construction, edges, null);
        }

        /***************************************************/
    }
}

