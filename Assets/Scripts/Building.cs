using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    #region Attributes
    public BuildingTypes buildingType;
    public int upkeepCost; // The money cost per minute
    public int buildCostMoney; // placement money cost
    public int buildCostPlanks; // placement planks cost
    public Tile.TileTypes[] compatibleTileTypes; // A restriction on which types of tiles it can be placed on

    public GameManager gameManager;
    public Tile tileBuildOn; // Reference to the tile it is built on
    #endregion

    #region Enumerations
    public enum BuildingTypes { Fishery, LumberJack };
    #endregion

    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    #endregion
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    #endregion
    

    #region Methods   
    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }
    #endregion
}
