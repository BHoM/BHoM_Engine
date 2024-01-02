/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.Engine.Graphics.Scales;
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Geometry;
using BH.oM.Graphics;
using BH.oM.Graphics.Components;
using BH.oM.Graphics.Fragments;
using BH.oM.Graphics.Scales;
using BH.oM.Graphics.Views;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Adds box representation fragments to IBHoMObjects.")]
        [Input("component", "The configuration properties for the box representation.")]
        [Input("dataset", "Dataset of a BH.oM.Analytical.Elements.Graph where Graph.Entities are one element of type BHoMGroup in Dataset.Data.")]
        [Input("viewConfig", "The configuration properties for the view.")]
        public static void RepresentationFragment(this Boxes component, Dataset dataset, ViewConfig viewConfig)
        {
            if(component == null || dataset == null || viewConfig == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot add box representation fragments if the components, datasets, or view configurations are null.");
                return;
            }

            SetScales(component, dataset, viewConfig);

            BHoMGroup<IBHoMObject> entityGroup = (BHoMGroup<IBHoMObject>)dataset.Data.Find(x => x.Name == "Entities");
            List<IBHoMObject> entities = entityGroup.Elements;

            var groups = entities.GroupBy(d => d.PropertyValue(component.Group));
            var groupNames = groups.Select(g => g.Key).Cast<string>().ToList();
            int maxGroup = groups.Max(g => g.Count());

            double xSpace = 0; 
            double ySpace = 0;
            
            if (component.IsHorizontal)
            {
                xSpace = viewConfig.Width / maxGroup;
                ySpace = (viewConfig.Height - (groupNames.Count * component.Padding)) / groupNames.Count;
            }
            else
            {
                xSpace = (viewConfig.Width - (groupNames.Count * component.Padding))/ groupNames.Count;
                ySpace = viewConfig.Height / maxGroup;
            }
            List<GroupRepresentation> groupRepresentations = new List<GroupRepresentation>();
            GraphRepresentation graphRepresentation = new GraphRepresentation();
            foreach (var group in groups)
            {
                int i = 0;
                var orderedgroup = group.OrderBy(g => g.PropertyValue(component.GroupOrder));
                double x = 0;
                double y = 0;
                GroupRepresentation representation = new GroupRepresentation();
                if (component.IsHorizontal)
                {
                    x = System.Convert.ToDouble(m_Xscale.IScale(0));
                    y = System.Convert.ToDouble(m_Yscale.IScale(group.Key));
                    representation.Boundary = Box(Geometry.Create.Point(x, y, 0), xSpace * orderedgroup.Count(), ySpace );
                    representation.TextPosition = SetAnchorPoint(Geometry.Create.Point(x, y, 0), -component.Padding, 0, 0);
                    representation.TextDirection = Vector.YAxis;
                }
                else
                {
                    x = System.Convert.ToDouble(m_Xscale.IScale(group.Key));
                    y = System.Convert.ToDouble(m_Yscale.IScale(0));
                    representation.Boundary = Box(Geometry.Create.Point(x, y, 0), xSpace, ySpace * orderedgroup.Count());
                    representation.TextPosition = SetAnchorPoint(Geometry.Create.Point(x, y, 0), 0, -viewConfig.Padding.Bottom, 0);
                    representation.TextDirection = Vector.XAxis;
                }
                representation.Colour = Convert.ColourFromObject(m_Colourscale.IScale(group.Key));
                representation.Text = group.Key.ToString();
                
                groupRepresentations.Add(representation);
                foreach (var obj in orderedgroup)
                {
                    obj.SetEntityRepresentation(i, component, xSpace, ySpace);
                    i++;
                }
                
            }
            graphRepresentation.Groups = groupRepresentations;
            dataset.Fragments.AddOrReplace(graphRepresentation);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void SetEntityRepresentation(this IBHoMObject obj,int seqNumber, Boxes component, double xSpace, double ySpace)
        {
            double x = 0;
            double y = 0;
            double cellX = xSpace - 2 * component.Padding;
            double cellY = ySpace - 2 * component.Padding;

            if (component.IsHorizontal)
            {
                x = System.Convert.ToDouble(m_Xscale.IScale(seqNumber));
                y = System.Convert.ToDouble(m_Yscale.IScale(obj.PropertyValue(component.Group)));
            }
            else
            {
                x = System.Convert.ToDouble(m_Xscale.IScale(obj.PropertyValue(component.Group)));
                y = System.Convert.ToDouble(m_Yscale.IScale(seqNumber));
            }

            EntityRepresentation representation = new EntityRepresentation();

            Point basePt = SetAnchorPoint(Geometry.Create.Point(x, y, 0), component.Padding, component.Padding, 0);

            representation.Boundary = Box(basePt, cellX, cellY);
            representation.Text = obj.PropertyValue(component.Text).ToString();
            representation.TextPosition = SetAnchorPoint(basePt, component.Padding, component.Padding, 0);
            representation.OutgoingRelationPoint = SetAnchorPoint(basePt, cellX, cellY / 2, 0);
            representation.IncomingRelationPoint = SetAnchorPoint(basePt, 0, cellY / 2, 0);
            representation.Colour = Convert.ColourFromObject(m_Colourscale.IScale(obj.PropertyValue(component.Colour)));
            obj.Fragments.AddOrReplace(representation);
        }

        /***************************************************/

        private static Polyline Box(Point basePt, double x, double y)
        {
            List<Point> points = new List<Point>();
            points.Add(basePt);
            points.Add(basePt + Vector.XAxis * x);
            points.Add(basePt + Vector.XAxis * x + Vector.YAxis * y);
            points.Add(basePt + Vector.YAxis * y);
            points.Add(basePt);
            return Geometry.Create.Polyline(points);
        }

        /***************************************************/

        private static Point SetAnchorPoint(Point basePt, double x, double y, double z)
        {
            return basePt + Vector.XAxis * x  + Vector.YAxis * y + Vector.ZAxis * z;
        }

        /***************************************************/

        private static void SetScales(this Boxes component, Dataset dataset, ViewConfig viewConfig)
        {
            BHoMGroup<IBHoMObject> entityGroup = (BHoMGroup<IBHoMObject>)dataset.Data.Find(x => x.Name == "Entities");
            List<IBHoMObject> entities = entityGroup.Elements;

            //set scales
            List<object> viewX = new List<object>() { 0, viewConfig.Width };
            List<object> viewY = new List<object>() { 0, viewConfig.Height };
            object allGroupNames = entities.PropertyValue(component.Group);
            var groups = Convert.ToDataList(allGroupNames).GroupBy(n => n);

            if (groups.Count() == 0)
            {
                Base.Compute.RecordError("No groups could be defined with the supplied property name: " + component.Group + ".");
                return;
            }

            int maxGroup = groups.Max(g => g.Count());
            List<object> colourNames = Graphics.Convert.ToDataList(entities.PropertyValue(component.Colour));

            if (component.IsHorizontal)
            {
                //group for x scale
                m_Xscale = Create.IScale(new List<object>() { 0, maxGroup }, viewX);
                m_Yscale = Create.IScale(groups.Select(g => g.Key).ToList(), viewY);
            }
            else
            {
                //group for y scale
                m_Xscale = Create.IScale(groups.Select(g => g.Key).ToList(), viewX);
                m_Yscale = Create.IScale(new List<object>() { 0, maxGroup }, viewY);
            }

            Gradient gradient = Create.Gradient();
            m_Colourscale = Create.IScale(colourNames, gradient.Markers.Values.Cast<object>().ToList());

        }

        /***************************************************/

        private static IScale m_Xscale = null;
        private static IScale m_Yscale = null;
        private static IScale m_Colourscale = null;
    }
}



