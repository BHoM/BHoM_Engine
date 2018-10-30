using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;

namespace BH.Engine.Structure
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static FramingElement ToFramingElement(this Bar bar, StructuralUsage1D usage = StructuralUsage1D.Beam)
        {
            return new FramingElement
            {
                LocationCurve = bar.Centreline(),
                Name = bar.Name,
                Property = Create.ConstantFramingElementProperty(bar.SectionProperty, bar.OrientationAngle, bar.SectionProperty.Name),
                StructuralUsage = usage
            };
        }


    }
}
