using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References
    public JobManager _jobManager; //Reference to the JobManager
    public GameManager _gameManager;//Reference to the GameManager
    #endregion

    public float _age; // The age of this worker
    public float _happiness = 1.0f; // The happiness of this worker
    public int ageTimeSeconds;
    public float resourcesPerAge; //should be pretty low (like < 0.01) otherwise there is way more consumption than production

    PhaseOfLife phaseOfLife = PhaseOfLife.Child;

    #region Enumerations
    enum PhaseOfLife { Child, Worker, Old };
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Age", ageTimeSeconds, ageTimeSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void Age()
    {
        //TODO: Implement a life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.

        if (_age == 14)
        {
            BecomeOfAge();
        }

        if (_age == 64)
        {
            Retire();
        }

        if (_age == 100)
        {
            Die();
        }

        _age++;


        List<bool> happynessFactors = new List<bool>();

        // get fish, clothes and schnapps
        bool gotFish = _gameManager.TakeResourceFromWareHouse(GameManager.ResourceTypes.Fish, resourcesPerAge);
        happynessFactors.Add(gotFish);

        bool gotClothes = _gameManager.TakeResourceFromWareHouse(GameManager.ResourceTypes.Clothes, resourcesPerAge);
        happynessFactors.Add(gotClothes);

        if(phaseOfLife != PhaseOfLife.Child)
        {
            bool gotSchnapps = _gameManager.TakeResourceFromWareHouse(GameManager.ResourceTypes.Schnapps, resourcesPerAge);
            happynessFactors.Add(gotSchnapps);
        }

        if (phaseOfLife == PhaseOfLife.Worker)
        {
            bool hasJob = _jobManager.hasJob(this);
            happynessFactors.Add(hasJob);

            // drink another schnapps because it's boring without a job :D 
            if (!hasJob)
            {
                _gameManager.TakeResourceFromWareHouse(GameManager.ResourceTypes.Schnapps, resourcesPerAge);
            }
        }

        _happiness = calcHappyness(happynessFactors);
    }

    private float calcHappyness(List<bool> happynessFactors)
    {
        float happyNess = 0;
        float happyNessWeight = 1.0f / happynessFactors.Count;

        foreach(bool factor in happynessFactors)
        {
            if (factor)
                happyNess += happyNessWeight;
        }

        return happyNess;
    }


    public void BecomeOfAge()
    {
        _jobManager.RegisterWorker(this);
        phaseOfLife = PhaseOfLife.Worker;
    }

    private void Retire()
    {
        _jobManager.RemoveWorker(this);
        phaseOfLife = PhaseOfLife.Old;
    }

    private void Die()
    {
        Destroy(this.gameObject, 1f);
    }
}
