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

        public double dt = 0.05;
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
                        radial.MassPerMetre = (double)bar.CustomData["Area"] * bar.Material.Density * 10;
                        bar.SectionProperty = radial;
                    }

                    else if ((string)bar.CustomData["SecType"] == "TensionRing")
                    {
                        bar.CustomData.Add("Area", 0.06168);
                        SectionProperty tensionRing = new SectionProperty();
                        tensionRing.MassPerMetre = (double)bar.CustomData["Area"] * bar.Material.Density * 10;
                        bar.SectionProperty = tensionRing;
                    }

                    else if ((string)bar.CustomData["SecType"] == "Diagonal")
                    {
                        bar.CustomData.Add("Area", 0.00249);
                        SectionProperty diagonal = new SectionProperty();
                        diagonal.MassPerMetre = (double)bar.CustomData["Area"] * bar.Material.Density * 10;
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

        public void SetStiffness()
        {
            foreach (Bar bar in Bars)
            {
                double Ks = (bar.Material.YoungsModulus * (double)bar.CustomData["Area"] + (double)bar.CustomData["prestress"]) / (double)bar.CustomData["StartLength"];
                bar.CustomData.Add("Ks", Ks);
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
                if (bar.StartPoint.DistanceTo(node.Point) < nodeTol)
                    connectedBars.Add(bar);
                if (bar.EndPoint.DistanceTo(node.Point) < nodeTol)
                    connectedBars.Add(bar);
            }
            return connectedBars;
        }

        public void SetFictionalNodeMass()
        {
            foreach (Node node in Nodes)
            {
                List<Bar> connectedBars = GetConnectedBars(node);
                double S = 0;
                double g = 1;
                foreach (Bar bar in connectedBars)
                {
                    S += bar.Material.YoungsModulus * (double)bar.CustomData["Area"] / (double)bar.CustomData["StartLength"] + g * (double)bar.CustomData["prestress"] / bar.Length;
                }
                        
                double M = dt * dt / 2 * S;
                node.CustomData.Add("Mass", M);
            }
        }

        public void SetLumpedNodeMass()
        {
            foreach (Node node in Nodes)
            {
                List<Bar> connectedBars = GetConnectedBars(node);
                double lumpedMass = 0;
                foreach (Bar bar in connectedBars)
                {                  
                    lumpedMass += bar.SectionProperty.MassPerMetre * bar.Length / 2;
                }

                node.CustomData.Add("Mass", lumpedMass);
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
                node.CustomData["Mass"]= M;
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

        public void CalcBarForce()
        {
            foreach (Bar bar in this.Bars)
            {
                FormFinding.BarForce barForce = new FormFinding.BarForce(Int32.Parse(bar.Name), 0.5, this.loadcase, new Plane(bar.StartPoint, Vector.CrossProduct(new Vector(bar.EndNode.X - bar.StartNode.X, bar.EndNode.Y - bar.StartNode.Y, bar.EndNode.Z - bar.StartNode.Z), new Vector(0, 0, 1))));
                Vector unitVec = new Vector((bar.EndPoint.X - bar.StartPoint.X) / bar.Length, (bar.EndPoint.Y - bar.StartPoint.Y) / bar.Length, (bar.EndPoint.Z - bar.StartPoint.Z) / bar.Length);

                double dl = bar.Length - (double)bar.CustomData["StartLength"];

                double T = (double)bar.CustomData["prestress"] + dl * (double)bar.CustomData["Ks"];

                bar.CustomData["T"] =  T;

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


        public void CalcSafeTimeStep()
        {
            double smallestRatio = 100000000000;
            foreach (Node node in Nodes)
                foreach (Bar bar in GetConnectedBars(node))
                    if ((double)node.CustomData["Mass"] / (double)bar.CustomData["Ks"] < smallestRatio)
                        smallestRatio = (double)node.CustomData["Mass"] / (double)bar.CustomData["Ks"];

            safeTimeStep = Math.Sqrt(2 * smallestRatio);
            dt = safeTimeStep;
        }

    }
}

