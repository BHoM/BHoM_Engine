using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BHG = BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Opening Opening(BHG.ICurve curve)
        {
            return Opening(curve as dynamic);
        }

        /***************************************************/

        public static Opening Opening(BHG.PolyCurve pCurve)
        {
            return new Opening
            {
                OpeningCurve = pCurve
            };
        }

        /***************************************************/

        public static Opening Opening(IEnumerable<BHG.Polyline> pLines)
        {
            return new Opening
            {
                OpeningCurve = Geometry.Create.PolyCurve(pLines)
            };
        }

        /***************************************************/

        public static Opening Opening(BHG.Polyline pLine)
        {
            return new Opening
            {
                OpeningCurve = Geometry.Create.PolyCurve(new BHG.Polyline[] { pLine })
            };
        }

        /***************************************************/
        
        public static BuildingElement BuildingElementOpening(this BuildingElement be, BHG.ICurve bound)
        {
            return be.BuildingElementOpening(new List<BHG.ICurve> { bound });
        }

        /***************************************************/

        public static BuildingElement BuildingElementOpening(this BuildingElement be, List<BHG.ICurve> bounds)
        {
            if (be == null || be.BuildingElementProperties == null || !be.BuildingElementProperties.CustomData.ContainsKey("Revit_elementId"))
                return be;

            foreach(BHG.ICurve bound in bounds)
            {
                string revitElementID = (be.BuildingElementProperties.CustomData["Revit_elementId"]).ToString();

                Opening opening = Opening(bound);

                //Assign the properties from the Element to the Opening
                opening.Name = be.Name;
                opening.CustomData.Add("Revit_elementId", revitElementID);

                be.Openings.Add(opening);
            }

            return be;
        }
    }
}
