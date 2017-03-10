using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
  
namespace BHoM.Geometry
{

    public static partial class Create
    {
        public static Point Point(double x, double y, double z)
        {
            return new Point(x, y, z);
        }
    }

}
