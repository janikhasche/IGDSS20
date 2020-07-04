using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class JobManager : MonoBehaviour
{

    private List<Job> _availableJobs = new List<Job>();
    private List<Job> _allJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();



    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleUnoccupiedWorkers();
    }
    #endregion


    #region Methods

    private void HandleUnoccupiedWorkers()
    {
        if (_unoccupiedWorkers.Count > 0)
        {

            //TODO: What should be done with unoccupied workers? 
            //-> handled in other functions

        }
    }

    public void RegisterWorker(Worker w)
    {
        //try to assign directly
        if(_availableJobs.Count > 0)
        {
            Job job = _availableJobs[0];
            _availableJobs.RemoveAt(0);
            job.AssignWorker(w);
        }
        else
        {
            _unoccupiedWorkers.Add(w);
        }
    }



    public void RemoveWorker(Worker w)
    {
        bool workerWasUnoccupied = _unoccupiedWorkers.Remove(w);
        
        if(!workerWasUnoccupied)
        {
            Job workersJob = _allJobs.Find(job => job._worker == w);
            Assert.IsNotNull(workersJob);
            workersJob.RemoveWorker(w);
            _availableJobs.Add(workersJob);
        }
    }


    public void registerJob(Job j)
    {
        _allJobs.Add(j);

        if (_unoccupiedWorkers.Count > 0)
        {
            Worker worker = _unoccupiedWorkers[0];
            _unoccupiedWorkers.RemoveAt(0);
            j.AssignWorker(worker);
        }
        else
        {
            _availableJobs.Add(j);
        }
    }


    public bool hasJob(Worker w)
    {
        return !_unoccupiedWorkers.Contains(w);
    }

    #endregion
}
