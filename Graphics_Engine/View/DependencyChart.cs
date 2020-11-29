using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Graphics;
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
        public static void View(this DependencyChart chart, List<IBHoMObject> data)
        {
            BHoMGroup<IBHoMObject> entityGroup = (BHoMGroup<IBHoMObject>)data.Find(x => x.Name == "Entities");
            List<IBHoMObject> entities = entityGroup.Elements;



            //set scales
            List<object> viewX = new List<object>() { 0, chart.ViewConfig.Width };
            List<object> viewY = new List<object>() { 0, chart.ViewConfig.Height };
            object allGroupNames = entities.PropertyValue(chart.Boxes.Group);
            var groups = Query.GetDataList(allGroupNames).GroupBy(n => n);
            int maxGroup = groups.Max(g => g.Count());
            List<object> colourNames = Graphics.Query.GetDataList(entities.PropertyValue(chart.Boxes.Colour));
            IScale xScale = null;
            IScale yScale = null;
            IScale colourScale = null;

            if (chart.Boxes.IsHorizontal)
            {
                //group for x scale
                xScale = Graphics.Create.IScale(new List<object>() { 0, maxGroup }, viewX);
                yScale = Graphics.Create.IScale(groups.Select(g => g.Key).ToList(), viewY);
            }
            else
            {
                //group for y scale
                xScale = Graphics.Create.IScale(groups.Select(g => g.Key).ToList(), viewX);
                yScale = Graphics.Create.IScale(new List<object>() { 0, maxGroup }, viewY);
            }
            Gradient gradient = Graphics.Create.Gradient();
            colourScale = Graphics.Create.IScale(colourNames, gradient.Markers.Values.Cast<object>().ToList());
            colourScale.Name = "colourScale";
            xScale.Name = "xScale";
            yScale.Name = "yScale";

            List<IScale> scales = new List<IScale>() { xScale, yScale, colourScale };

            chart.Boxes.IRepresentationFragment(data, chart.ViewConfig, scales);

            chart.Links.IRepresentationFragment(data, chart.ViewConfig, scales);
        }
       
    }
}
