using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    #region Attributes
    public int resourceGenerationIntervalSeconds; // If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    public int outputCountPerCycle; //  The number of output resources per generation cycle (for example the Sawmill produces 2 planks at a time)
    public GameManager.ResourceTypes inputResourceType; // A choice for input resource types (0, 1 or 2 types)
    public GameManager.ResourceTypes outputResourceType; // A choice for output resource type 
    public Tile.TileTypes efficiencyTileType; // A choice if its efficiency scales with a specific type of surrounding tile
    public int maxNumEfficiencyNeighbours;  // The minimum and maximum number of surrounding tiles its efficiency scales with (0-6)
    public int numAvailableJobs;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        float efficiency = calcEfficiency();
        
        if(efficiency > 0)
        {
            resourceGenerationIntervalSeconds = Convert.ToInt32((double)resourceGenerationIntervalSeconds * (1.0 / efficiency));
            InvokeRepeating("processResources", 0, resourceGenerationIntervalSeconds);
        }
         
        for(int i = 0; i < numAvailableJobs; i++)
        {
            _jobManager.registerJob(new Job(this));
        }
    }

    private float calcEfficiency()
    {
        if(efficiencyTileType == Tile.TileTypes.Empty)
            return 1;

        int nEfficiencyNeighbours = 0;
        foreach(Tile neighbourTile in tileBuildOn._neighborTiles)
        {
            if (neighbourTile._type == efficiencyTileType)
                nEfficiencyNeighbours++;
        }

        if (nEfficiencyNeighbours == 0)
            return 0;

        if (nEfficiencyNeighbours >= maxNumEfficiencyNeighbours)
            return 1;

        return (float)nEfficiencyNeighbours / (float)maxNumEfficiencyNeighbours;
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void processResources()
    {
        if(getInputResources())
        {
            float workerEfficiency = calcWorkerEfficiency();
            workerEfficiency = Mathf.Max(workerEfficiency, 0.5f); //otherwise the game progresses really slow...

            float generetedResources = outputCountPerCycle * workerEfficiency;
            gameManager.PutResourceInWareHouse(outputResourceType, generetedResources);
        }
        
    }

    private float calcWorkerEfficiency()
    {
        int nWorkers = 0;
        float happinessSum = 0;

        foreach(Worker w in _workers)
        {
            nWorkers++;
            happinessSum += w._happiness;
        }

        if (nWorkers == 0)
            return 0;

        float happinessAverage = happinessSum / nWorkers;
        float manPower = (float)nWorkers / (float)numAvailableJobs;

        return manPower * happinessAverage;
    }

    private bool getInputResources()
    {
        if(inputResourceType == GameManager.ResourceTypes.None)
        {
            return true;
        }

        return gameManager.TakeResourceFromWareHouse(inputResourceType, 1);
    }
}
