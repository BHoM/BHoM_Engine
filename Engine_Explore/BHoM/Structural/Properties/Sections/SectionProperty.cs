using BHoM.Geometry;
using Engine_Explore.BHoM.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Structural.Properties
{
    public abstract class SectionProperty : Base.BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<Curve> Edges { get; set; }

        public Material Material { get; set; }

        public ShapeType Shape { get; set; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SectionProperty() { }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
