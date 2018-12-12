using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties.Framing;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static FramingElement FramingElement(Line locationCurve, IFramingElementProperty property, StructuralUsage1D structuralUsage= StructuralUsage1D.Beam, string name = "")
        {
            return new FramingElement { LocationCurve = locationCurve, Property = property, StructuralUsage = structuralUsage, Name = name };
        }

        /***************************************************/
    }
}
