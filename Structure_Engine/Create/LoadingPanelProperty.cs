using System.Collections.Generic;
using System.Collections.ObjectModel;
using BH.oM.Structure.Properties;
using BH.oM.Geometry;
using BH.oM.Common.Materials;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LoadingPanelProperty LoadingPanelProperty(LoadPanelSupportConditions supportCondition = LoadPanelSupportConditions.AllSides, int referenceEdge = 1)
        {
            return new LoadingPanelProperty()
            {
                ReferenceEdge = referenceEdge,
                LoadApplication = supportCondition,
            };
        }

        /***************************************************/
    }
}
