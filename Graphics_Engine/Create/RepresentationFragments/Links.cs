using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Graphics.Scales;
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Graphics.Components;
using BH.oM.Graphics.Fragments;
using BH.oM.Graphics.Scales;
using BH.oM.Graphics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        private static void RepresentationFragment(this Links component, List<IBHoMObject> data, ViewConfig viewConfig, List<IScale> scales)
        {
            BHoMGroup<IBHoMObject> entityGroup = (BHoMGroup<IBHoMObject>)data.Find(x => x.Name == "Entities");
            List<IBHoMObject> entities = entityGroup.Elements;

            BHoMGroup<IBHoMObject> relationGroup = (BHoMGroup<IBHoMObject>)data.Find(x => x.Name == "Relations");
            List<IBHoMObject> relations = relationGroup.Elements;

            foreach(IBHoMObject obj in relations)
            {
                IBHoMObject start = entities.Find(ent => ent.BHoM_Guid.Equals(obj.PropertyValue(component.Start)));
                IBHoMObject end = entities.Find(ent => ent.BHoM_Guid.Equals(obj.PropertyValue(component.End)));

                EntityRepresentation startFrag = start.FindFragment<EntityRepresentation>();
                EntityRepresentation endFrag = end.FindFragment<EntityRepresentation>();

                RelationRepresentation representation = new RelationRepresentation();
                Line line = Geometry.Create.Line(startFrag.OutgoingRelationPoint, endFrag.IncomingRelationPoint);

                representation.Curves.Add(line);

                
                representation.Curves.AddRange(component.Marker.IArrowMarker(line.End , line.Direction()));
                representation.TextDirection = line.Direction();
                representation.TextPosition = line.IPointAtLength(line.Length() / 3);

                if (component.Text.Contains("Source"))
                    representation.Text = start.PropertyValue(component.Text.Replace("Source.", "")).ToString();
                else if (component.Text.Contains("Target"))
                    representation.Text = end.PropertyValue(component.Text.Replace("Target.", "")).ToString();
                else
                    representation.Text = obj.PropertyValue(component.Text).ToString();
                

                if (component.Colour.Contains("Source"))
                    representation.Colour = Query.ColourFromObject(m_Colourscale.IScale(start.PropertyValue(component.Colour.Replace("Source.", ""))));
                else if (component.Colour.Contains("Target"))
                    representation.Colour = Query.ColourFromObject(m_Colourscale.IScale(end.PropertyValue(component.Colour.Replace("Target.", ""))));
                else
                    representation.Colour = Query.ColourFromObject(m_Colourscale.IScale(obj.PropertyValue(component.Colour)));

                obj.Fragments.AddOrReplace(representation);
            }
        }

    }

}
