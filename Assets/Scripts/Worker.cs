using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;//Reference to the GameManager
    #endregion

    public float _age; // The age of this worker
    public float _happiness = 1.0f; // The happiness of this worker

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Age", 15, 15);
    }

    // Update is called once per frame
    void Update()
    {
        Age();
    }


    private void Age()
    {
        //TODO: Implement a life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.

        if (_age > 14)
        {
            BecomeOfAge();
        }

        if (_age > 64)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
        }

        _age++;


        // get fish, clothes and schnapps, (TODO: maybe children should not drink schnapps...)
        bool gotFish = _gameManager.TakeResourceFromWareHouse(GameManager.ResourceTypes.Fish);
        bool gotClothes = _gameManager.TakeResourceFromWareHouse(GameManager.ResourceTypes.Clothes);
        bool gotSchnapps = _gameManager.TakeResourceFromWareHouse(GameManager.ResourceTypes.Schnapps);
        bool hasJob = true; //TODO

        List<bool> happynessFactors = new List<bool>();
        happynessFactors.Add(gotFish);
        happynessFactors.Add(gotClothes);
        happynessFactors.Add(gotSchnapps);
        happynessFactors.Add(hasJob);

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
    }

    private void Retire()
    {
        _jobManager.RemoveWorker(this);
    }

    private void Die()
    {
        Destroy(this.gameObject, 1f);
    }
}
