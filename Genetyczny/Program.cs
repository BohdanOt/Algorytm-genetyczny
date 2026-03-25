using System;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static Random rand = new Random();

    const int CHROMOSOME_LENGTH = 11;
    const int POP_SIZE = 50;
    const int GENERATIONS = 200;
    const double CROSS_PROB = 0.7;
    const double MUT_PROB = 0.01;

    static void Main()
    {
        var population = InitPopulation();

        //Śledzenie najlepszego rozwiązania
        string bestChromosomeEver = "";
        double bestFitnessEver = double.MinValue;
        int bestGeneration = 0;

        for (int gen = 0; gen < GENERATIONS; gen++)
        {
            var fitness = population.Select(Fitness).ToList();

            Console.WriteLine($"Generacja {gen} | Średnia: {fitness.Average():F4} | Max: {fitness.Max():F4}");

            //Sprawdzamy najlepszy w tej generacji
            double maxFitness = fitness.Max();
            int bestIndex = fitness.IndexOf(maxFitness);

            if (maxFitness > bestFitnessEver)
            {
                bestFitnessEver = maxFitness;
                bestChromosomeEver = population[bestIndex];
                bestGeneration = gen;
            }

            var newPopulation = new List<string>();

            while (newPopulation.Count < POP_SIZE)
            {
                var parent1 = Selection(population, fitness);
                var parent2 = Selection(population, fitness);

                var (child1, child2) = Crossover(parent1, parent2);

                child1 = Mutation(child1);
                child2 = Mutation(child2);

                newPopulation.Add(child1);
                newPopulation.Add(child2);
            }

            population = newPopulation.Take(POP_SIZE).ToList();
        }

        //Wynik (najlepszy ze wszystkich generacji)
        double bestX = Decode(bestChromosomeEver);

        Console.WriteLine("\n**WYNIK** ");
        Console.WriteLine($"Najlepsza generacja: {bestGeneration}");
        Console.WriteLine($"Chromosom: {bestChromosomeEver}");
        Console.WriteLine($"x = {bestX:F3}");
        Console.WriteLine($"f(x) = {bestFitnessEver:F6}");
    }

    //Inicjalizacja
    static List<string> InitPopulation()
    {
        var pop = new List<string>();

        for (int i = 0; i < POP_SIZE; i++)
        {
            string chromosome = "";
            for (int j = 0; j < CHROMOSOME_LENGTH; j++)
                chromosome += rand.Next(2);

            pop.Add(chromosome);
        }

        return pop;
    }

  //Dekodowanie
   static double Decode(string chrom)
    {
        int value = Convert.ToInt32(chrom, 2);
        int max = (1 << CHROMOSOME_LENGTH) - 1;
        return 1.5 + (3.5 - 1.5) * value / max;
    }

   //Funkcja celu
    static double Fitness(string chrom)
    {
        double x = Decode(chrom);
        return 1 + ((Math.Exp(x) * Math.Sin(Math.PI * x) - 1) / x);
    }

    //Selekcja ruletkowa
    static string Selection(List<string> pop, List<double> fitness)
    {
        double sum = fitness.Sum();
        double r = rand.NextDouble() * sum;

        double acc = 0;
        for (int i = 0; i < pop.Count; i++)
        {
            acc += fitness[i];
            if (acc >= r)
                return pop[i];
        }

        return pop.Last();
    }

    //Krzyżowanie jednopunktowe
    static (string, string) Crossover(string p1, string p2)
    {
        if (rand.NextDouble() > CROSS_PROB)
            return (p1, p2);

        int point = rand.Next(1, CHROMOSOME_LENGTH - 1);

        string c1 = p1.Substring(0, point) + p2.Substring(point);
        string c2 = p2.Substring(0, point) + p1.Substring(point);

        return (c1, c2);
    }

    //Mutacja
    static string Mutation(string chrom)
    {
        char[] genes = chrom.ToCharArray();

        for (int i = 0; i < genes.Length; i++)
        {
            if (rand.NextDouble() < MUT_PROB)
                genes[i] = genes[i] == '0' ? '1' : '0';
        }

        return new string(genes);
    }
}