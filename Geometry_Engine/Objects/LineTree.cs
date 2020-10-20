using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public class LineTree
    {
        public Line ThisLine { get; set; } = null;
        public Line Parent { get; set; } = null;
        public Point UnconnectedPoint { get; set; } = null;
    }
}
