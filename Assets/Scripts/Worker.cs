using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Assertions;
using Vector3 = UnityEngine.Vector3;

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

    public Tile hometile = null;
    public Tile worktile = null;
    public Vector3 homePosition;
    private Tile currentTile = null;
    private Tile nextTile = null;

    private bool working = false;
    private bool dudeWalking = false;

    private bool onWayToWork = true;
    private int currentWayTileIndex = 0;
    List<Tile> wayToWork;

    public float walkSpeed = 2.0f;

    #region Enumerations
    enum PhaseOfLife { Child, Worker, Old };
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Age", ageTimeSeconds, ageTimeSeconds);
    }

    void Update()
    {
        if (worktile != null)
        {
            if (!working)
            {
                working = true;
                dudeWalking = true;
                currentTile = hometile;
                //nextTile = worktile; //TODO: navigationmanager.getNextTile (currentTile, destTile)

                transform.position = hometile.transform.position;

                wayToWork = _gameManager.navigationManager.getTilePathTo(currentTile, worktile, _gameManager._tileMap);
                Assert.IsTrue(wayToWork.Count > 0);

                currentWayTileIndex = 1;
                nextTile = wayToWork[currentWayTileIndex];
            }

        }
        else
        {
            if (working)
            {
                working = false;
            }

        }

        if (dudeWalking)
        {                
            float speed = walkSpeed * Time.deltaTime;
            Vector3 sourcePosition = new Vector3(transform.position.x, currentTile.transform.position.y, transform.position.z);
            Vector3 destPosition = new Vector3( nextTile.transform.position.x, currentTile.transform.position.y , nextTile.transform.position.z);
            Vector3 currentPosition = Vector3.MoveTowards(sourcePosition, destPosition, speed);

            float distance = Vector3.Distance(currentTile.transform.position, destPosition);
            float distanceLeft = Vector3.Distance(currentPosition, destPosition);
            float distanceProgress = distanceLeft / distance;

            float jumpBorderValue = (currentTile.transform.position.y > nextTile.transform.position.y) ? 0.4f : 0.6f;
            if (distanceProgress < jumpBorderValue)
            {
                transform.position = new Vector3(currentPosition.x, nextTile.transform.position.y, currentPosition.z);
            }
            else
            {
                transform.position = currentPosition;
            }

            //reached nextTile ?   
            if (transform.position == nextTile.transform.position)
            {
                
                // reached worktile?
                if(nextTile == worktile)
                {
                    onWayToWork = false;
                }
                else if(nextTile == hometile)
                {
                    onWayToWork = true;
                }

                if(onWayToWork)
                    currentWayTileIndex += 1;
                else
                    currentWayTileIndex -= 1;

                nextTile = wayToWork[currentWayTileIndex];
               
            }
        }
    }



    private void Age()
    {
        //TODO: Implement a life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.

        if (_age == 2)
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
