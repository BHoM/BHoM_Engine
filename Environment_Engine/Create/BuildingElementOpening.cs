using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHE = BH.oM.Environment.Elements;
using BHG = BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BHE.BuildingElementOpening BuildingElementOpening(BHG.ICurve curve)
        {
            return BuildingElementOpening(curve as dynamic);
        }

        /***************************************************/

        public static BHE.BuildingElementOpening BuildingElementOpening(BHG.PolyCurve pCurve)
        {
            return new BHE.BuildingElementOpening
            {
                Opening = pCurve
            };
        }

        /***************************************************/

        public static BHE.BuildingElementOpening BuildingElementOpening(IEnumerable<BHG.Polyline> pLines)
        {
            return new BHE.BuildingElementOpening
            {
                Opening = Geometry.Create.PolyCurve(pLines)
            };
        }

        /***************************************************/

        public static BHE.BuildingElementOpening BuildingElementOpening(BHG.Polyline pLine)
        {
            return new BHE.BuildingElementOpening
            {
                Opening = Geometry.Create.PolyCurve(new BHG.Polyline[] { pLine })
            };
        }

        /***************************************************/
        
        public static BHE.BuildingElement BuildingElementOpening(this BHE.BuildingElement be, BHG.ICurve bound)
        {
            return be.BuildingElementOpening(new List<BHG.ICurve> { bound });
        }

        /***************************************************/

        public static BHE.BuildingElement BuildingElementOpening(this BHE.BuildingElement be, List<BHG.ICurve> bounds)
        {
            if (be == null || be.BuildingElementProperties == null || !be.BuildingElementProperties.CustomData.ContainsKey("Revit_elementId"))
                return be;

            foreach(BHG.ICurve bound in bounds)
            {
                string revitElementID = (be.BuildingElementProperties.CustomData["Revit_elementId"]).ToString();

                BHE.BuildingElementOpening opening = BuildingElementOpening(bound);

                //Assign the properties from the Element to the Opening
                opening.Name = be.Name;
                opening.CustomData.Add("Revit_elementId", revitElementID);

                be.Openings.Add(opening);
            }

            return be;
        }
    }
}
