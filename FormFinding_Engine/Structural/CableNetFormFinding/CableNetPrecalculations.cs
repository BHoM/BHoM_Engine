using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using FormFinding_Engine.Structural.Goals;


namespace FormFinding_Engine.Structural.CableNetFormFinding
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

            basRadVec.Normalise();

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
                v1.Normalise();
                v2.Normalise();
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

            // double initialForceVal;  // Never used

            Point trPt1 = trPts[0];
            Point crPt = crPts[0];
            Point trPt2 = trPts[1];

            Vector radVec = crPt - trPt1;

            radVec.Normalise();

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
            v1.Normalise();
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

                v2.Normalise();
                v3.Normalise();

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


        public static List<ConstantHorizontalPrestressGoal> HorForceCalcGenericIterative(List<Point> crPts, List<Point> trPts, List<Vector> trForces, List<Vector> crForces, double scaleFactor, out List<Point> newTrPts, out List<double> crPrestressForce, int iterations = 1000)
        {
            //Calculate gridline planes
            List<Plane> grPlns = CalcGridLinePLanes(crPts, trPts);

            //Calculate scaled initial force value
            Vector initRadForce = CalculateScaledInitialRadialForce(trPts[0], crPts[0], trForces[0], scaleFactor);

            //Calculate compressionring vectors
            List<Vector> crVecs = CompressionRingForces(crPts, initRadForce);

            //Calculate radial force vectors
            List<Vector> radVecs = RadialForces(crVecs);

            //Calulate CR force values for output
            //crPrestressForce = crVecs.Select(x => x.Length).ToList();
            List<Vector> addCrForce, addRadForce;

            //Calculate distributed point loads on compression ring
            CalculateCrForceComponents(crForces, radVecs, crVecs, out addCrForce, out addRadForce);



            //Project compressionring forces to gridline planes
            //List<Vector> outOfBalanceVecs;
            //List<Vector> crProjForces = ProjectForce(crForces, grPlns, out outOfBalanceVecs);
            //List<Vector> crProjForces = crForces;

            //crPrestressForce = CalculateCrPrestressForce(crVecs, outOfBalanceVecs);
            //crPrestressForce = CalculateCrPrestressForce2(ref crVecs, outOfBalanceVecs, ref crProjForces);

            List<Vector> totCrForce = AddVectors(crVecs, addCrForce);
            crPrestressForce = totCrForce.Select(x => x.Length).ToList();

            List<Vector> totRadVecs = AddVectors(radVecs, addRadForce);

            //Calculate radial force vectors
            //List<Vector> radVecs = RadialForces(crVecs, crProjForces);

            //Project Tensionring forces
            //List<Vector> trProjForces = ProjectForce(trForces, grPlns);
            List<Vector> trProjForces = trForces;

            //Calculate total tensionring forces
            List<Vector> totHorTrForce = AddVectorsHorisontal(totRadVecs, trProjForces);

            //Loop through tensionring points until equilibrium is found
            List<Vector> trVecs;
            FindTensionRingPositionAndForce(trPts, grPlns, totHorTrForce, iterations, out newTrPts, out trVecs);

            //Construct the prestress goals
            return ConstructPrestressGoals(newTrPts, crPts, totRadVecs, trVecs);

        }

        private static List<Vector> AddVectors(List<Vector> vecs1, List<Vector> vecs2)
        {
            List<Vector> sums = new List<Vector>();

            for (int i = 0; i < vecs1.Count; i++)
            {
                sums.Add(vecs1[i] + vecs2[i]);
            }

            return sums;
        }

        private static void CalculateCrForceComponents(List<Vector> crForces, List<Vector> radVecs, List<Vector> crVecs, out List<Vector> addCrForce, out List<Vector> addRadForce)
        {
            List<Vector> crUnits = crVecs.Select(x => x / x.Length).ToList();
            List<Vector> radUnits = radVecs.Select(x => x / x.Length).ToList();
            int count = crUnits.Count;
            int quarterCount = count / 4;
            int halfQ = quarterCount / 2;

            addCrForce = FillWithZeros(count);
            addRadForce = FillWithZeros(count);

            for (int i = 0; i < 4; i++)
            {
                CalcCrForceCompQuarter(i * quarterCount, quarterCount, crUnits,  crForces, radUnits, ref addCrForce, ref addRadForce);
            }

            for (int i = 0; i < count; i += quarterCount)
            {
                int prev = i == 0 ? count - 1 : i-1;
                Vector force = crForces[i].DuplicateVector();
                force.Z = 0;
                addRadForce[i] = addCrForce[i] - addCrForce[prev] + force;
            }

        }

        private static void CalcCrForceCompQuarter(int start, int quarterCount, List<Vector> crUnits, List<Vector> crForces, List<Vector> radVecs, ref List<Vector> addCrForce, ref List<Vector> addRadForce)
        {
            int mid = quarterCount/2;
            Vector midForce = crForces[start + mid].DuplicateVector();
            midForce.Z = 0;
            Vector radProj = ProjectVector(radVecs[start + mid], midForce);
            Vector rem = midForce - radProj;

            addRadForce[start + mid] += radProj;


            Vector v2 = crUnits[start + mid - 1];
            Vector v3 = crUnits[start + mid];

            double a = (rem.X * v3.Y - rem.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
            double b = (v2.X * rem.Y - v2.Y * rem.X) / (v2.X * v3.Y - v2.Y * v3.X);

            addCrForce[start + mid - 1] = v2 * a;
            addCrForce[start + mid] = v3 * (-b);

            for (int i = start+mid-1; i > start; i--)
            {
                Vector f = addCrForce[i] + crForces[i];
                f.Z = 0;
                v2 = crUnits[i - 1];
                v3 = radVecs[i];

                a = (f.X * v3.Y - f.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                b = (v2.X * f.Y - v2.Y * f.X) / (v2.X * v3.Y - v2.Y * v3.X);

                addCrForce[i - 1] = v2 * a;
                addRadForce[i] = v3 * b;
            }


            for (int i = start + mid+1; i < start + quarterCount; i++)
            {
                Vector f = crForces[i] - addCrForce[i - 1];
                f.Z = 0;
                v2 = crUnits[i];
                v3 = radVecs[i];

                a = (f.X * v3.Y - f.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                b = (v2.X * f.Y - v2.Y * f.X) / (v2.X * v3.Y - v2.Y * v3.X);

                addCrForce[i] = v2 * (-a);
                addRadForce[i] = v3 * b;
            }

            //TODO: finish this

        }

        private static List<double> CalculateCrPrestressForce2(ref List<Vector> crVecs, List<Vector> outOfBalanceVecs, ref List<Vector> crForces)
        {
            //List<double> psForces = new List<double>();

            Vector v1;
            
            v1 = ProjectVector(crVecs[0], outOfBalanceVecs[1]);
            crVecs[0] += v1;
            crForces[1] += v1;

            v1 = ProjectVector(crVecs[13], outOfBalanceVecs[13]);
            crVecs[13] -= v1;
            crForces[13] += v1;

            v1 = ProjectVector(crVecs[14], outOfBalanceVecs[15]);
            crVecs[14] += v1;
            crForces[15] += v1;

            v1 = ProjectVector(crVecs[27], outOfBalanceVecs[27]);
            crVecs[27] -= v1;
            crForces[27] += v1;

            v1 = ProjectVector(crVecs[28], outOfBalanceVecs[29]);
            crVecs[28] += v1;
            crForces[29] += v1;

            v1 = ProjectVector(crVecs[41], outOfBalanceVecs[41]);
            crVecs[41] -= v1;
            crForces[41] += v1;

            v1 = ProjectVector(crVecs[42], outOfBalanceVecs[43]);
            crVecs[42] += v1;
            crForces[43] += v1;

            v1 = ProjectVector(crVecs[55], outOfBalanceVecs[55]);
            crVecs[55] -= v1;
            crForces[55] += v1;

            //crVecs[0] += ProjectVector(crVecs[0], outOfBalanceVecs[1]);
            //crVecs[13] -= ProjectVector(crVecs[13], outOfBalanceVecs[13]);
            //crVecs[14] += ProjectVector(crVecs[14], outOfBalanceVecs[15]);
            //crVecs[27] -= ProjectVector(crVecs[27], outOfBalanceVecs[27]);
            //crVecs[28] += ProjectVector(crVecs[28], outOfBalanceVecs[29]);
            //crVecs[40] -= ProjectVector(crVecs[40], outOfBalanceVecs[40]);
            //crVecs[41] += ProjectVector(crVecs[41], outOfBalanceVecs[42]);
            //crVecs[55] -= ProjectVector(crVecs[55], outOfBalanceVecs[55]);



            return crVecs.Select(x => x.Length).ToList();
        }

        private static Vector ProjectVector(Vector target, Vector v)
        {
            return Vector.DotProduct(target, v) / Vector.DotProduct(target, target) * target;
        }
        private static List<double> CalculateCrPrestressForce(List<Vector> crVecs, List<Vector> outOfBalanceVecs)
        {
            List<double> psForces = new List<double>();

            List<Vector> crUnitVectors = new List<Vector>();

            foreach (Vector v in crVecs)
            {
                Vector dup = v.DuplicateVector();
                dup.Normalise();
                crUnitVectors.Add(dup);
            }

            List<Vector> additionalCrForce = FillWithZeros(crVecs.Count);

            for (int i = 0; i < outOfBalanceVecs.Count; i++)
            {
                int prevIndex = (i == 0) ? outOfBalanceVecs.Count - 1 : i - 1;

                Vector v1 = outOfBalanceVecs[i];

                Vector v2 = crUnitVectors[prevIndex];
                Vector v3 = crUnitVectors[i];

                v2.Z = 0;
                v3.Z = 0;

                v2.Normalise();
                v3.Normalise();

                //Calculate the scale factor for the tensionring force vector
                double a = (v1.X * v3.Y - v1.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                double b = (v2.X * v1.Y - v2.Y * v1.X) / (v2.X * v3.Y - v2.Y * v3.X);

                additionalCrForce[prevIndex] -= v2 * a;
                additionalCrForce[i] += v3 * b;
            }

            for (int i = 0; i < crVecs.Count; i++)
            {
                Vector res = crVecs[i] + additionalCrForce[i];
                psForces.Add(res.Length);
            }

            return psForces;
        }

        /***********************************************************************/

        private static List<ConstantHorizontalPrestressGoal> ConstructPrestressGoals(List<Point> newTrPts, List<Point> crPts, List<Vector> radVecs, List<Vector> trVecs)
        {
            List<ConstantHorizontalPrestressGoal> prestresses = new List<ConstantHorizontalPrestressGoal>();

            for (int i = 0; i < radVecs.Count; i++)
            {
                int nextIndex = (i == newTrPts.Count - 1) ? 0 : i + 1;

                prestresses.Add(new ConstantHorizontalPrestressGoal(newTrPts[i], crPts[i], radVecs[i].Length));
                prestresses.Add(new ConstantHorizontalPrestressGoal(newTrPts[i], newTrPts[nextIndex], trVecs[i].Length));
            }

            return prestresses;
        }

        /***********************************************************************/

        private static void FindTensionRingPositionAndForce(List<Point> trPts, List<Plane> grPlns, List<Vector> totTrForce, int iterations, out List<Point> newTrPts, out List<Vector> trVecs)
        {
            //Copy Tensionring points
            newTrPts = new List<Point>();

            foreach (Point p in trPts)
            {
                newTrPts.Add(p.DuplicatePoint());
            }

            //Construct list with default elements for tensionring vector values
            trVecs = FillListWithDefault<Vector>(trPts.Count);
            Vector adjustment = new Vector(0, 0, 0);

            //While loop setup parameters and bailout
            bool run = true;
            int counter = 0;
            int bailout = iterations;


            //Loop through the tensionring nodes until convergence is found
            while (run)
            {


                //Adjust the second point in the tr list
                if (counter > 0)
                {
                    int halfCount = newTrPts.Count / 2;
                    Vector dist = newTrPts[halfCount] - trPts[halfCount];

                    if (dist.Length < 0.00000000001)
                        run = false;

                    Vector nextNorm = grPlns[1].Normal;
                    Vector stepAdjustment = dist - Vector.DotProduct(dist, nextNorm) / Vector.DotProduct(nextNorm, nextNorm) * nextNorm;
                    stepAdjustment /= 1000;
                    adjustment += stepAdjustment;
                    newTrPts[1] = trPts[1] + adjustment;

                }

                //Mirror the the second point in the list, assuming symetry
                Point p1, p2, p3;

                p2 = trPts[0];
                p3 = newTrPts[1];

                Plane p = grPlns[0];
                Vector n = p.Normal;

                p1 = p3 - 2 * Vector.DotProduct(new Vector(p3), n) / Vector.DotProduct(n, n) * n;

                Vector force = totTrForce[0];

                Vector v2 = p1 - p2;
                Vector v3 = p2 - p3;

                v2.Z = 0;
                v3.Z = 0;

                v2.Normalise();
                v3.Normalise();

                //Calculate the scale factor for the tensionring force vector
                double a = (force.X * v3.Y - force.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                double b = (v2.X * force.Y - v2.Y * force.X) / (v2.X * v3.Y - v2.Y * v3.X);

                Vector v1 = v3 * b;
                trVecs[0] = v1;

                for (int i = 1; i < newTrPts.Count; i++)
                {
                    int nextIndex = (i == newTrPts.Count - 1) ? 0 : i + 1;
                    int prevIndex = (i == 0) ? newTrPts.Count - 1 : i - 1;


                    Point prevPt, thisPt, nextPt;

                    prevPt = newTrPts[prevIndex];
                    thisPt = newTrPts[i];
                    nextPt = newTrPts[nextIndex];

                    force = totTrForce[i];

                    v3 = force + v1;

                    Line ln = new Line(thisPt, thisPt + v3);

                    Plane nxtPl = grPlns[nextIndex];

                    Point interPt = Intersect.PlaneLine(nxtPl, ln, false);

                    trVecs[i] = v3.DuplicateVector();

                    newTrPts[nextIndex] = interPt;

                    v1 = v3.DuplicateVector();

                }


                counter++;

                if (counter > bailout)
                    run = false;
            }
        }


        /***********************************************************************/

        private static List<Vector> AddVectorsHorisontal(List<Vector> radVecs, List<Vector> trProjForces)
        {
            List<Vector> totForces = new List<Vector>();

            for (int i = 0; i < radVecs.Count; i++)
            {
                totForces.Add(new Vector(radVecs[i].X + trProjForces[i].X, radVecs[i].Y + trProjForces[i].Y, 0));
            }

            return totForces;
        }

        /***********************************************************************/

        private static List<Vector> ProjectForce(List<Vector> forces, List<Plane> plns, out List<Vector> outOfBalanceVectors)
        {
            List<Vector> prForces = new List<Vector>();
            outOfBalanceVectors = new List<Vector>();

            for (int i = 0; i < forces.Count; i++)
            {
                Vector f = forces[i];
                Vector n = plns[i].Normal;
                Vector proj = Vector.DotProduct(f, n) / Vector.DotProduct(n, n) * n;
                prForces.Add(f- proj);
                outOfBalanceVectors.Add(proj);
            }

            return prForces;
        }

        private static Vector ProjectToPlane(Vector v, Plane p, out Vector remainder)
        {
            Vector n = p.Normal;
            remainder = Vector.DotProduct(v, n) / Vector.DotProduct(n, n) * n;
            return v - remainder;
        }

        /***********************************************************************/

        private static Vector CalculateScaledInitialRadialForce(Point trPt, Point crPt, Vector trForce, double scaleFactor)
        {
            Vector radVec = crPt - trPt;

            radVec.Normalise();

            if (radVec.Z == 0)
            {
                return null;
            }

            radVec *= (trForce.Z / radVec.Z) * scaleFactor;
            return radVec;
        }

        /***********************************************************************/

        private static List<Plane> CalcGridLinePLanes(List<Point> crPts, List<Point> trPts)
        {
            Vector z = new Vector(0, 0, 1);

            List<Plane> grPlns = new List<Plane>();

            for (int i = 0; i < trPts.Count; i++)
            {
                Vector dup = crPts[i] - trPts[i];
                dup.Z = 0;
                dup.Normalise();

                Vector norm = Vector.CrossProduct(dup, z);
                
                grPlns.Add(new Plane(trPts[i], norm));
            }

            return grPlns;
        }

        /***********************************************************************/

        private static List<Vector> RadialForces(List<Vector> crVecs)
        {
            List<Vector> radVecs = new List<Vector>();

            for (int i = 0; i < crVecs.Count; i++)
            {
                Vector v1, v2;
                v1 = (i == 0) ? crVecs[crVecs.Count - 1] : crVecs[i - 1];
                v2 = crVecs[i];

                radVecs.Add((v2 - v1));
            }

            return radVecs;
        }

        /*****************************************************************************/

        private static List<Vector> RadialForces(List<Vector> crVecs, List<Vector> crForces)
        {
            List<Vector> radVecs = new List<Vector>();

            for (int i = 0; i < crVecs.Count; i++)
            {
                Vector v1, v2, v3;
                v1 = (i == 0) ? crVecs[crVecs.Count - 1] : crVecs[i - 1];
                v2 = crVecs[i];
                v3 = crForces[i].DuplicateVector();
                v3.Z = 0;

                radVecs.Add((v2 - v1)+v3);
            }

            return radVecs;
        }

        /***********************************************************************/

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

            v2.Normalise();
            v3.Normalise();

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
                compRingVec.Normalise();

                compVecs.Add(compRingVec * a);
            }


            return compVecs;
        }

        /***********************************************************************/

        private static List<T> FillListWithDefault<T>(int size)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < size; i++)
            {
                list.Add(default(T));
            }

            return list;
        }

        /***********************************************************************/

        private static List<Vector> FillWithZeros(int size)
        {
            List<Vector> list = new List<Vector>();

            for (int i = 0; i < size; i++)
            {
                list.Add(new Vector(0, 0, 0));
            }

            return list;
        }

        /***********************************************************************/

        public static List<ConstantHorizontalPrestressGoal> HorForceCalcGenericIterativeOld(List<Point> crPts, List<Point> trPts, List<Vector> trForces, List<Vector> crForces, double scaleFactor, out List<Point> newTrPts, int iterations = 1000)
        {


            //Calculate gridline planes
            List<Plane> grPlns = CalcGridLinePLanes(crPts, trPts);


            //Calculate scaled initial force value
            Point trPt = trPts[0];
            Point crPt = crPts[0];
            Vector radVec = crPt - trPt;

            radVec.Normalise();

            if (radVec.Z == 0)
            {
                newTrPts = null;
                return null;
            }

            radVec *= (trForces[0].Z / radVec.Z) * scaleFactor;

            //Calculate compressionring vectors
            List<Vector> crVecs = CompressionRingForces(crPts, radVec);


            //Project compressionring forces to gridline planes
            List<Vector> prCrForce = new List<Vector>();


            for (int i = 0; i < trForces.Count; i++)
            {
                Vector l = crForces[i];
                Vector n = grPlns[i].Normal;
                prCrForce.Add(l - Vector.DotProduct(l, n) / Vector.DotProduct(n, n) * n);
            }


            //Calculate radial force vectors
            List<Vector> radVecs = RadialForces(crVecs, prCrForce);


            //Project Tensionring forces

            List<Vector> projForces = new List<Vector>();

            for (int i = 0; i < trForces.Count; i++)
            {
                Vector l = trForces[i];
                Vector n = grPlns[i].Normal;
                projForces.Add(l - Vector.DotProduct(l, n) / Vector.DotProduct(n, n) * n);
            }

            //Calculate total tensionring forces

            List<Vector> totForces = new List<Vector>();

            for (int i = 0; i < radVecs.Count; i++)
            {
                totForces.Add(new Vector(radVecs[i].X + projForces[i].X, radVecs[i].Y + projForces[i].Y, 0));
            }

            //Copy Tensionring points
            newTrPts = new List<Point>();

            foreach (Point p in trPts)
            {
                newTrPts.Add(p.DuplicatePoint());
            }

            //Construct list with default elements for tensionring vector values
            List<Vector> trVecs = FillListWithDefault<Vector>(trPts.Count);
            Vector adjustment = new Vector(0, 0, 0);

            bool run = true;

            int counter = 0;
            int bailout = iterations;


            while (run)
            {


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

                v2.Normalise();
                v3.Normalise();

                double a = (force.X * v3.Y - force.Y * v3.X) / (v2.X * v3.Y - v2.Y * v3.X);
                double b = (v2.X * force.Y - v2.Y * force.X) / (v2.X * v3.Y - v2.Y * v3.X);

                Vector v1 = v3 * b;

                List<Point> tempPts = FillListWithDefault<Point>(newTrPts.Count);

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

                    newTrPts[nextIndex] = interPt;


                    v1 = v3;

                }


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

    }
}
