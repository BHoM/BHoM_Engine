using BH.oM.Structural.Design;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Span Span(StructuralLayout elem, double effectiveLength = 0)
        {
            Span span = new Span { EffectiveLength = effectiveLength };

            for (int i = 0; i < elem.AnalyticBars.Count; i++)
                span.BarIndices.Add(i);

            return span;
        }

        /***************************************************/
    }
}
