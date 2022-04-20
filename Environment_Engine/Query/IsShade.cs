using System;
using System.Collections.Generic;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Something smart")]
        [Input("panelType", "panel again")]
        [Output("Same with panel", "when does this end??")]

        public static bool IsShade(this PanelType panelType)
        {
            List<PanelType> shadeTypes = new List<PanelType>() {
                PanelType.Shade,
                PanelType.TranslucentShade,
            };

            return shadeTypes.Contains(panelType);
        }

        [Description("something")]
        [Input("panel", "panelpanel")]
        [Output("some", "results")]

        public static bool IsShade(this Panel panel)
        {
            return panel.Type.IsShade();
        }

    }
}
