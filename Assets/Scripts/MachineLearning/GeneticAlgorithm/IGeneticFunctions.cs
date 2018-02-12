
namespace MachineLearning.GeneticAlgorithm
{
    public interface IGeneticFunctions<T>
    {
        double ComputeFitness(T individual);
        T CreateIndividual();
        T Mutate(T individual, double mutationRate);
        Tuple<T, T> SelectParents(T[] individuals, double[] fitnesses);
        Tuple<T, T> Crossover(Tuple<T, T> parents);
    }
}