using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using FormFinding_Engine.Structural.Goals;


namespace FormFinding_Engine.Structural.CableNetSetUp
{
    public static class CableNetPrecalculations
    {

        //public static List<ConstantHorizontalPrestressGoal> HorForceCalc(List<Point> crPts, List<Point> trPts, List<double> loads, int baseGridIndex)
        public static List<ConstantHorizontalPrestressGoal> HorForceCalc(List<Point> crPts, List<Point> trPts, List<Vector> loadVs, int baseGridIndex)
        {

            
            double baseRadialForceValue;

           
            // Calculate the horizontal Radial Force
            //-----------------------------------------------------------------------------------------------------

            Vector baseLoadVec = loadVs[baseGridIndex];

            Vector basRadVec = crPts[baseGridIndex] - trPts[baseGridIndex];

            basRadVec.Unitize();

            baseRadialForceValue = -1 * (baseLoadVec.Z / basRadVec.Z);
            //baseRadialForceValue = baseLoadVec.Z / basRadVec.Z;

            basRadVec *= baseRadialForceValue;

            Vector xyRadialForceVec = new Vector(basRadVec.X, basRadVec.Y, 0);

            Vector xyLoadForceVec = new Vector(baseLoadVec.X, baseLoadVec.Y, 0);

            Vector xyTotalForceVec = xyRadialForceVec + xyLoadForceVec;




            //Vector basRadVec = crPts[baseGridIndex] - trPts[baseGridIndex];

            //basRadVec.Unitize();

            //baseRadialForceValue = loads[baseGridIndex] / basRadVec.Z;

            //basRadVec *= baseRadialForceValue;

            //Vector xyRadForceVec = new Vector(basRadVec.X, basRadVec.Y, 0);




            //-----------------------------------------------------------------------------------------------------


            // Calculate the horizontal TR force.
            //-----------------------------------------------------------------------------------------------------

            Point trPt = trPts[baseGridIndex];
            //Point nextPt = trPts[baseGridIndex];
            Point prevPt = trPts[baseGridIndex-1];

            Vector trVec = prevPt - trPt;


            xyTotalForceVec /= 2;

            Vector trForceVec = trVec * Vector.DotProduct(xyTotalForceVec, xyTotalForceVec) / Vector.DotProduct(trVec, xyTotalForceVec);

            double trXyPrestressValue = trForceVec.Length;


            //Point trPt = trPts[baseGridIndex];
            ////Point nextPt = trPts[baseGridIndex];
            //Point prevPt = trPts[baseGridIndex - 1];

            //Vector trVec = prevPt - trPt;


            //xyRadForceVec /= 2;

            //Vector trForceVec = trVec * Vector.DotProduct(xyRadForceVec, xyRadForceVec) / Vector.DotProduct(trVec, xyRadForceVec);

            //double trXyPrestressValue = trForceVec.Length;


            //-----------------------------------------------------------------------------------------------------


            // Calculate all radial forces
            //-----------------------------------------------------------------------------------------------------

            List<double[]> radial_Cs = new List<double[]>();

            List<ConstantHorizontalPrestressGoal> prestresses = new List<ConstantHorizontalPrestressGoal>();


            for (int i = 0; i < trPts.Count; i++)
            {
                Point prevTrPoint;

                if (i == 0)
                    prevTrPoint = trPts[trPts.Count - 1];
                else
                    prevTrPoint = trPts[i - 1];

                Point thisCRPt = crPts[i];
                Point thisTRPt = trPts[i];
                Point nextTRPt;

                if (i == trPts.Count - 1)
                    nextTRPt = trPts[0];
                else
                    nextTRPt = trPts[i + 1];

                Vector v1 = prevTrPoint - thisTRPt;
                Vector v2 = nextTRPt - thisTRPt;
                Vector v3 = new Vector(loadVs[i].X, loadVs[i].Y, 0);
                v1.Unitize();
                v2.Unitize();
                v1 *= trXyPrestressValue;
                v2 *= trXyPrestressValue;

                

                Vector v4 = v1 + v2 + v3;

                double preStressValue = v4.Length;

                ConstantHorizontalPrestressGoal radial = new ConstantHorizontalPrestressGoal(thisCRPt, thisTRPt, preStressValue);
                prestresses.Add(radial);

                ConstantHorizontalPrestressGoal trPrestress = new ConstantHorizontalPrestressGoal(thisTRPt, nextTRPt, trXyPrestressValue);
                prestresses.Add(trPrestress);
                

            }


            //for (int i = 0; i < trPts.Count; i++)
            //{
            //    Point prevTrPoint;

            //    if (i == 0)
            //        prevTrPoint = trPts[trPts.Count - 1];
            //    else
            //        prevTrPoint = trPts[i - 1];

            //    Point thisCRPt = crPts[i];
            //    Point thisTRPt = trPts[i];
            //    Point nextTRPt;

            //    if (i == trPts.Count - 1)
            //        nextTRPt = trPts[0];
            //    else
            //        nextTRPt = trPts[i + 1];

            //    Vector v1 = prevTrPoint - thisTRPt;
            //    Vector v2 = nextTRPt - thisTRPt;
            //    v1.Unitize();
            //    v2.Unitize();
            //    v1 *= trXyPrestressValue;
            //    v2 *= trXyPrestressValue;
            //    Vector v3 = v1 + v2;

            //    double preStressValue = v3.Length;

            //    ConstantHorizontalPrestressGoal radial = new ConstantHorizontalPrestressGoal(thisCRPt, thisTRPt, preStressValue);
            //    prestresses.Add(radial);

            //    ConstantHorizontalPrestressGoal trPrestress = new ConstantHorizontalPrestressGoal(thisTRPt, nextTRPt, trXyPrestressValue);
            //    prestresses.Add(trPrestress);


            //}

            return prestresses;




        }

        public static List<ConstantHorizontalPrestressGoal> HorForceCalcGeneric(List<Point> crPts, List<Point> trPts, List<Vector> loadVs, int baseGridIndex)
        {


            //Keeping the initial vector the same as before untill a better solution is found
            /*************************************************************************************************/

            double baseRadialForceValue;


            // Calculate the horizontal Radial Force
            //-----------------------------------------------------------------------------------------------------

            Vector baseLoadVec = loadVs[baseGridIndex];

            Vector basRadVec = crPts[baseGridIndex] - trPts[baseGridIndex];

            basRadVec.Unitize();

            baseRadialForceValue = -1 * (baseLoadVec.Z / basRadVec.Z);

            basRadVec *= baseRadialForceValue;

            Vector xyRadialForceVec = new Vector(basRadVec.X, basRadVec.Y, 0);

            Vector xyLoadForceVec = new Vector(baseLoadVec.X, baseLoadVec.Y, 0);

            Vector xyTotalForceVec = xyRadialForceVec + xyLoadForceVec;



            //-----------------------------------------------------------------------------------------------------


            // Calculate the horizontal TR force.
            //-----------------------------------------------------------------------------------------------------

            Point trPt = trPts[baseGridIndex];
            //Point nextPt = trPts[baseGridIndex];
            Point prevPt = trPts[baseGridIndex - 1];

            Vector trVec = prevPt - trPt;


            xyTotalForceVec /= 2;

            Vector trForceVec = trVec * Vector.DotProduct(xyTotalForceVec, xyTotalForceVec) / Vector.DotProduct(trVec, xyTotalForceVec);

            double trXyPrestressValue = trForceVec.Length;

            /***************************************************************************************************************/


            List<ConstantHorizontalPrestressGoal> prestresses = new List<ConstantHorizontalPrestressGoal>();
            Vector v1;

            v1 = (trPts[0] - trPts[trPts.Count - 1]);
            v1.Unitize();
            v1 *= trXyPrestressValue;

            for (int i = 0; i < trPts.Count; i++)
            {

                Point prevTrPoint;

                if (i == 0)
                    prevTrPoint = trPts[trPts.Count - 1];
                else
                    prevTrPoint = trPts[i - 1];

                Point thisCRPt = crPts[i];
                Point thisTRPt = trPts[i];
                Point nextTRPt;

                if (i == trPts.Count - 1)
                    nextTRPt = trPts[0];
                else
                    nextTRPt = trPts[i + 1];


                Vector v2, v3;


                v2 = thisCRPt - thisTRPt;
                v3 = nextTRPt - thisTRPt;

                v2.Unitize();
                v3.Unitize();

                double a = (v1.X * v3.Y - v1.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                double b = (v2.X * v1.Y - v2.Y * v1.X) / (v2.X * v3.Y - v2.Y * v3.X);

                prestresses.Add(new ConstantHorizontalPrestressGoal(thisTRPt, thisCRPt, a));
                prestresses.Add(new ConstantHorizontalPrestressGoal(thisTRPt, nextTRPt, b));


                v1 = v3 * b;
            }


            return prestresses;
        }

    }
}
