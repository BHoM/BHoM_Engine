using System;
using System.Collections.Generic;
using System.Text;

using BH.oM.Environment.Elements;
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Base;
using BH.oM.Spatial.Layouts;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Checks if the space is valid by checking if the perimeter curve is closed, if the perimeter length is not equal 0 and if the space is not self intersecting.")]
        [Input("spaces", "The Space to check if it is valid.")]
        [MultiOutput(0,"validSpaces", "Returns list of valid spaces")]
        [MultiOutput(1, "selfIntersectingSpaces", "Returns list of invalid spaces due to self intersection")]
        [MultiOutput(2, "zeroPerimeterSpaces", "Returns list of invalid spaces due to perimeter length being zero")]
        [MultiOutput(3, "notClosedSpaces", "Returns list of invalid spaces due to not closed perimeter")]
        public static Output<List<Space>, List<Space>, List<Space>, List<Space>> IsValid(this List<Space> spaces)
        {
            List<Space> validSpaces = new List<Space>();
            List<Space> selfIntersectingSpaces = new List<Space>();
            List<Space> zeroPerimeterSpaces = new List<Space>();
            List<Space> notClosedSpaces = new List<Space>();

            foreach (Space space in spaces)
            {
                bool isvalid = true;

                if (space.IsSelfIntersecting())
                {
                    selfIntersectingSpaces.Add(space);
                    isvalid = false;
                }

                if (space.Perimeter.Length() == 0)
                {
                    zeroPerimeterSpaces.Add(space);
                    isvalid = false;
                }

                if (!space.Perimeter.IIsClosed())
                {
                    notClosedSpaces.Add(space);
                    isvalid = false;
                }
                
                if (isvalid)
                {
                    validSpaces.Add(space);
                }
            }

            return new Output<List<Space>, List<Space>, List<Space>, List<Space>>
            {

                Item1 = validSpaces,

                Item2 = selfIntersectingSpaces,

                Item3 = notClosedSpaces,

                Item4 = zeroPerimeterSpaces

            };
        }
    }
}
