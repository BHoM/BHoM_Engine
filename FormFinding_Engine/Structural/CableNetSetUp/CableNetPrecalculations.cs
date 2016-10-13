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

        public static List<ConstantHorizontalPrestressGoal> HorForceCalcGeneric(List<Point> crPts, List<Point> trPts, List<Vector> loadVs, double scaleFactor)
        {

            double initialForceVal;

            Point trPt1 = trPts[0];
            Point crPt = crPts[0];
            Point trPt2 = trPts[1];

            Vector radVec = crPt - trPt1;

            radVec.Unitize();

            if (radVec.Z == 0)
                return null;

            radVec *= loadVs[0].Z / radVec.Z;

            radVec /= -2;

            Vector trVec = trPt2 - trPt1;

            Vector trForceVec = trVec * Vector.DotProduct(radVec, radVec) / Vector.DotProduct(trVec, radVec);

            double trXyPrestressValue = trForceVec.Length * scaleFactor;


            List<ConstantHorizontalPrestressGoal> prestresses = new List<ConstantHorizontalPrestressGoal>();
            Vector v1;

            v1 = (trPts[0]-trPts[trPts.Count - 1]);
            v1.Unitize();
            v1 *= trXyPrestressValue;

            List<double> debug = new List<double>();

            v1.Z = 0;

            for (int i = 0; i < trPts.Count; i++)
            {

                //Add the load
                v1.X -= loadVs[i].X;
                v1.Y -= loadVs[i].Y;

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

                v2.Z = 0;
                v3.Z = 0;

                v2.Unitize();
                v3.Unitize();

                double a = (v1.X * v3.Y- v1.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                double b = (v2.X * v1.Y - v2.Y * v1.X) / (v2.X * v3.Y - v2.Y * v3.X);

                prestresses.Add(new ConstantHorizontalPrestressGoal(thisCRPt, thisTRPt, a));
                prestresses.Add(new ConstantHorizontalPrestressGoal(thisTRPt, nextTRPt, b));


                Vector debugVec = v2 * a + v3 * b - v1;

                debugVec.Z = 0;

                debug.Add(debugVec.Length);

                v1 = v3 * b;
            }


            return prestresses;
        }

        public static List<ConstantHorizontalPrestressGoal> HorForceCalcGenericIterative(List<Point> crPts, List<Point> trPts, List<Vector> loadVs, double scaleFactor, out List<Point> newTrPts, int iterations = 1000)
        {

            double initialForceVal;

            Point trPt0 = trPts[trPts.Count - 1];
            Point trPt1 = trPts[0];
            Point crPt = crPts[0];
            Point trPt2 = trPts[1];

            Vector radVec = crPt - trPt1;

            radVec.Unitize();

            if (radVec.Z == 0)
            {
                newTrPts = null;
                return null;
            }

            radVec *= (loadVs[0].Z / radVec.Z) * scaleFactor;

            //Calculate compressionring vectors
            List<Vector> crVecs = CompressionRingForces(crPts, radVec);
            //Calculate radial force vectors
            List<Vector> radVecs = RadialForces(crVecs);
            //Calculate gridline planes
            List<Plane> grPlns = CalcGridLinePLanes(radVecs, trPts);


            //Calculate initial guess value for tensionring prestress force
            //radVec /= -2;
            //Vector trVec = trPt2 - trPt1;

            //Vector trForceVec = trVec * Vector.DotProduct(radVec, radVec) / Vector.DotProduct(trVec, radVec);

            //double trXyPrestressValue = trForceVec.Length;

            //Vector v1;

            //v1 = (trPts[0] - trPts[trPts.Count - 1]);
            //v1.Unitize();
            //v1 *= -trXyPrestressValue;

            //List<double> debug = new List<double>();

            //v1.Z = 0;

            //Contruct new list of tensionring points and fill the with the initial ones
            newTrPts = new List<Point>();

            foreach (Point p in trPts)
            {
                newTrPts.Add(p.DuplicatePoint());
            }


            //Construct list with default elements for tensionring vector values
            List<Vector> trVecs = FillListWithDefault<Vector>(trPts.Count);

            List<Vector> projForces = new List<Vector>();

            for (int i = 0; i < loadVs.Count; i++)
            {
                Vector l = loadVs[i];
                Vector n = grPlns[i].Normal;
                projForces.Add(l - Vector.DotProduct(l, n) / Vector.DotProduct(n, n) * n);
            }



            List<Vector> totForces = new List<Vector>();

            for (int i = 0; i < radVecs.Count; i++)
            {
                totForces.Add(new Vector(radVecs[i].X + projForces[i].X, radVecs[i].Y + projForces[i].Y, 0));
            }

            //Vector force = totForces[0];

            //Vector v2 = trPt0 - trPt1;
            //Vector v3 = trPt1 - trPt2;

            //v2.Z = 0;
            //v3.Z = 0;

            //v2.Unitize();
            //v3.Unitize();

            //double a = (force.X * v3.Y - force.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
            //double b = (v2.X * force.Y - v2.Y * force.X) / (v2.X * v3.Y - v2.Y * v3.X);

            //Vector v1 = v3 * b;


            bool run = true;

            int counter = 0;
            int bailout = iterations;

            List<Vector> move = FillWithZeros(newTrPts.Count);
            Vector adjustment = new Vector(0, 0, 0);

            while (run)
            {

                //int quaterIndex = newTrPts.Count / 4;

                ////Project the point throught the midplane

                

                if (counter > 0)
                {
                    int halfCount = newTrPts.Count / 2;
                    Vector dist = newTrPts[halfCount] - trPts[halfCount];

                    //dist *= -1;

                    if (dist.Length < 0.00000000001)
                        run = false;

                    Vector nextNorm = grPlns[1].Normal;
                    Vector stepAdjustment = dist - Vector.DotProduct(dist, nextNorm) / Vector.DotProduct(nextNorm, nextNorm) * nextNorm;
                    stepAdjustment /= 1000;
                    adjustment += stepAdjustment;
                    newTrPts[1] = trPts[1] + adjustment;

                }
                Point p1, p2, p3;

                //p1 = newTrPts[newTrPts.Count - 1];
                p2 = trPts[0];
                p3 = newTrPts[1];

                Plane p = grPlns[0];
                Vector n = p.Normal;

                p1 = p3 - 2 * Vector.DotProduct(new Vector(p3), n) / Vector.DotProduct(n, n) * n;



                Vector force = totForces[0];

                Vector v2 = p1 - p2;
                Vector v3 = p2 - p3;

                v2.Z = 0;
                v3.Z = 0;

                v2.Unitize();
                v3.Unitize();

                double a = (force.X * v3.Y - force.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                double b = (v2.X * force.Y - v2.Y * force.X) / (v2.X * v3.Y - v2.Y * v3.X);

                Vector v1 = v3 * b;


                //List<Vector> avgTrVecs = CalcAvgTrVectors(totForces, newTrPts);


                List<Point> tempPts = FillListWithDefault<Point>(newTrPts.Count);
                move = FillWithZeros(newTrPts.Count);

                trVecs[0] = v1;

                for (int i = 1; i < newTrPts.Count; i++)
                {
                    int nextIndex = (i == newTrPts.Count - 1) ? 0 : i + 1;
                    int prevIndex = (i == 0) ? newTrPts.Count - 1 : i - 1;


                    Point prevPt, thisPt, nextPt;

                    prevPt = newTrPts[prevIndex];
                    thisPt = newTrPts[i];
                    nextPt = newTrPts[nextIndex];

                    force = totForces[i];

                    v3 = force + v1;

                    Line ln = new Line(thisPt, thisPt + v3);

                    Plane nxtPl = grPlns[nextIndex];

                    Point interPt = Intersect.PlaneLine(nxtPl, ln, false);

                    trVecs[i] = v3;


                    move[nextIndex] = interPt - nextPt;
                    newTrPts[nextIndex] = interPt;
                    

                    v1 = v3;

                }

                ////Project the point throught the midplane

                //p1 = newTrPts[quaterIndex - 1];
                //p2 = newTrPts[quaterIndex];

                //p = grPlns[quaterIndex];
                //n = p.Normal;

                //p3 = p1 - 2 * Vector.DotProduct(new Vector(p1), n) / Vector.DotProduct(n, n) * n;


                //force = totForces[quaterIndex];

                //v2 = p1 - p2;
                //v3 = p2 - p3;

                //v2.Z = 0;
                //v3.Z = 0;

                //v2.Unitize();
                //v3.Unitize();

                //a = (force.X * v3.Y - force.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                //b = (v2.X * force.Y - v2.Y * force.X) / (v2.X * v3.Y - v2.Y * v3.X);

                //v1 = v2 * a;



                //for (int i = quaterIndex-1; i > 0 ; i--)
                //{
                //    int nextIndex = (i == newTrPts.Count - 1) ? 0 : i + 1;
                //    int prevIndex = (i == 0) ? newTrPts.Count - 1 : i - 1;


                //    Point prevPt, thisPt, nextPt;

                //    prevPt = newTrPts[prevIndex];
                //    thisPt = newTrPts[i];
                //    nextPt = newTrPts[nextIndex];

                //    force = totForces[i];

                //    v3 = force + v1;

                //    Line ln = new Line(thisPt, thisPt + v3);

                //    Plane prevPln = grPlns[prevIndex];

                //    Point interPt = Intersect.PlaneLine(prevPln, ln, false);

                //    newTrPts[prevIndex] = interPt;

                //    v1 = v3;

                //}

                counter++;

                if (counter > bailout)
                    run = false;
            }

            List<ConstantHorizontalPrestressGoal> prestresses = new List<ConstantHorizontalPrestressGoal>();

            for (int i = 0; i < radVecs.Count; i++)
            {
                int nextIndex = (i == newTrPts.Count - 1) ? 0 : i + 1;

                prestresses.Add(new ConstantHorizontalPrestressGoal(newTrPts[i], crPts[i], radVecs[i].Length));
                prestresses.Add(new ConstantHorizontalPrestressGoal(newTrPts[i], newTrPts[nextIndex], trVecs[i].Length));
            }

            return prestresses;
        }

        private static List<Vector> CalcAvgTrVectors(List<Vector> totForces, List<Point> trPts)
        {
            List<Vector> tr1 = FillListWithDefault<Vector>(totForces.Count);
            List<Vector> tr2 = FillListWithDefault<Vector>(totForces.Count);

            for (int i = 0; i < trPts.Count; i++)
            {
                int nextIndex = (i == trPts.Count - 1) ? 0 : i + 1;
                int prevIndex = (i == 0) ? trPts.Count - 1 : i - 1;

                Point prevPt, thisPt, nextPt;

                prevPt = trPts[prevIndex];
                thisPt = trPts[i];
                nextPt = trPts[nextIndex];

                Vector v1 = totForces[i];

                Vector v2 = prevPt - thisPt;
                Vector v3 = thisPt - nextPt;

                v2.Z = 0;
                v3.Z = 0;

                v2.Unitize();
                v3.Unitize();

                double a = (v1.X * v3.Y - v1.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                double b = (v2.X * v1.Y - v2.Y * v1.X) / (v2.X * v3.Y - v2.Y * v3.X);

                tr1[i] = v2 * a;
                tr2[nextIndex] = v3 * -b;

            }

            List<Vector> tr = new List<Vector>();

            for (int i = 0; i < tr1.Count; i++)
            {
                tr.Add(tr1[i] / 2 + tr2[i] / 2);
            }

            return tr;
        }

        private static List<Plane> CalcGridLinePLanes(List<Vector> radVecs, List<Point> trPts)
        {
            Vector z = new Vector(0, 0, 1);

            List<Plane> grPlns = new List<Plane>();

            for (int i = 0; i < radVecs.Count; i++)
            {
                Vector dup = radVecs[i].DuplicateVector();
                dup.Z = 0;
                dup.Unitize();

                Vector norm = Vector.CrossProduct(dup, z);
                
                grPlns.Add(new Plane(trPts[i], norm));
            }

            return grPlns;
        }

        private static List<Vector> RadialForces(List<Vector> crVecs)
        {
            List<Vector> radVecs = new List<Vector>();

            for (int i = 0; i < crVecs.Count; i++)
            {
                Vector v1, v2;
                v1 = (i == 0) ? crVecs[crVecs.Count - 1] : crVecs[i - 1];
                v2 = crVecs[i];

                radVecs.Add(v2 - v1);
            }

            return radVecs;
        } 

        private static List<Vector> CompressionRingForces(List<Point> compRingPts, Vector radVec)
        {
            radVec.Z = 0;

            List<Vector> compVecs = new List<Vector>();

            Point p1, p2, p3;

            p1 = compRingPts[compRingPts.Count - 1];
            p2 = compRingPts[0];
            p3 = compRingPts[1];

            Vector v1 = radVec;

            Vector v2, v3;


            v2 = p2 - p1;
            v3 = p3 - p2;

            v2.Z = 0;
            v3.Z = 0;

            v2.Unitize();
            v3.Unitize();

            double a = (v1.X * v3.Y - v1.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
            double b = (v2.X * v1.Y - v2.Y * v1.X) / (v2.X * v3.Y - v2.Y * v3.X);

            if (a == 0)
                return null;

            if (Math.Abs(Math.Abs(a)-Math.Abs(b)) > 1/Math.Abs(a))
                return null;

            for (int i = 0; i < compRingPts.Count; i++)
            {
                p1 = compRingPts[i];
                p2 = (i == compRingPts.Count - 1) ? compRingPts[0] : compRingPts[i + 1];


                Vector compRingVec = p2 - p1;
                compRingVec.Unitize();

                compVecs.Add(compRingVec * a);
            }


            return compVecs;
        }

        private static List<T> FillListWithDefault<T>(int size)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < size; i++)
            {
                list.Add(default(T));
            }

            return list;
        }

        private static List<Vector> FillWithZeros(int size)
        {
            List<Vector> list = new List<Vector>();

            for (int i = 0; i < size; i++)
            {
                list.Add(new Vector(0, 0, 0));
            }

            return list;
        }

    }
}
