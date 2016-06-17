using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Structural;
using BHoM.Structural.SectionProperties;
using BHoM.Structural.Results.Bars;
using BHoM.Structural.Results.Nodes;
using BHoM.Structural.Loads;
using BHoM.Global;


namespace BHoM_Engine.FormFinding
{
    public class Structure
    {
        public List<Bar> Bars = new List<Bar>();
        public List<Node> Nodes = new List<Node>();
        public List<Node> LockedNodes = new List<Node>();

        public Dictionary<string, NodalResult> nodalResultCollection = new Dictionary<string, NodalResult>();
        public Dictionary<string, BarForce> barForceCollection = new Dictionary<string, BarForce>();

        public Loadcase loadcase = new Loadcase(1, "Loadcase", LoadNature.Dead, 0);

        public double dt = 0.01;
        public double t = 0;
        public double c = 0.95;
        public double nodeTol = 0.1;
        public double treshold;

        public double safeTimeStep;
        public double maxTimeStep;

        public Structure(List<Bar> bars)
        {
            foreach (Bar bar in bars)
            {
                bar.Name = this.Bars.Count.ToString();

                if (NodeExists(bar.StartNode) != null)
                    bar.StartNode = NodeExists(bar.StartNode);
                else
                {
                    bar.StartNode.Name = this.Nodes.Count.ToString();
                    this.Nodes.Add(bar.StartNode);
                }

                if (NodeExists(bar.EndNode) != null)
                    bar.EndNode = NodeExists(bar.EndNode);
                else
                {
                    bar.EndNode.Name = this.Nodes.Count.ToString();
                    this.Nodes.Add(bar.EndNode);
                }

                this.Bars.Add(bar);
            }
        }

        public void SetMassPerMetre()
        {
            foreach (Bar bar in Bars)
            {
                double diameter;
                if (double.TryParse((string)bar.CustomData["SecType"], out diameter))
                {
                    bar.CustomData.Add("Area", Math.Pow(diameter / 2, 2) * Math.PI);
                    bar.CustomData.Add("MassPerMetre", (double)bar.CustomData["Area"] * bar.Material.Density * 10);
                }
                else
                {
                    if ((string)bar.CustomData["SecType"] == "Radial")
                    {
                        bar.CustomData.Add("Area", 0.00846);
                        SectionProperty radial = new SectionProperty();
                        radial.MassPerMetre = 69.8 * 10;
                        bar.SectionProperty = radial;
                    }

                    else if ((string)bar.CustomData["SecType"] == "TensionRing")
                    {
                        bar.CustomData.Add("Area", 0.06168);
                        SectionProperty tensionRing = new SectionProperty();
                        tensionRing.MassPerMetre = 508.9 * 10;
                        bar.SectionProperty = tensionRing;
                    }

                    else if ((string)bar.CustomData["SecType"] == "Diagonal")
                    {
                        bar.CustomData.Add("Area", 0.00249);
                        SectionProperty diagonal = new SectionProperty();
                        diagonal.MassPerMetre = 20.5 * 10;
                        bar.SectionProperty = diagonal;
                    }

                    else if ((string)bar.CustomData["SecType"] == "CompressionRing")
                    {
                        bar.CustomData.Add("Area", 1 * 2);
                        bar.SectionProperty.MassPerMetre = (double)bar.CustomData["Area"] * bar.Material.Density * 10;
                    }

                    else
                    {
                        bar.CustomData.Add("Area", Math.Pow(0.04 / 2, 2) * Math.PI);
                        bar.SectionProperty.MassPerMetre = (double)bar.CustomData["Area"] * bar.Material.Density * 10;
                    }
                }
            }
        }

        public void RestrainXY()
        {
            foreach (Node node in Nodes)
                node.SetConstraint(new NodeConstraint("XYFix", new double[] { -1, -1, 0, 0, 0, 0 }));
        }

        public Node NodeExists(Node node)
        {
            Node existingNode = null;
            foreach (Node structureNode in this.Nodes)
            {
                double d = structureNode.Point.DistanceTo(node.Point);
                if (d < this.nodeTol)
                    existingNode = structureNode;
            }

            return existingNode;
        }

        public void SetStartVelocity()
        {
            foreach (Node node in this.Nodes)
            {
                FormFinding.NodalResult nodeResult = new FormFinding.NodalResult(Int32.Parse(node.Name));
                nodeResult.Velocity = new Vector(0, 0, 0);

                nodalResultCollection.Add(node.Name + ":" + t.ToString(), nodeResult);
            }
        }

        public List<Bar> GetConnectedBars(Node node)
        {
            List<Bar> connectedBars = new List<Bar>();
            foreach (Bar bar in Bars)
            {
                if (bar.StartNode.Point.DistanceTo(node.Point) < nodeTol)
                    connectedBars.Add(bar);
                if (bar.EndNode.Point.DistanceTo(node.Point) < nodeTol)
                    connectedBars.Add(bar);
            }
            return connectedBars;
        }

        //public void CalcSlackLength()
        //{
        //    foreach (Bar bar in Bars)
        //    {
        //        if ((double)bar.CustomData["prestress"] != 0)
        //            bar.CustomData.Add("SlackLength", (bar.Length - (double)bar.CustomData["prestress"]/(double)bar.CustomData["Stiffness"]));
        //        else
        //            bar.CustomData.Add("SlackLength", (bar.Length * (double)bar.CustomData["lengthMultiplier"]));               
        //    }
                
        //}

        public void SetNodeMass()
        {
            foreach (Node node in Nodes)
            {
                List<Bar> connectedBars = GetConnectedBars(node);
                double S = 0;
                double g = 1;
                foreach (Bar bar in connectedBars)
                    S += bar.Material.YoungsModulus * (double)bar.CustomData["Area"] / (double)bar.CustomData["StartLength"] + g * (double)bar.CustomData["prestress"] / bar.Length;
                double M = dt * dt / 2 * S;
                node.CustomData.Add("FormFindMass", M);
            }
        }

        public void UpdateNodeMass()
        {
            foreach (Node node in Nodes)
            {
                List<Bar> connectedBars = GetConnectedBars(node);
                double S = 0;
                double g = 1;
                foreach (Bar bar in connectedBars)
                    S += bar.Material.YoungsModulus * (double)bar.CustomData["Area"] / (double)bar.CustomData["StartLength"] + g * (double)bar.CustomData["prestress"] / bar.Length;
                double M = dt * dt / 2 * S;
                node.CustomData["FormFindMass"]= M;
            }
        }


        public void FindLockedNodes(List<Node> listLocked)
        {
            foreach (Node node in this.Nodes)
                node.CustomData.Add("isLocked", false);

            foreach (Node node in this.Nodes)
                foreach (Node lockedPt in listLocked)
                    if (node.Point.DistanceTo(lockedPt.Point) < this.nodeTol)
                        node.CustomData["isLocked"] = true;
        }

        public void CalcBarForceOld()
        {
            foreach (Bar bar in this.Bars)
            {
                FormFinding.BarForce barForce = new FormFinding.BarForce(Int32.Parse(bar.Name), 0.5, this.loadcase, new Plane(bar.StartNode.Point, Vector.CrossProduct(new Vector(bar.EndNode.X - bar.StartNode.X, bar.EndNode.Y - bar.StartNode.Y, bar.EndNode.Z - bar.StartNode.Z), new Vector(0, 0, 1))));
                Vector unitVec = new Vector((bar.EndNode.Point.X - bar.StartNode.Point.X) / bar.Length, (bar.EndNode.Point.Y - bar.StartNode.Point.Y) / bar.Length, (bar.EndNode.Point.Z - bar.StartNode.Point.Z) / bar.Length);

                double dl = bar.Length - (double)bar.CustomData["SlackLength"];

                barForce.FX = unitVec.X * dl * (double)bar.CustomData["Stiffness"];
                barForce.FY = unitVec.Y * dl * (double)bar.CustomData["Stiffness"];
                barForce.FZ = unitVec.Z * dl * (double)bar.CustomData["Stiffness"];

                barForceCollection.Add(bar.Name + ":" + t.ToString(), barForce);

            }
        }

        public void CalcBarForce()
        {
            foreach (Bar bar in this.Bars)
            {
                FormFinding.BarForce barForce = new FormFinding.BarForce(Int32.Parse(bar.Name), 0.5, this.loadcase, new Plane(bar.StartNode.Point, Vector.CrossProduct(new Vector(bar.EndNode.X - bar.StartNode.X, bar.EndNode.Y - bar.StartNode.Y, bar.EndNode.Z - bar.StartNode.Z), new Vector(0, 0, 1))));
                Vector unitVec = new Vector((bar.EndNode.Point.X - bar.StartNode.Point.X) / bar.Length, (bar.EndNode.Point.Y - bar.StartNode.Point.Y) / bar.Length, (bar.EndNode.Point.Z - bar.StartNode.Point.Z) / bar.Length);

                double dl = bar.Length - (double)bar.CustomData["StartLength"];
                double Ks = (bar.Material.YoungsModulus * (double)bar.CustomData["Area"] + (double)bar.CustomData["prestress"]) / (double)bar.CustomData["StartLength"];

                bar.CustomData.Add("Ks", Ks);

                double T = (double)bar.CustomData["prestress"] + dl * Ks;

                bar.CustomData.Add("T", T);

                if (T >= 0)
                {
                    barForce.FX = unitVec.X * T;
                    barForce.FY = unitVec.Y * T;
                    barForce.FZ = unitVec.Z * T;
                }
                else
                {
                    barForce.FX = 0;
                    barForce.FY = 0;
                    barForce.FZ = 0;
                }

                barForceCollection.Add(bar.Name + ":" + t.ToString(), barForce);

            }
        }

        public void CalcNodeForce(double gravity)
        {
            foreach (Node node in this.Nodes)
            {
                FormFinding.NodalResult nodeResult = new FormFinding.NodalResult(Int32.Parse(node.Name));

                nodeResult.Force = new Vector(0, 0, 0);

                nodeResult.Force = nodeResult.Force + new BHoM.Geometry.Vector(0, 0, gravity);

                nodalResultCollection.Add(node.Name + ":" + t.ToString(), nodeResult);
            }

            foreach (Bar bar in this.Bars)
            {
                FormFinding.BarForce barForce = barForceCollection[bar.Name + ":" + t.ToString()];
                nodalResultCollection[bar.StartNode.Name + ":" + t.ToString()].Force = nodalResultCollection[bar.StartNode.Name + ":" + t.ToString()].Force + new Vector(barForce.FX, barForce.FY, barForce.FZ - bar.SectionProperty.MassPerMetre * (double)bar.CustomData["StartLength"] / 2);
                nodalResultCollection[bar.EndNode.Name + ":" + t.ToString()].Force = nodalResultCollection[bar.EndNode.Name + ":" + t.ToString()].Force + new Vector(-barForce.FX, -barForce.FY, -barForce.FZ - bar.SectionProperty.MassPerMetre * (double)bar.CustomData["StartLength"] / 2);
            }
        }

        public void SetLockedToZero()
        {
            foreach (Node node in this.Nodes)
            {
                if ((bool)node.CustomData["isLocked"])
                    nodalResultCollection[node.Name + ":" + t.ToString()].Velocity = new Vector(0, 0, 0);
            }
        }

        public void SetRestrainedTranslationsToZero()
        {
            foreach (Node node in this.Nodes)
            {
                if (node.IsConstrained)
                {
                    if (node.Constraint.UX.Type == DOFType.Fixed)
                        nodalResultCollection[node.Name + ":" + t.ToString()].Translation = new Vector(0, nodalResultCollection[node.Name + ":" + t.ToString()].Translation.Y, nodalResultCollection[node.Name + ":" + t.ToString()].Translation.Z);

                    if (node.Constraint.UY.Type == DOFType.Fixed)
                        nodalResultCollection[node.Name + ":" + t.ToString()].Translation = new Vector(nodalResultCollection[node.Name + ":" + t.ToString()].Translation.X, 0, nodalResultCollection[node.Name + ":" + t.ToString()].Translation.Z);
                }
            }
        }

        public void CalcAcceleration()
        {
            foreach (Node node in this.Nodes)
                nodalResultCollection[node.Name + ":" + t.ToString()].Acceleration = nodalResultCollection[node.Name + ":" + t.ToString()].Force / (double)node.CustomData["Mass"];
        }

        public void CalcVelocity()
        {
            foreach (Node node in this.Nodes)
                nodalResultCollection[node.Name + ":" + t.ToString()].Velocity = c * nodalResultCollection[node.Name + ":" + (t - dt).ToString()].Velocity + nodalResultCollection[node.Name + ":" + t.ToString()].Acceleration * (dt);
        }
        public void CalcTranslation()
        {
            foreach (Node node in this.Nodes)
                nodalResultCollection[node.Name + ":" + t.ToString()].Translation = nodalResultCollection[node.Name + ":" + t.ToString()].Velocity * dt;
        }

        public void UpdateGeometry()
        {
            foreach (Node node in this.Nodes)
                node.Point += nodalResultCollection[node.Name + ":" + t.ToString()].Translation;

            foreach (Bar bar in Bars) //Make bar update line and length
            {
                Node newStartNode = bar.StartNode;
                bar.StartNode = newStartNode;
                Node newEndNode = bar.EndNode;
                bar.EndNode = newEndNode;
            }

        }

        public void CalcKineticEnergy()
        {
            foreach (Node node in this.Nodes)
                node.CustomData["KineticEnergy"] = Math.Pow(nodalResultCollection[node.Name + ":" + t.ToString()].Velocity.Length, 2) * (double)node.CustomData["Mass"] / 2.0;
        }

        public bool HasConverged()
        {
            bool hasConverged = true;
            foreach (Node node in this.Nodes)
                if ((double)node.CustomData["KineticEnergy"] > this.treshold)
                    hasConverged = false;

            return hasConverged;
        }

        public int IndexOfLargest(List<double> values)
        {
            int maxIndex = -1;
            double maxValue = 0;
            int index = 0;

            foreach (double val in values)
            {
                if (val > maxValue)
                {
                    maxValue = val;
                    maxIndex = index;
                }
                index++;
            }

            return maxIndex;
        }

        public int IndexOfSmallest(List<double> values)
        {
            int minIndex = -1;
            double minValue = 10000000;
            int index = 0;

            foreach (double val in values)
            {
                if (val < minValue)
                {
                    minValue = val;
                    minIndex = index;
                }
                index++;
            }

            return minIndex;
        }






        /** Find the node with the smallest mass
        *
        */
        public Node FindLightestNode()
        {
            double smallestMass = 1000000000;
            int index = 0;
            foreach (Node node in Nodes)
            {
                if ((double)node.CustomData["Mass"] < smallestMass)
                {
                    smallestMass = (double)node.CustomData["Mass"];
                    index = Nodes.IndexOf(node);
                }
            }
            return Nodes[index];
        }

        /** Find the bar with the greatest stiffness
        *
        */

        public Bar FindStiffestBar()
        {
            if (Bars.Count == 0) return null;             

            double stiffest = 0;
            double barStiffness;
            int index = 0;
            foreach (BHoM.Structural.Bar bar in Bars)
            {         
                barStiffness = bar.Material.YoungsModulus * (double)bar.CustomData["Area"] / (double)bar.Length * (double)bar.CustomData["GlobalStiffnessScaleFactor"] * (double)bar.CustomData["CustomStiffnessScaleFactor"];

                if (barStiffness > stiffest)
                {
                    stiffest = barStiffness;
                    index = Bars.IndexOf(bar);
                }
            }
            return Bars[index];
        }



        /** Loops throught all mNodes and calculates the weight of all bars connected to the node.
        *
        */
        public void SetNodalMassesPerUnitLength(double massPerUnitLength)
        {
            foreach (BHoM.Structural.Node node in Nodes)
            {
                node.CustomData.Add("SumLength", 0);
            }

                foreach (Bar bar in Bars)
            {
                bar.StartNode.CustomData["SumLength"] = (double)bar.StartNode.CustomData["SumLength"] + massPerUnitLength * bar.Length / 2;
                bar.EndNode.CustomData["SumLength"] = (double)bar.StartNode.CustomData["SumLength"] + massPerUnitLength * bar.Length / 2;
            }
        }

        /// <summary>
        ///  Calculate the total average length of bars in the model
        /// </summary>
        /// <returns></returns>
        public double CalculateMeanBarLength()
        {
            double avrgLength = 0;
            foreach (Bar bar in Bars)
            {
                avrgLength += bar.Length;
            }
            avrgLength /= Bars.Count;
            return avrgLength;
        }

        public double CalculateMeanVelocity()
        {
            double mAvrgVelocity;
            // double[] mAvrgVelocities = new double[3];
            Vector mAvrgVelocities = new Vector();

            mAvrgVelocity = 0;

            foreach (BHoM.Structural.Node node in Nodes)
            {
                //mAvrgVelocities[0] += node.Velocity[0];
                //mAvrgVelocities[1] += node.Velocity[1];
                //mAvrgVelocities[2] += node.Velocity[2];
                //mAvrgVelocity += Math.Sqrt(node.Velocity[0] * node.Velocity[0] + node.Velocity[1] * node.Velocity[1] + node.Velocity[2] * node.Velocity[2]);
                mAvrgVelocities = mAvrgVelocities + nodalResultCollection[node.Name + ":" + t.ToString()].Velocity;
                mAvrgVelocity += nodalResultCollection[node.Name + ":" + t.ToString()].Velocity.Length;

            }
            //mAvrgVelocities[0] = mAvrgVelocities[0] / _vertices.Count;
            //mAvrgVelocities[1] = mAvrgVelocities[1] / _vertices.Count;
            //mAvrgVelocities[2] = mAvrgVelocities[2] / _vertices.Count;
            //why? does it do anything?
            mAvrgVelocities = mAvrgVelocities / Nodes.Count;

            return mAvrgVelocity / Nodes.Count;

        }

        public void SetGlobalStriffnessScaleFactor()
        {
            double factor = 0.3;
            foreach (Bar bar in Bars)
                bar.CustomData.Add("GlobalStiffnessScaleFactor", factor);
        }

        public void SetCustomStriffnessScaleFactor()
        {
            double factor = 0.3;
            foreach (Bar bar in Bars)
                bar.CustomData.Add("CustomStiffnessScaleFactor", factor);
        }

        /** Calculates the bounding box for the geometry
*
*/
        public void CalcSafeDynamicTimeStep()
        {
            if (this == null || Bars.Count == 0) { return; }

            double ts1;
            double ts2;
            double newTimeStep;
            double tempStiff = 0;
            double stiffest = 0;
            double lightest;

            Bar stiffestBar = FindStiffestBar();
            Node lightestNode = FindLightestNode();

            //Find the stiffest bar connected to the node.
            foreach (BHoM.Structural.Bar bar in GetConnectedBars(lightestNode))
            {
                tempStiff = bar.Material.YoungsModulus * (double)bar.CustomData["Area"] / (double)bar.Length * (double)bar.CustomData["GlobalStiffnessScaleFactor"] * (double)bar.CustomData["CustomStiffnessScaleFactor"];
                if (tempStiff > stiffest)
                    stiffest = tempStiff;
            }
            ts1 = Math.Sqrt(2 * (double)lightestNode.CustomData["FormFindMass"] / tempStiff);

            //Find the lightest node connected to the stiffest bar.
            if ((double)stiffestBar.StartNode.CustomData["FormFindMass"] > (double)stiffestBar.EndNode.CustomData["FormFindMass"])
                lightest = (double)stiffestBar.StartNode.CustomData["FormFindMass"];
            else
                lightest = (double)stiffestBar.EndNode.CustomData["FormFindMass"];

            ts2 = Math.Sqrt(2 * lightest / ((stiffestBar.Material.YoungsModulus * (double)stiffestBar.CustomData["Area"] / (double)stiffestBar.Length) * (double)stiffestBar.CustomData["GlobalStiffnessScaleFactor"] * (double)stiffestBar.CustomData["CustomStiffnessScaleFactor"]));

            if (ts1 < ts2)
                newTimeStep = ts1;
            else
                newTimeStep = ts2;

            safeTimeStep = newTimeStep;
        }

        public void CalcDynamicMaxTimeStep()
        {
            List<double> stiffnessValues = new List<double>(Bars.Count);
            List<double> massValues = new List<double>(Nodes.Count);

            double tempStiffness;
            double medianStiffness;
            double medianMass;

            foreach (Bar bar in Bars)
            {
                tempStiffness = bar.Material.YoungsModulus * (double)bar.CustomData["Area"] / (double)bar.Length * (double)bar.CustomData["GlobalStiffnessScaleFactor"] * (double)bar.CustomData["CustomStiffnessScaleFactor"];
                stiffnessValues.Add(tempStiffness);
            }
            foreach (Node node in Nodes)
            {
                massValues.Add((double)node.CustomData["Mass"]);
            }

            //Get max, min and median stiffness
            stiffnessValues.Sort();
            medianStiffness = stiffnessValues[(int)Math.Round((double)Bars.Count / 2, 0)];

            //Get max, min and median mass
            massValues.Sort();
            medianMass = massValues[(int)Math.Round((double)Nodes.Count / 2, 0)];

            //Current timestep value is based on the smallest mass and the maximum stiffness.
            maxTimeStep = 0.4 * Math.Sqrt((2 * medianMass) / medianStiffness);
        }

        //public void UpdateTimeStep()
        //{
        //    if (_autoTimeStep)
        //        _timeStep = 0.35 * _safeTimeStep;
        //    else
        //        _timeStep = _maxTimeStep * _timeStepScaleFactor;
        //}


    }
}

