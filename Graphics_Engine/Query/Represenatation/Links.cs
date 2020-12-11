using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Graphics.Scales;
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Geometry;
using BH.oM.Graphics;
using BH.oM.Graphics.Components;
using BH.oM.Graphics.Scales;
using BH.oM.Graphics.Views;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Create link representations.")]
        [Input("component", "The configuration properties for the link representation.")]
        [Input("dataset", "Dataset of a BH.oM.Analytical.Elements.Graph where Graph.Entities are one element of type BHoMGroup in Dataset.Data and Graph.Relations are another element of type BHoMGroup in Dataset.Data.")]
        [Input("viewConfig", "The configuration properties for the view.")]
        [Output("representation", "Collection of IRepresentation objects.")]
        private static List<IRepresentation> Component(this Links component, Dataset dataset, ViewConfig viewConfig)
        {
            List<IRepresentation> representations = new List<IRepresentation>();

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
                GeometricRepresentation geoRep = new GeometricRepresentation();
                TextRepresentation textRep = new TextRepresentation();
                IBHoMObject start = entities.Find(ent => ent.BHoM_Guid.Equals(obj.PropertyValue(component.Start)));
                IBHoMObject end = entities.Find(ent => ent.BHoM_Guid.Equals(obj.PropertyValue(component.End)));

                GeometricRepresentation startRep = m_EntityRepresentation[start.BHoM_Guid] as GeometricRepresentation;
                GeometricRepresentation endRep = m_EntityRepresentation[end.BHoM_Guid] as GeometricRepresentation;

                
                Line line = Geometry.Create.Line((startRep.Geometry as ICurve).IPointAtParameter(0.125), (endRep.Geometry as ICurve).IPointAtParameter(0.825));

                List<IGeometry> geometries = new List<IGeometry>();
                geometries.Add(line);

                geometries.AddRange(component.Marker.IMarker(line.End , line.Direction()));
                geoRep.Geometry = new CompositeGeometry() { Elements = geometries };
                textRep.Cartesian = Geometry.Create.CartesianCoordinateSystem(line.IPointAtLength(line.Length() / 3), line.Direction(), Geometry.Query.CrossProduct(Vector.ZAxis, line.Direction()));
                textRep.FontConfig = component.FontConfig;

                if (component.Text.Contains("Source."))
                    textRep.Text = start.PropertyValue(component.Text.Replace("Source.", "")).ToString();
                else if (component.Text.Contains("Target."))
                    textRep.Text = end.PropertyValue(component.Text.Replace("Target.", "")).ToString();
                else
                    textRep.Text = obj.PropertyValue(component.Text).ToString();
                

                if (component.Colour.Contains("Source."))
                    geoRep.Colour = Convert.ColourFromObject(colourScale.IScale(start.PropertyValue(component.Colour.Replace("Source.", ""))));
                else if (component.Colour.Contains("Target."))
                    geoRep.Colour = Convert.ColourFromObject(colourScale.IScale(end.PropertyValue(component.Colour.Replace("Target.", ""))));
                else
                    geoRep.Colour = Convert.ColourFromObject(colourScale.IScale(obj.PropertyValue(component.Colour)));

                //obj.Fragments.AddOrReplace(representation);
                representations.Add(geoRep);
                representations.Add(textRep);
            }

            return representations;
        }

    }

}
