using BH.Engine.Base;
using BH.oM.Data.Genetic;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        public static IGeneticAlgorithmResult GridFittingTest(IGeneticAlgorithmSettings settings, List<Polyline> coverageOutlines, List<Polyline> availableOutlines, double radius, double cellSize = 0.5, List<Polyline> coverageHoles = null, List<Polyline> availableHoles = null, double tol = Tolerance.Distance)
        {
            coverageHoles = coverageHoles ?? new List<Polyline>();
            availableHoles = availableHoles ?? new List<Polyline>();

            m_CoverageOutlines = coverageOutlines.Select(o => o.CleanPolyline(tol, tol)).ToList();
            m_AvailableOutlines = availableOutlines.Select(o => o.CleanPolyline(tol, tol)).ToList();
            m_CoverageHoles = coverageHoles.Select(o => o.CleanPolyline(tol, tol)).ToList();
            m_AvailableHoles = availableHoles.Select(o => o.CleanPolyline(tol, tol)).ToList();

            m_Radius = radius;
            m_Tolerance = tol;

            DoubleParameter[] parameters = new DoubleParameter[]
            {
                new DoubleParameter
                {
                    Min = 0.8,
                    Max = 1.25,
                    Step = 0.01
                },
                new DoubleParameter
                {
                    Min = 0.0,
                    Max = 1.0,
                    Step = 0.01
                },
                new DoubleParameter
                {
                    Min = 0.0,
                    Max = 1.0,
                    Step = 0.01
                },
            };

            Func<double[], double> fitnessFunction = FitGridFunction;
            Action preproc = () =>
            {
                GenerateContainmentGrid(m_AvailableOutlines, m_AvailableHoles, cellSize, m_Radius * Math.Sqrt(2) * 2, m_Tolerance);
            };
            return RunGeneticAlgorithm(settings as dynamic, parameters.ToArray(), fitnessFunction, preproc);
        }

        private static List<Polyline> m_CoverageOutlines;
        private static List<Polyline> m_AvailableOutlines;
        private static List<Polyline> m_CoverageHoles;
        private static List<Polyline> m_AvailableHoles;
        private static double m_Radius;
        private static double m_Tolerance;
        private static bool?[,] m_ContainmentGrid;
        private static SquareGrid m_SquareGrid;

        private static double FitGridFunction(double[] chromosome)
        {
            List<Point> grid = CreatePointGrid(m_AvailableOutlines, m_AvailableHoles, m_SquareGrid, m_ContainmentGrid, m_Radius, chromosome[0], chromosome[1], chromosome[2], m_Tolerance);
            if (grid.Count == 0)
                return -1e+6;

            List<oM.Base.Output<List<Line>, List<double>>> uncoveredOutline = m_CoverageOutlines.Select(x => UncoveredEdges(grid, x, m_Radius, m_Tolerance)).ToList();
            List<oM.Base.Output<List<Line>, List<double>>> uncoveredHoles = m_CoverageHoles.Select(x => UncoveredEdges(grid, x, m_Radius, m_Tolerance)).ToList();
            List<Polyline> joinedOutlines = uncoveredOutline.SelectMany(x => x.Item1).ToList().Join(m_Tolerance);
            List<Polyline> joinedHoles = uncoveredHoles.SelectMany(x => x.Item1).ToList().Join(m_Tolerance);
            double circleCount = grid.Count + joinedOutlines.Sum(x => Math.Ceiling(x.Length() / m_Radius)) + joinedHoles.Sum(x => Math.Max(2, Math.Ceiling(x.Length() / m_Radius)));
            //List<double> dists = grid.Select(x => m_CoverageOutlines.Min(y => x.Distance(y))).ToList();
            List<double> dists = uncoveredOutline[0].Item2;
            if (uncoveredOutline.Count > 1)
            {
                for (int i = 1; i < uncoveredOutline.Count; i++)
                {
                    List<double> cands = uncoveredOutline[i].Item2;
                    for (int j = 0; j < dists.Count; j++)
                    {
                        dists[j] = Math.Min(dists[j], cands[j]);
                    }
                }
            }

            double avgDist = dists.Average();
            double stDev = Math.Sqrt(dists.Sum(x => Math.Pow(x - avgDist, 2)) / dists.Count);
            double stDevThing = stDev / m_Radius;
            return -circleCount - stDevThing;
        }

        private static void GenerateContainmentGrid(List<Polyline> outlines, List<Polyline> holes, double cellSize, double offset, double tol)
        {
            SquareGrid grid = Grid(outlines, holes, cellSize, offset);
            m_SquareGrid = grid;
            m_ContainmentGrid = ContainmentGrid(outlines, holes, grid, tol);
        }

        public static IGeneticAlgorithmResult GeneticSumTest(IGeneticAlgorithmSettings settings, List<DoubleParameter> parameters)
        {
            Func<double[], double> fitnessFunction = SumFunction;
            return IRunGeneticAlgorithm(settings, parameters.ToArray(), fitnessFunction);
        }

        public static IGeneticAlgorithmResult GeneticStDevTest(IGeneticAlgorithmSettings settings, List<DoubleParameter> parameters)
        {
            Func<double[], double> fitnessFunction = StDevFunction;
            return IRunGeneticAlgorithm(settings, parameters.ToArray(), fitnessFunction);
        }

        private static double SumFunction(double[] chromosome)
        {
            // Example fitness function: sum of the chromosome values
            return chromosome.Sum();
        }

        private static double StDevFunction(double[] chromosome)
        {
            // Example fitness function: standard deviation of the chromosome values
            double mean = chromosome.Average();
            double sumOfSquares = chromosome.Sum(x => Math.Pow(x - mean, 2));
            return Math.Sqrt(sumOfSquares / chromosome.Length);
        }

        public static IGeneticAlgorithmResult IRunGeneticAlgorithm(IGeneticAlgorithmSettings settings, DoubleParameter[] parameters, Func<double[], double> fitnessFunction)
        {
            return RunGeneticAlgorithm(settings as dynamic, parameters, fitnessFunction);
        }

        private static Dictionary<double[], double> m_ResultPool = null;
        private static List<GenerationResult> m_ResultsPerGeneration = null;
        private static DoubleParameter[] m_Parameters = null;
        private static Random m_Random = new Random();

        public static GeneticAlgorithmResult RunGeneticAlgorithm(FixedGenerationCount settings, DoubleParameter[] parameters, Func<double[], double> fitnessFunction, Action prep = null)
        {
            if (settings == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't run genetic algorithm without valid settings.");
                return null;
            }

            if (settings.PopulationSize < 2 || settings.PopulationSize % 2 != 0)
            {
                BH.Engine.Base.Compute.RecordError("Population size needs to be a positive even number.");
                return null;
            }

            if (prep != null)
                prep.Invoke();

            // Set global settings
            m_Parameters = parameters;
            m_ResultPool = new Dictionary<double[], double>(new DoubleArrayComparer());
            m_ResultsPerGeneration = new List<GenerationResult>();

            // Generate first generation
            List<(double[], double)> population = InitializePopulation((int)Math.Round(settings.PopulationSize * settings.InitialBoost), parameters, fitnessFunction);
            StoreResults(population);

            //TODO: remember to check if the size is not equal to the number of permutations - then simply all options checked
            int generation = 0;
            int stagnant = 0;
            int maxStagnant = settings.MaxStagnant;
            int maintain = (int)(Math.Round(settings.PopulationSize * settings.ForceMaintainRatio));
            int drop = (int)(Math.Round(settings.PopulationSize * settings.ForceDropRatio));


            // Generate further generations
            while (generation < settings.GenerationCount - 1)
            {
                // Pick most fit individuals from previous generation to maintain
                List<(double[], double)> toMaintain = population.OrderByDescending(individual => individual.Item2).Take(maintain).ToList();

                // Pick least fit individuals from previous generation to drop without a chance to breed
                population = population.OrderBy(individual => individual.Item2).Skip(drop).ToList();

                // Generate new generation
                List<(double[], double)> newPopulation = new List<(double[], double)>();
                for (int i = 0; i < settings.PopulationSize; i += 2)
                {
                    // Select parents
                    double[] parent1 = ISelectFirstParent(population, settings.SelectionMethod);
                    double[] parent2 = ISelectSecondParent(population, parent1, settings.SecondParentSelectionMethod);

                    // Perform crossover
                    (double[] child1, double[] child2) = ICrossover(parent1, parent2, settings.CrossoverMethod);

                    // Perform mutations
                    IMutate(child1, settings.MutationMethod, parameters);
                    double fitness = fitnessFunction(child1);
                    newPopulation.Add((child1, fitness));

                    IMutate(child2, settings.MutationMethod, parameters);
                    fitness = fitnessFunction(child2);
                    newPopulation.Add((child2, fitness));
                }

                //TODO: could easily be parallelised if fitness calculation moved to here

                // Try including the most fit individuals from previous generation, if more fit than the weakest in the current one
                newPopulation = newPopulation.OrderBy(x => x.Item2).ToList();
                HashSet<double[]> currentIndividuals = new HashSet<double[]>(newPopulation.Select(x => x.Item1), new DoubleArrayComparer());
                foreach ((double[], double) individual in toMaintain)
                {
                    if (currentIndividuals.Contains(individual.Item1))
                        continue;

                    for (int i = maintain - 1; i >= 0; i--)
                    {
                        if (newPopulation[i].Item2 < individual.Item2)
                        {
                            newPopulation[i] = individual;
                            break;
                        }
                    }
                }

                // Store results and increment
                if (population.OrderByDescending(x => x.Item2).First().Item1.SequenceEqual(newPopulation.OrderByDescending(x => x.Item2).First().Item1))
                    stagnant++;
                else
                    stagnant = 0;

                population = newPopulation;
                StoreResults(population);

                if (stagnant == maxStagnant)
                    break;

                generation++;
            }

            // Return results
            return new GeneticAlgorithmResult { Generations = m_ResultsPerGeneration };
        }

        private static void StoreResults(IEnumerable<(double[], double)> population)
        {
            foreach ((double[], double) individual in population)
            {
                m_ResultPool[individual.Item1] = individual.Item2;
            }

            m_ResultsPerGeneration.Add(new GenerationResult
            {
                GenerationNumber = m_ResultsPerGeneration.Count,
                Population = population.Select(x => new Individual { Genes = x.Item1.ToList(), Fitness = x.Item2 }).OrderByDescending(x => x.Fitness).ToList(),
            });
        }

        private static List<(double[], double)> InitializePopulation(int populationSize, DoubleParameter[] parameters, Func<double[], double> fitnessFunction)
        {
            List<(double[], double)> population = new List<(double[], double)>();

            int i = 0;
            while (i < populationSize)
            {
                double[] chromosome = new double[parameters.Length];
                for (int j = 0; j < parameters.Length; j++)
                {
                    double min = parameters[j].Min;
                    double max = parameters[j].Max;
                    double step = parameters[j].Step;
                    chromosome[j] = (min + m_Random.NextDouble() * (max - min)).Round(step);
                }

                if (population.Any(x => x.Item1.SequenceEqual(chromosome)))
                    continue;

                double fitness = fitnessFunction(chromosome);
                population.Add((chromosome, fitness));
                i++;
            }

            return population;
        }

        public static double[] ISelectFirstParent(List<(double[], double)> population, IFirstParentSelectionMethod selectionMethod)
        {
            return SelectParent(population, selectionMethod as dynamic);
        }

        public static double[] SelectParent(List<(double[], double)> population, RouletteWheelSelection selectionMethod)
        {
            double totalFitness = population.Sum(individual => individual.Item2);
            double randomValue = m_Random.NextDouble() * totalFitness;
            double cumulativeFitness = 0.0;

            foreach ((double[], double) individual in population)
            {
                cumulativeFitness += individual.Item2;
                if (cumulativeFitness >= randomValue)
                {
                    return individual.Item1;
                }
            }

            return population.Last().Item1; // Fallback
        }

        public static double[] SelectParent(List<(double[], double)> population, RankSelection selectionMethod)
        {
            List<(double[], double)> sortedPopulation = population.OrderBy(individual => individual.Item2).ToList();
            int rankSum = sortedPopulation.Count * (sortedPopulation.Count + 1) / 2;
            double randomValue = m_Random.NextDouble() * rankSum;
            double cumulativeRank = 0.0;

            for (int i = 0; i < sortedPopulation.Count; i++)
            {
                cumulativeRank += i + 1;
                if (cumulativeRank >= randomValue)
                    return sortedPopulation[i].Item1;
            }

            return sortedPopulation.Last().Item1; // Fallback
        }

        public static double[] SelectParent(List<(double[], double)> population, TournamentSelection selectionMethod)
        {
            int tournamentSize = selectionMethod.TournamentSize;
            List<(double[], double)> tournament = new List<(double[], double)>();
            for (int i = 0; i < tournamentSize; i++)
            {
                int randomIndex = m_Random.Next(population.Count);
                tournament.Add(population[randomIndex]);
            }

            return tournament.OrderByDescending(individual => individual.Item2).First().Item1;
        }


        public static double[] SelectParent(List<(double[], double)> population, IParentSelectionMethod selectionMethod)
        {
            throw new NotImplementedException();
        }

        public static double[] ISelectSecondParent(List<(double[], double)> population, double[] firstParent, ISecondParentSelectionMethod method)
        {
            return SelectSecondParent(population, firstParent, method as dynamic);
        }

        public static double[] SelectSecondParent(List<(double[], double)> population, double[] firstParent, DistanceBasedSelection method)
        {
            List<(double[], double)> distances = population.Select(individual => (individual.Item1, Math.Sqrt(individual.Item1.Zip(firstParent, (x, y) => (x - y) * (x - y)).Sum())))
                            .OrderBy(individual => individual.Item2).ToList();

            int start = (int)(Math.Round(population.Count * method.MinPercentile));
            int end = (int)(Math.Round(population.Count * method.MaxPercentile));

            List<(double[], double)> candidates = distances.Skip(start).Take(end - start).ToList();
            return candidates[m_Random.Next(candidates.Count)].Item1;
        }

        public static double[] SelectSecondParent(List<(double[], double)> population, double[] firstParent, IParentSelectionMethod method)
        {
            return SelectParent(population, method as dynamic);
        }

        private static (double[], double[]) ICrossover(double[] parent1, double[] parent2, ICrossoverMethod crossoverMethod)
        {
            return Crossover(parent1, parent2, crossoverMethod as dynamic);
        }

        public static (double[], double[]) Crossover(double[] parent1, double[] parent2, SinglePointCrossover crossoverMethod)
        {
            //TODO: rounding, min max etc.

            if (m_Random.NextDouble() > crossoverMethod.CrossoverRate)
                return (parent1.ToArray(), parent2.ToArray());

            int crossoverPoint = m_Random.Next(parent1.Length);
            double[] child1 = new double[parent1.Length];
            double[] child2 = new double[parent1.Length];

            for (int i = 0; i < parent1.Length; i++)
            {
                if (i < crossoverPoint)
                {
                    child1[i] = parent1[i];
                    child2[i] = parent2[i];
                }
                else
                {
                    child1[i] = parent2[i];
                    child2[i] = parent1[i];
                }
            }

            return (child1, child2);
        }

        public static (double[], double[]) Crossover(double[] parent1, double[] parent2, TwoPointCrossover crossoverMethod)
        {
            if (m_Random.NextDouble() > crossoverMethod.CrossoverRate)
                return (parent1.ToArray(), parent2.ToArray());

            int point1 = m_Random.Next(parent1.Length);
            int point2 = m_Random.Next(point1, parent1.Length);
            double[] child1 = new double[parent1.Length];
            double[] child2 = new double[parent1.Length];

            for (int i = 0; i < parent1.Length; i++)
            {
                if (i < point1 || i > point2)
                {
                    child1[i] = parent1[i];
                    child2[i] = parent2[i];
                }
                else
                {
                    child1[i] = parent2[i];
                    child2[i] = parent1[i];
                }
            }

            return (child1, child2);
        }

        public static (double[], double[]) Crossover(double[] parent1, double[] parent2, UniformCrossover crossoverMethod)
        {
            if (m_Random.NextDouble() > crossoverMethod.CrossoverRate)
                return (parent1.ToArray(), parent2.ToArray());

            double[] child1 = new double[parent1.Length];
            double[] child2 = new double[parent1.Length];

            double probability = crossoverMethod.Probability;
            for (int i = 0; i < parent1.Length; i++)
            {
                if (m_Random.NextDouble() < probability)
                {
                    child1[i] = parent1[i];
                    child2[i] = parent2[i];
                }
                else
                {
                    child1[i] = parent2[i];
                    child2[i] = parent1[i];
                }
            }

            return (child1, child2);
        }

        public static (double[], double[]) Crossover(double[] parent1, double[] parent2, ArithmeticCrossover crossoverMethod)
        {
            if (m_Random.NextDouble() > crossoverMethod.CrossoverRate)
                return (parent1.ToArray(), parent2.ToArray());

            double[] child1 = new double[parent1.Length];
            double[] child2 = new double[parent1.Length];

            double alpha = crossoverMethod.Alpha;
            for (int i = 0; i < parent1.Length; i++)
            {
                double minAllowed = m_Parameters[i].Min;
                double maxAllowed = m_Parameters[i].Max;
                double step = m_Parameters[i].Step;

                double new1 = (alpha * parent1[i] + (1 - alpha) * parent2[i]).Round(step);
                double new2 = (alpha * parent2[i] + (1 - alpha) * parent1[i]).Round(step);
                child1[i] = Math.Max(Math.Min(new1, maxAllowed), minAllowed);
                child2[i] = Math.Max(Math.Min(new2, maxAllowed), minAllowed);
            }

            return (child1, child2);
        }

        public static (double[], double[]) Crossover(double[] parent1, double[] parent2, ProportionalArithmeticCrossover crossoverMethod)
        {
            if (m_Random.NextDouble() > crossoverMethod.CrossoverRate)
                return (parent1.ToArray(), parent2.ToArray());

            double f1 = m_ResultPool[parent1];
            double f2 = m_ResultPool[parent2];
            double alpha = f1 / (f1 + f2);

            double[] child1 = new double[parent1.Length];
            double[] child2 = new double[parent1.Length];

            for (int i = 0; i < parent1.Length; i++)
            {
                double minAllowed = m_Parameters[i].Min;
                double maxAllowed = m_Parameters[i].Max;
                double step = m_Parameters[i].Step;

                double new1 = (alpha * parent1[i] + (1 - alpha) * parent2[i]).Round(step);
                double new2 = (alpha * parent2[i] + (1 - alpha) * parent1[i]).Round(step);
                child1[i] = Math.Max(Math.Min(new1, maxAllowed), minAllowed);
                child2[i] = Math.Max(Math.Min(new2, maxAllowed), minAllowed);
            }

            return (child1, child2);
        }

        public static (double[], double[]) Crossover(double[] parent1, double[] parent2, BlendCrossover crossoverMethod)
        {
            if (m_Random.NextDouble() > crossoverMethod.CrossoverRate)
                return (parent1.ToArray(), parent2.ToArray());

            double[] child1 = new double[parent1.Length];
            double[] child2 = new double[parent1.Length];

            double alpha = crossoverMethod.Alpha;
            for (int i = 0; i < parent1.Length; i++)
            {
                double minAllowed = m_Parameters[i].Min;
                double maxAllowed = m_Parameters[i].Max;
                double step = m_Parameters[i].Step;

                double min = Math.Min(parent1[i], parent2[i]);
                double max = Math.Max(parent1[i], parent2[i]);
                double range = max - min;
                double lowerBound = min - alpha * range;
                double upperBound = max + alpha * range;
                double new1 = (lowerBound + m_Random.NextDouble() * (upperBound - lowerBound)).Round(step);
                double new2 = (lowerBound + m_Random.NextDouble() * (upperBound - lowerBound)).Round(step);
                child1[i] = Math.Max(Math.Min(new1, maxAllowed), minAllowed);
                child2[i] = Math.Max(Math.Min(new2, maxAllowed), minAllowed);
            }

            return (child1, child2);
        }

        public static (double[], double[]) Crossover(double[] parent1, double[] parent2, SimulatedBinaryCrossover crossoverMethod)
        {
            if (m_Random.NextDouble() > crossoverMethod.CrossoverRate)
                return (parent1.ToArray(), parent2.ToArray());

            double[] child1 = new double[parent1.Length];
            double[] child2 = new double[parent1.Length];

            double eta = crossoverMethod.Eta;
            for (int i = 0; i < parent1.Length; i++)
            {
                double minAllowed = m_Parameters[i].Min;
                double maxAllowed = m_Parameters[i].Max;
                double step = m_Parameters[i].Step;

                double u = m_Random.NextDouble();
                double beta;

                if (u <= 0.5)
                    beta = Math.Pow(2 * u, 1.0 / (eta + 1));
                else
                    beta = Math.Pow(1 / (2 * (1 - u)), 1.0 / (eta + 1));

                double new1 = 0.5 * ((1 + beta) * parent1[i] + (1 - beta) * parent2[i]);
                double new2 = 0.5 * ((1 - beta) * parent1[i] + (1 + beta) * parent2[i]);
                child1[i] = Math.Max(Math.Min(new1, maxAllowed), minAllowed);
                child2[i] = Math.Max(Math.Min(new2, maxAllowed), minAllowed);
            }

            return (child1, child2);
        }


        public static (double[], double[]) Crossover(double[] parent1, double[] parent2, ICrossoverMethod crossoverMethod)
        {
            throw new NotImplementedException();
        }

        private static void IMutate(double[] chromosome, IMutationMethod mutationMethod, DoubleParameter[] parameters)
        {
            Mutate(chromosome, mutationMethod as dynamic, parameters);
        }

        public static void Mutate(double[] individual, RandomPointMutation mutationMethod, DoubleParameter[] parameters)
        {
            double mutationRate = mutationMethod.MutationRate;

            //TODO: this may get really slow if most combinations already covered - need to think how to avoid gridlock
            do
            {
                for (int i = 0; i < individual.Length; i++)
                {
                    if (m_Random.NextDouble() < mutationRate)
                    {
                        double min = parameters[i].Min;
                        double max = parameters[i].Max;
                        double step = parameters[i].Step;
                        individual[i] = min + (m_Random.NextDouble() * (max - min)).Round(step);
                    }
                }
            }
            while (m_ResultPool.ContainsKey(individual));
        }

        public static void Mutate(double[] individual, GaussianPointMutation mutationMethod, DoubleParameter[] parameters)
        {
            double mutationRate = mutationMethod.MutationRate;
            int intervalCount = mutationMethod.IntervalCount;

            //TODO: this may get really slow if most combinations already covered - need to think how to avoid gridlock
            do
            {
                for (int i = 0; i < individual.Length; i++)
                {
                    if (m_Random.NextDouble() < mutationRate)
                    {
                        double min = parameters[i].Min;
                        double max = parameters[i].Max;
                        double step = parameters[i].Step;
                        double x = individual[i];

                        // Calculate the standard deviation for the Gaussian distribution
                        double sigma = (max - min) / intervalCount;

                        // Generate a Gaussian-distributed random number with mean 0 and standard deviation sigma
                        double gaussianMutation = NextGaussian(0, sigma);

                        // Apply the mutation
                        double newValue = x + gaussianMutation;

                        // Clamp the new value to the [min, max] range
                        newValue = Math.Max(min, Math.Min(max, newValue));

                        // Round to the nearest step size
                        individual[i] = newValue.Round(step);
                    }
                }
            }
            while (m_ResultPool.ContainsKey(individual));
        }

        public static void Mutate(double[] individual, GleamPointMutation mutationMethod, DoubleParameter[] parameters)
        {
            double mutationRate = mutationMethod.MutationRate;
            int k = mutationMethod.IntervalCount;

            //TODO: this may get really slow if most combinations already covered - need to think how to avoid gridlock
            do
            {
                for (int i = 0; i < individual.Length; i++)
                {
                    if (m_Random.NextDouble() < mutationRate)
                    {
                        double min = parameters[i].Min;
                        double max = parameters[i].Max;
                        double step = parameters[i].Step;
                        double x = individual[i];

                        // Decide whether to increase or decrease
                        bool increase = m_Random.NextDouble() < 0.5;

                        if (increase)
                        {
                            double delta = (max - x) / k;
                            int subInterval = m_Random.Next(1, k + 1); // Select a sub-interval
                            double lowerBound = x;
                            double upperBound = x + delta * subInterval;
                            double newValue = lowerBound + m_Random.NextDouble() * (upperBound - lowerBound);
                            newValue = Math.Min(max, newValue); // Clamp to max
                            individual[i] = newValue.Round(step); // Round to nearest step
                        }
                        else
                        {
                            double delta = (x - min) / k;
                            int subInterval = m_Random.Next(1, k + 1); // Select a sub-interval
                            double lowerBound = x - delta * subInterval;
                            double upperBound = x;
                            double newValue = lowerBound + m_Random.NextDouble() * (upperBound - lowerBound);
                            newValue = Math.Max(min, newValue); // Clamp to min
                            individual[i] = newValue.Round(step); // Round to nearest step
                        }
                    }
                }
            }
            while (m_ResultPool.ContainsKey(individual));
        }

        private static double NextGaussian(double mean, double stddev)
        {
            // Use Box-Muller transform to generate a standard normal distribution
            double u1 = 1.0 - m_Random.NextDouble(); // Uniform(0,1] random doubles
            double u2 = 1.0 - m_Random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // Random normal(0,1)
            return mean + stddev * randStdNormal; // Random normal(mean,stddev)
        }

        private static void Mutate(double[] chromosome, IMutationMethod mutationMethod, double mutationRate, DoubleParameter[] parameters)
        {
            throw new NotImplementedException();
        }
    }

    internal class DoubleArrayComparer : IEqualityComparer<double[]>
    {
        public bool Equals(double[] x, double[] y)
        {
            if (x == null || y == null)
                return x == y;

            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                    return false;
            }

            return true;
        }

        public int GetHashCode(double[] obj)
        {
            if (obj == null)
                return 0;

            int hash = 17;
            foreach (double d in obj)
            {
                hash = hash * 31 + d.GetHashCode();
            }

            return hash;
        }
    }
}