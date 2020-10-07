using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using System.ComponentModel.Design;

using BH.oM.Architecture.Elements;
using BH.Engine.Geometry;



namespace BH.Engine.Architecture
{
    public static partial class Modify
    {
        [Description("Modifies BHoM Geometry Room vertices to be rounded to the number of provided decimal places")]
        [Input("room", "The BHoM Geometry Room to modify")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6")]
        [Output("room", "The modified BHoM Geometry Room")]
        public static Room RoundingVertices(Room room, int decimalPlaces = 6)
        {
            room.Perimeter = room.Perimeter.IRoundCoordinates();

            /* IElement2D  geometry*/

            return room;
        }
    }
}