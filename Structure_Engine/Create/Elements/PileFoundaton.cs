using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural PileFoundation from a pile cap and pile groups.")]
        [Input("pileCap", "The pile cap defining the outer edge of the PileFoundation and location in 3D.")]
        [Input("coordinates", "Groups of piles contained within the outline of the pile cap.")]
        public static PileFoundation PileFoundation(PadFoundation pileCap, List<PileGroup> pileGroups)
        {
            return new PileFoundation() { PileCap = pileCap, PileGroups = pileGroups };
        }

        /***************************************************/

    }
}