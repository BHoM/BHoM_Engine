using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Analytical.Elements;
using BH.oM.Reflection.Attributes;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
using BH.oM.Reflection;
using System.Security.Cryptography.X509Certificates;


namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        //Description
        public static List<List<IRegion>> LevelsByRegion(List<IRegion> regions, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, int decimals = 2)
        {
            List<List<IRegion>> regionsByLevel = new List<List<IRegion>>();
            List<IRegion> oneLevelRegions = new List<IRegion>();
            List<double> levels = new List<double>();

            foreach (IRegion region in regions)
            {
                levels.Add(Math.Round(region.Perimeter.IControlPoints()[0].Z, decimals));
            }

            foreach (double level in levels)
            {
                Math.Round(level, decimals);
            }           
            levels.Distinct().ToList();

            //double minZ = levels.Min() - distanceTolerance;
            //double maxZ = levels.Max() + distanceTolerance;

            //for (int x = 0; x < levels.Count; x++)
            foreach (double level in levels)
            {
                double minZ = level - distanceTolerance;
                double maxZ = level + distanceTolerance;

                oneLevelRegions = regions.Where(x =>
                {
                    double zL = x.Perimeter.IControlPoints()[0].Z;
                    return (zL >= minZ && zL <= maxZ);
                }).ToList();

                /*
                foreach (IRegion region in regions)
                {
                    double regionLevel = region.Perimeter.IControlPoints()[0].Z;
                    if (regionLevel <= maxZ && regionLevel >= minZ)
                        oneLevelRegions.Add(region);
                }
                */
            }

            regionsByLevel.Add(oneLevelRegions);

            /*

            foreach (IRegion region in regions)
            {
                Polyline perimeter = region.Perimeter.ICollapseToPolyline(angleTolerance);
                List<Point> controlpoints = perimeter.IControlPoints();

                foreach (Point controlpoint in controlpoints)
                {
                    levels.Add(controlpoint.Z);
                }

                //double minZ = controlpoints.Select(x => x.Z).Min() - distanceTolerance;
                //double maxZ = controlpoints.Select(x => x.Z).Max() + distanceTolerance;
            }

            levels.Distinct().ToList();

            double minZ = levels.Min() - distanceTolerance;
            double maxZ = levels.Max() + distanceTolerance;

            /*
            foreach (double level in levels)
            {
                if (double )
                {
                    leveledRegion.Add(region)
            }
            }
            

            List<IRegion> leveledRegions = regions.Where(x => { double zL = x.Perimeter.IControlPoints()[0].Z;
                                                               return (zL <= maxZ && zL >= minZ); 
                                                             }).ToList();

            regionsByLevel.Add(leveledRegions);

            */

            return regionsByLevel;
        }

    }
}
