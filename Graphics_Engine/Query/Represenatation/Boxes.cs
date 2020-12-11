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
        [Description("Create box representation.")]
        [Input("component", "The configuration properties for the box representation.")]
        [Input("dataset", "Dataset of a BH.oM.Analytical.Elements.Graph where Graph.Entities are one element of type BHoMGroup in Dataset.Data.")]
        [Input("viewConfig", "The configuration properties for the view.")]
        [Output("representation", "Collection of IRepresentation objects.")]

        public static List<IRepresentation> Component(this Boxes component, Dataset dataset, ViewConfig viewConfig)
        {
            List<IRepresentation> representations = new List<IRepresentation>();
            SetScales(component,dataset, viewConfig);
            m_EntityRepresentation = new Dictionary<Guid, IRepresentation>();

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

            foreach (var group in groups)
            {
                GeometricRepresentation geoRep = new GeometricRepresentation();
                TextRepresentation textRep = new TextRepresentation();
                int i = 0;
                var orderedgroup = group.OrderBy(g => g.PropertyValue(component.GroupOrder));
                double x = 0;
                double y = 0;
                Vector textX = new Vector();
                Point textorigin = new Point();
                if (component.IsHorizontal)
                {
                    x = System.Convert.ToDouble(m_Xscale.IScale(0));
                    y = System.Convert.ToDouble(m_Yscale.IScale(group.Key));
                    geoRep.Geometry = Box(Geometry.Create.Point(x, y, 0), xSpace * orderedgroup.Count(), ySpace );
                    textorigin = SetAnchorPoint(Geometry.Create.Point(x, y, 0), -component.Padding, 0, 0);
                    textX = Vector.YAxis;
                    
                }
                else
                {
                    x = System.Convert.ToDouble(m_Xscale.IScale(group.Key));
                    y = System.Convert.ToDouble(m_Yscale.IScale(0));
                    geoRep.Geometry = Box(Geometry.Create.Point(x, y, 0), xSpace, ySpace * orderedgroup.Count());
                    textorigin = SetAnchorPoint(Geometry.Create.Point(x, y, 0), 0, -viewConfig.Padding.Bottom, 0);
                    textX = Vector.XAxis;
                }

                textRep.Cartesian = Geometry.Create.CartesianCoordinateSystem(textorigin, textX, Geometry.Query.CrossProduct(Vector.ZAxis, textX));

                geoRep.Colour = Convert.ColourFromObject(m_Colourscale.IScale(group.Key));
                
                representations.Add(geoRep);
                textRep.Text = group.Key.ToString();
                textRep.FontConfig = component.FontConfig;
                representations.Add(textRep);
                representations.Add(geoRep);

                foreach (var obj in orderedgroup)
                {
                    representations.AddRange(obj.SetEntityRepresentation(i, component, xSpace, ySpace));
                    i++;
                }
                
            }
            return representations;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<IRepresentation> SetEntityRepresentation(this IBHoMObject obj, int seqNumber, Boxes component, double xSpace, double ySpace)
        {
            double x = 0;
            double y = 0;
            double cellX = xSpace - 2 * component.Padding;
            double cellY = ySpace - 2 * component.Padding;
            GeometricRepresentation geoRep = new GeometricRepresentation();
            TextRepresentation textRep = new TextRepresentation();
            Vector textX = new Vector();
            if (component.IsHorizontal)
            {
                x = System.Convert.ToDouble(m_Xscale.IScale(seqNumber));
                y = System.Convert.ToDouble(m_Yscale.IScale(obj.PropertyValue(component.Group)));
                textX = Vector.YAxis;
            }
            else
            {
                x = System.Convert.ToDouble(m_Xscale.IScale(obj.PropertyValue(component.Group)));
                y = System.Convert.ToDouble(m_Yscale.IScale(seqNumber));
                textX = Vector.XAxis;
            }

            Point basePt = SetAnchorPoint(Geometry.Create.Point(x, y, 0), component.Padding, component.Padding, 0);

            geoRep.Geometry = Box(basePt, cellX, cellY);
            textRep.Text = obj.PropertyValue(component.Text).ToString();
            textRep.Cartesian = Geometry.Create.CartesianCoordinateSystem(basePt, textX, Geometry.Query.CrossProduct(Vector.ZAxis, textX));
            textRep.FontConfig = component.FontConfig;
            //representation.OutgoingRelationPoint = SetAnchorPoint(basePt, cellX, cellY / 2, 0);
            //representation.IncomingRelationPoint = SetAnchorPoint(basePt, 0, cellY / 2, 0);
            geoRep.Colour = Convert.ColourFromObject(m_Colourscale.IScale(obj.PropertyValue(component.Colour)));
            m_EntityRepresentation.Add(obj.BHoM_Guid, geoRep);
            return new List<IRepresentation>() { geoRep, textRep };
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
                Reflection.Compute.RecordError("No groups could be defined with the supplied property name: " + component.Group + ".");
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
        private static Dictionary<Guid, IRepresentation> m_EntityRepresentation = new Dictionary<Guid, IRepresentation>();
    }
}
