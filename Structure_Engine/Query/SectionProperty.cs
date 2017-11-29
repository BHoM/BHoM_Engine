//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BH.oM.Structural.Properties;
//using BH.oM.Geometry;
//using BH.Engine.Geometry;

//namespace BH.Engine.Structure.Query
//{
//    public static partial class Query
//    {
//        public static void Calculate(this SectionProperty property)
//        {
//            List<IntegrationSlice> verticalSlices = Geometry.Create.CreateSlices(property.Edges, Vector.XAxis);
//            List<IntegrationSlice> horizontalSlices = Geometry.Create.CreateSlices(property.Edges, Vector.YAxis);


            
//            Point min = property.Edges.GetBounds().Min;
//            Point max = property.Edges.Bounds().Max;
//            double centreY = 0;
//            double centreX = 0;
//            property.Area = Geometry.Query.IntegrateArea(horizontalSlices, 1, min.Y, max.Y, ref centreY);
//            Geometry.Query.IntegrateArea(verticalSlices, 1, min.X, max.X, ref centreX);

//            property.TotalWidth = max.X - min.X;
//            property.TotalDepth = max.Y - min.Y;
//            property.Iy = Geometry.Query.IntegrateArea(verticalSlices, 1, 2, 1, centreX);
//            property.Iz = Geometry.Query.IntegrateArea(horizontalSlices, 1, 2, 1, centreY);
//            property.Sy = 2 * Geometry.Query.IntegrateArea(verticalSlices, 1, 1, 1, min.X, centreX);
//            property.Sz = 2 * Geometry.Query.IntegrateArea(verticalSlices, 1, 1, 1, min.Y, centreY);
//            property.Rgy = System.Math.Sqrt(property.Iy / property.Area);
//            property.Rgz = System.Math.Sqrt(property.Iz / property.Area);
//            property.Vy = max.X - centreX;
//            property.Vpy = centreX - min.X;
//            property.Vz = max.Y - centreY;
//            property.Vpz = centreY - min.Y;
//            property.Zz = property.Iz / property.Vy;
//            property.Zy = property.Iy / property.Vz;
//            property.J = property.TorsionContant();
//            property.Iw = property.WarpingConstant();
//            property.Asy = ShearArea(verticalSlices, property.Iz, centreX);
//            property.Asz = ShearArea(horizontalSlices, property.Iy, centreY);
//        }

//    }
//}
