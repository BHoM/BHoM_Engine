using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Geometry
{
    public abstract class BHoMGeometry
    {
        protected BHoMGeometry() { }

        public BHoMGeometry ShallowClone()
        {
            return (BHoMGeometry)this.MemberwiseClone();
        }

    }
}
