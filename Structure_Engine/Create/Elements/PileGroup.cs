using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.Design;
using BH.Engine.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural PileGroup defining a group of piles with the same length and section. The object is used with a PileFoundation whereby the PileCap defines the location and orientation.")]
        [Input("pileLength", "The length of the piles within the group.")]
        [InputFromProperty("sectionProperty")]
        [Input("pileLayout", "Pile Layout defining the position of the piles about the World Origin.")]
        public static PileGroup PileGroup(double pileLength, ISectionProperty pileSection, ExplicitSection pileLayout )
        {
            return new PileGroup() { PileLength = pileLength, PileSection = pileSection, PileLayout = pileLayout};
        }

        /***************************************************/

    }
}