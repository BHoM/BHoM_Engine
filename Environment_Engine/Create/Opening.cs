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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Environment.Configuration;
using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Description("Create an opening at a given location from the provided configuration options.")]
        [Input("location", "The point in 3D space where the opening should begin to be generated. The point should match to a Panel that will be used to identify the host panel.")]
        [Input("configurationOption", "The Configuration Options the Opening should be generated against that define the height, width, and sill height of the Opening.")]
        [Input("panels", "A collection of Environment Panels which will be used to identify the host panel for the opening from the provided location point.")]
        [Output("opening", "An Opening generated with the provided Configuration Options.")]
        public static Opening Opening(Point location, OpeningOption configurationOption, List<Panel> panels, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if(location == null)
            {
                BH.Engine.Base.Compute.RecordError("Location point to build Opening from was null.");
                return null;
            }

            if(configurationOption == null)
            {
                BH.Engine.Base.Compute.RecordError("Configuration Options cannot be null to build openings on.");
                return null;
            }

            if(panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Panels cannot be null, panels are necessary to define the orientation of the openings being built from configurations.");
                return null;
            }

            Point searchPoint = location.DeepClone();
            searchPoint.Z += configurationOption.SillHeight;
            searchPoint.Z += configurationOption.Height / 2;

            Panel hostPanel = panels.Where(x => x.IsContaining(searchPoint, tolerance: tolerance)).FirstOrDefault();
            if (hostPanel == null)
                return null; //Error

            Point bottomPoint = location.DeepClone();
            bottomPoint.Z += configurationOption.SillHeight;

            ICurve bottomEdge = hostPanel.Bottom();
            Vector direction = bottomEdge.IEndPoint() - bottomEdge.IStartPoint();

            direction = direction.Normalise();
            direction *= (configurationOption.Width / 2);

            Point bottomCorner1 = bottomPoint.DeepClone().Translate(direction);
            Point bottomCorner2 = bottomPoint.DeepClone().Translate(-direction);

            Point topCorner1 = bottomCorner1.DeepClone();
            topCorner1.Z += configurationOption.Height;

            Point topCorner2 = bottomCorner2.DeepClone();
            topCorner2.Z += configurationOption.Height;

            List<Point> controlPoints = new List<Point>()
            {
                topCorner1,
                topCorner2,
                bottomCorner2,
                bottomCorner1,
                topCorner1,
            };

            Polyline curve = new Polyline()
            {
                ControlPoints = controlPoints,
            };

            return new Opening()
            {
                Edges = curve.ToEdges(),
                Name = configurationOption.Name,
                Type = configurationOption.Type,
            };
        }
    }
}




