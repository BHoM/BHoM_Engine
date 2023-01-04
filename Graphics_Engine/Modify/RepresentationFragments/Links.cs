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

using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Graphics.Scales;
using BH.Engine.Reflection;
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

        [Description("Adds links representation fragments to IBHoMObjects.")]
        [Input("component", "The configuration properties for the link representation.")]
        [Input("dataset", "Dataset of a BH.oM.Analytical.Elements.Graph where Graph.Entities are one element of type BHoMGroup in Dataset.Data and Graph.Relations are another element of type BHoMGroup in Dataset.Data.")]
        [Input("viewConfig", "The configuration properties for the view.")]
        private static void RepresentationFragment(this Links component, Dataset dataset, ViewConfig viewConfig)
        {
            BHoMGroup<IBHoMObject> entityGroup = (BHoMGroup<IBHoMObject>)dataset.Data.Find(x => x.Name == "Entities");
            List<IBHoMObject> entities = entityGroup.Elements;

            BHoMGroup<IBHoMObject> relationGroup = (BHoMGroup<IBHoMObject>)dataset.Data.Find(x => x.Name == "Relations");
            List<IBHoMObject> relations = relationGroup.Elements;

            Gradient gradient = Create.Gradient();

            List<object> colourNames = new List<object>();
            if (component.Colour.Contains("Source."))
                colourNames = Convert.ToDataList(entities.PropertyValue(component.Colour.Replace("Source.", "")));
            if (component.Colour.Contains("Target."))
                colourNames = Convert.ToDataList(entities.PropertyValue(component.Colour.Replace("Target.", "")));
            else
                colourNames = Convert.ToDataList(relations.PropertyValue(component.Colour));

            IScale colourScale = Create.IScale(colourNames, gradient.Markers.Values.Cast<object>().ToList());

            foreach (IBHoMObject obj in relations)
            {
                IBHoMObject start = entities.Find(ent => ent.BHoM_Guid.Equals(obj.PropertyValue(component.Start)));
                IBHoMObject end = entities.Find(ent => ent.BHoM_Guid.Equals(obj.PropertyValue(component.End)));

                EntityRepresentation startFrag = start.FindFragment<EntityRepresentation>();
                EntityRepresentation endFrag = end.FindFragment<EntityRepresentation>();

                RelationRepresentation representation = new RelationRepresentation();
                Line line = Geometry.Create.Line(startFrag.OutgoingRelationPoint, endFrag.IncomingRelationPoint);

                representation.Curves.Add(line);

                representation.Curves.AddRange(component.Marker.IMarker(line.End , line.Direction()));
                representation.TextDirection = line.Direction();
                representation.TextPosition = line.IPointAtLength(line.Length() / 3);

                if (component.Text.Contains("Source."))
                    representation.Text = start.PropertyValue(component.Text.Replace("Source.", "")).ToString();
                else if (component.Text.Contains("Target."))
                    representation.Text = end.PropertyValue(component.Text.Replace("Target.", "")).ToString();
                else
                    representation.Text = obj.PropertyValue(component.Text).ToString();
                

                if (component.Colour.Contains("Source."))
                    representation.Colour = Convert.ColourFromObject(colourScale.IScale(start.PropertyValue(component.Colour.Replace("Source.", ""))));
                else if (component.Colour.Contains("Target."))
                    representation.Colour = Convert.ColourFromObject(colourScale.IScale(end.PropertyValue(component.Colour.Replace("Target.", ""))));
                else
                    representation.Colour = Convert.ColourFromObject(colourScale.IScale(obj.PropertyValue(component.Colour)));

                obj.Fragments.AddOrReplace(representation);
            }
        }

    }

}


