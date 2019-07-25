/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Physical;

using BH.Engine.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the External Polyline representation of a physical object (e.g. wall or window)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> ExternalPolyline(IPhysical physicalObject)
        {
            return ExternalPolyline(physicalObject as dynamic);
        }

        [Description("Returns the External Polyline representation of a physical object that represents a solid impassable object (e.g. wall or roof)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> ExternalPolyline(BH.oM.Physical.Elements.ISurface physicalObject)
        {
            return physicalObject.Location.IExternalEdges().Select(x => x.ICollapseToPolyline(Tolerance.Angle)).ToList();
        }

        [Description("Returns the External Polyline representation of a physical object that represents an opening (e.g. window or door)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> ExternalPolyline(IOpening physicalOpening)
        {
            return physicalOpening.Location.IExternalEdges().Select(x => x.ICollapseToPolyline(Tolerance.Angle)).ToList();
        }

        [Description("Returns the Internal Polyline representation of a physical object (e.g. wall or window)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> InternalPolyline(IPhysical physicalObject)
        {
            return InternalPolyline(physicalObject as dynamic);
        }

        [Description("Returns the Internal Polyline representation of a physical object that represents a solid impassable object (e.g. wall or roof)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> InternalPolyline(BH.oM.Physical.Elements.ISurface physicalObject)
        {
            return physicalObject.Location.IInternalEdges().Select(x => x.ICollapseToPolyline(Tolerance.Angle)).ToList();
        }

        [Description("Returns the Internal Polyline representation of a physical object that represents an opening (e.g. window or door)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> InternalPolyline(IOpening physicalOpening)
        {
            return physicalOpening.Location.IInternalEdges().Select(x => x.ICollapseToPolyline(Tolerance.Angle)).ToList();
        }

        [Description("Returns a Polyline representation of a physical object (e.g. wall or window)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> Polyline(IPhysical physicalObject)
        {
            return Polyline(physicalObject as dynamic);
        }

        [Description("Returns a Polyline representation of a physical object that represents a solid impassable object (e.g. wall or roof)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> Polyline(BH.oM.Physical.Elements.ISurface physicalObject)
        {
            return physicalObject.Location.Edges().Select(x => x.ICollapseToPolyline(Tolerance.Angle)).ToList();
        }

        [Description("Returns a Polyline representation of a physical object that represents an opening (e.g. window or door)")]
        [Input("physicalObject", "A physical object to query the polyline representation of")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static List<Polyline> Polyline(IOpening physicalOpening)
        {
            return physicalOpening.Location.Edges().Select(x => x.ICollapseToPolyline(Tolerance.Angle)).ToList();
        }
    }
}
