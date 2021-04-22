using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Assign space type to a SAP space. ")]
        [Input("spaces", "A space object is an analytical depiction of a room defined by its environmental conditions(internal gains)")]
        [Input("searchPoints", "A set of points inside the spaces")]
        [Input("type", "The list of roomtypes to assign")]
        [Input("ignoreCase", "")] // Whether or not the parse will be casesensitive?
        [Output("spaces", "")]
    
        public static List<Space> AssignSpaceType(this List<Space> spaces, List<Point> searchPoints, string type, bool ignoreCase = true)
        {
            List<Space> updateSpaces = new List<Space>();
            foreach (Space s in spaces)
                updateSpaces.Add(s.DeepClone());

            SpaceType spaceType = SpaceType.Undefined;
            object value = Enum.Parse(typeof(SpaceType), type, ignoreCase);
            if (value != null)
                spaceType = (SpaceType)value;

            List<Space> returnSpaces = new List<Space>();
            for (int x = 0; x < searchPoints.Count; x++)
            {
                Space update = updateSpaces.Where(a => a.Perimeter.IIsContaining(new List<Point> { searchPoints[x] })).FirstOrDefault();
                if (update == null)
                    continue;

                update.SpaceType = spaceType;

                returnSpaces.Add(update);
            }

            return returnSpaces;
        }
    }
}