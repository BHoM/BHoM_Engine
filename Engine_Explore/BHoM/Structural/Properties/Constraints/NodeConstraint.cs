using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Base;
using Engine_Explore.BHoM.Structural;
using Engine_Explore.BHoM.Geometry;

namespace Engine_Explore.BHoM.Structural.Properties
{
    public class NodeConstraint : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public double KX { get; set; } = 0;

        public double KY { get; set; } = 0;

        public double KZ { get; set; } = 0;

        public double HX { get; set; } = 0;

        public double HY { get; set; } = 0;

        public double HZ { get; set; } = 0;

        public DOFType UX { get; set; } = DOFType.Free;

        public DOFType UY { get; set; } = DOFType.Free;

        public DOFType UZ { get; set; } = DOFType.Free;

        public DOFType RX { get; set; } = DOFType.Free;

        public DOFType RY { get; set; } = DOFType.Free;

        public DOFType RZ { get; set; } = DOFType.Free;


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public NodeConstraint(string name = "")
        {
            Name = name;
        }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
