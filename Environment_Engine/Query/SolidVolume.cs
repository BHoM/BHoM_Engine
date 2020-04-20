using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;

using BH.Engine.Physical;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.Quantities.Attributes;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Returns a Panels solid volume based on its height, width, and construction thickness")]
        [Input("panel", "The Panel to get the volume from")]
        [Output("volume", "The Panel solid volume", typeof(Volume))]
        public static double SolidVolume(Panel panel)
        {
            return panel.Area() * panel.Construction.IThickness();
        }

        [Description("Returns an Openings solid volume based on its area, and construction thickness")]
        [Input("opening", "The Opening to get the volume from")]
        [Output("volume", "The Opening solid volume", typeof(Volume))]
        public static double SolidVolume(Opening opening)
        {
            double glazedVolume = 0;
            double frameVolume = 0;

            if (opening.InnerEdges != null)
            {
                double innerArea = opening.InnerEdges.Polyline().Area();
                glazedVolume = innerArea * opening.OpeningConstruction.IThickness();
                frameVolume = (opening.Polyline().Area() - innerArea) * opening.FrameConstruction.IThickness();
            }
            else
                glazedVolume = opening.Polyline().Area() * opening.OpeningConstruction.IThickness();

            return glazedVolume + frameVolume;
        }
    }
}
