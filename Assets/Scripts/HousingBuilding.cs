using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{

    #region Attributes
    public float workerGenerationIntervalSeconds;
    public int startWorkersAmount;
    public int maxWorkersAmount;
    public int upkeepCostPerWorker;

    public GameObject workerPrefab;

    public float[] workerPositionsX;
    public float[] workerPositionsZ;

    Worker[] residents;
    Vector3[] residentSpotPositions;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        residents = new Worker[maxWorkersAmount];

        //todo refactor: simply define in unity instead of this weird "algorithm"
        residentSpotPositions = new Vector3[maxWorkersAmount];
        for(int i = 0; i < residentSpotPositions.Length; i++)
        {
            int x = i / 2;
            int z = i % 2;
            residentSpotPositions[i] = new Vector3(workerPositionsX[x], 0.0f, workerPositionsZ[z]);
        }

        for (int i = 0; i < startWorkersAmount; i++)
        {
            generateWorker();
        }

        Invoke("generationLoop", workerGenerationIntervalSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override int getUpkeepCost()
    {
        int nWorkers = 0;     
        foreach (Worker worker in residents)
        {
            if (worker != null)
            {
                nWorkers++;
            }
        }

        return nWorkers * upkeepCostPerWorker;
    }

    private void generationLoop()
    {
        generateWorker();

        // recalc efficiency for next invoke
        float efficiency = calcEfficiency();
        if(efficiency > 0)
        {
            float efficiencyInterval = workerGenerationIntervalSeconds * (1.0f / efficiency);
            Invoke("generationLoop", efficiencyInterval);
        }
        else
        {
            Invoke("generationLoop", workerGenerationIntervalSeconds);
        }
        
    }


    private void generateWorker()
    {
        int residentIndex = findFreeResidentSpot();
        if (residentIndex >= 0)
        {
            //TODO set to random position in tile(kann ausgelagert werden in building, placeworker)
            Vector3 position = this.transform.position + workerPrefab.transform.position + residentSpotPositions[residentIndex];
            //TODO position 
            GameObject workerObject = Instantiate(workerPrefab, position, Quaternion.identity);
            Worker worker = workerObject.GetComponent<Worker>();
            _workers.Add(worker);
            residents[residentIndex] = worker;
        }

    }

    private float calcEfficiency()
    {
        float efficiencysum = 0;
        int nWorkers = 0;

        foreach (Worker worker in residents)
        {
            if(worker != null)
            {
                efficiencysum += worker._happiness;
                nWorkers++;
            }
        }

        return efficiencysum / nWorkers;
    }


    private int findFreeResidentSpot()
    {
        for(int i = 0; i < residents.Length; i++)
        {
            if (residents[i] == null)
                return i;
        }

        return -1;
    }
}
