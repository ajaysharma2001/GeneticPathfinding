using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    public List<Vector2> genes = new List<Vector2>();

    // Initial randam genome 
    public DNA(int genomeLength = 50) {
        for (int i = 0; i < genomeLength; i++) {
            genes.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
        }
    }

    // Creating genome for child (add possibile decaying mutationRate) 
    public DNA(DNA parent, DNA partner, float mutationRate = 0.01f) {
        for (int i = 0; i < parent.genes.Count; i++) {
            float mutationChance = Random.Range(0.0f, 1.0f);
            if (mutationChance <= mutationRate)
            {
                // Add random gene
                genes.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
            }
            else {
                // generate a 50% change of 0 or 1 (chance parent or partner gene is passed on)
                int chance = Random.Range(0, 2);
                if (chance == 0)
                {
                    genes.Add(parent.genes[i]);
                }
                else
                {
                    genes.Add(partner.genes[i]);
                }
            }
        }
    }


}
