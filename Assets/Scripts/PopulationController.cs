using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    List<GeneticPathfinder> population = new List<GeneticPathfinder>();
    public GameObject creaturePrefab;
    public int populationSize = 100;
    public int genomeLength;
    public float cutoff = 0.3f;
    public int survivorKeep = 5;
    [Range(0f, 1f)] public float mutationRate = 0.01f;
    public Transform spawnPoint;
    public Transform end;

    // Creats starting population with random genome 
    void InitPopulation()
    {
        for(int i = 0; i < populationSize; i++)
        {
            GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
            go.GetComponent<GeneticPathfinder>().InitCreature(new DNA(genomeLength), end.position);
            population.Add(go.GetComponent<GeneticPathfinder>());

        }
    }

    void NextGeneration()
    {
        int survivorsCut = Mathf.RoundToInt(populationSize * cutoff);
        List<GeneticPathfinder> survivors = new List<GeneticPathfinder>();

        // Adds the best creatures from the generation based on fitness to surviors list
        for(int i = 0; i < survivorsCut; i++)
        {
            survivors.Add(GetFittest());
        }

        // Destory all instantiated game objects and clears population list
        for (int i = 0; i < population.Count; i++)
        {
            Destroy(population[i].gameObject);
        }
        population.Clear();

        // Carries over the best survivors to the next generation
        for(int i = 0; i < survivorKeep; i++)
        {
            GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
            go.GetComponent<GeneticPathfinder>().InitCreature(survivors[i].dna, end.position);
            population.Add(go.GetComponent<GeneticPathfinder>());
        }

        // Creates the rest of the population with breeding of the survivors
        while (population.Count < populationSize)
        {
            for(int i = 0; i < survivors.Count; i++)
            {
                GameObject go = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
                // uses DNA from the ith survivor and the top 10 fittest
                go.GetComponent<GeneticPathfinder>().InitCreature(new DNA(survivors[i].dna, survivors[Random.Range(0, 10)].dna, mutationRate), end.position);
                population.Add(go.GetComponent<GeneticPathfinder>());

                if (population.Count >= populationSize)
                {
                    break;
                }
            }
        }

        // Destories teh survivors
        for(int i = 0; i < survivors.Count; i++)
        {
            Destroy(survivors[i].gameObject);
        }
    }

    private void Start()
    {
        InitPopulation();
    }

    private void Update()
    {
        if (!HasActive())
        {
            NextGeneration();
        }
    }

    // Finds the fittest creature in the current generation and removes it from the population
    GeneticPathfinder GetFittest()
    {
        float maxFitness = float.MinValue;
        int index = 0;
        
        for(int i = 0; i < population.Count; i++)
        {
            if (population[i].fitness > maxFitness)
            {
                maxFitness = population[i].fitness;
                index = i;
            }
        }
        GeneticPathfinder fittest = population[index];
        population.Remove(fittest);
        return fittest;
    }

    // Returns true if there are still creatures alive and false otherwise  
    bool HasActive()
    {
        for(int i = 0; i < population.Count; i++)
        {
            if (!population[i].hasFinished)
            {
                return true;
            }
        }
        return false;
    }
}
