using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NavigationManager
{
    class PotentialFields
    {
        public PotentialFields(int size)
        {
            p = new double[size, size];
        }

        public double[,] p;
    }

    private double[,] weightMap;
    private int mapSize;
    private PotentialFields[,] pFields; //potential fields for each destination

    public NavigationManager(double[,] weightMap, int mapSize)
    {
        this.weightMap = weightMap;
        this.mapSize = mapSize;

        pFields = new PotentialFields[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                initMapForDest(new Vector2Int(x, y));
            }
        }
    }

    public List<Tile> getTilePathTo(Tile src, Tile dst, Tile[,] tileMap)
    {
        //wrapper for getPathTo
        List<Tile> tileList = new List<Tile>();

        Vector2Int srcVec = new Vector2Int(src._coordinateWidth, src._coordinateHeight);
        Vector2Int dstVec = new Vector2Int(dst._coordinateWidth, dst._coordinateHeight);
        List<Vector2Int> vecList = getPathTo(srcVec, dstVec);

        tileList.Add(src);
        foreach(Vector2Int vec in vecList)
        {
            tileList.Add(tileMap[vec.x, vec.y]);
        }
        return tileList;
    }

    public List<Vector2Int> getPathTo(Vector2Int src, Vector2Int dst)
    {
        int loopCount = 0;
        List<Vector2Int> path = new List<Vector2Int>();

        bool[,] visited = new bool[mapSize, mapSize];

        //copy to temp field
        PotentialFields pField = new PotentialFields(mapSize);
        pField.p = pFields[dst.x, dst.y].p.Clone() as double[,];

        Vector2Int pos = src;

        while(pos != dst)
        {
            loopCount++;
            Assert.IsTrue(loopCount < 10000); //seems to be in endless loop :(

            List<Vector2Int> neighBours = getNeighbours(pos);
            Vector2Int smallestWeightPos = getSmallestWeightPos(neighBours, pField.p);
            
            double smallestWeight = pField.p[smallestWeightPos.x, smallestWeightPos.y];
            double currentWeight = pField.p[pos.x, pos.y];

            // local minima trap:
            // was already visited, increase weight and restart algorithm
            if (visited[pos.x, pos.y] || (currentWeight < smallestWeight))
            {
               pField.p[pos.x, pos.y] = smallestWeight + 0.1;

                path.Clear();
                pos = src;
                visited = new bool[mapSize, mapSize];
            }
            else
            {
                visited[pos.x, pos.y] = true;

                pos = smallestWeightPos;
                path.Add(pos);
            }
        }


        return path;
    }

    private Vector2Int getSmallestWeightPos(List<Vector2Int> poss, double[,] pField)
    {
        double smallestWeight = double.PositiveInfinity;
        Vector2Int smallestPos = new Vector2Int(-1, -1);

        foreach(Vector2Int pos in poss)
        {
            double curWeight = pField[pos.x, pos.y];
            if(curWeight < smallestWeight)
            {
                smallestWeight = curWeight;
                smallestPos = pos;
            }
        }

        //Assert.That(smallestPos.x != -1);
        return smallestPos;
    }

    private List<Vector2Int> getNeighbours(Vector2Int pos)
    {
        List<Vector2Int> neighBours = new List<Vector2Int>();

        if (pos.y > 0)
        {
            Vector2Int top = new Vector2Int(pos.x, pos.y - 1);
            neighBours.Add(top);
        }

        if (pos.y < (mapSize - 1))
        {
            Vector2Int bottom = new Vector2Int(pos.x, pos.y + 1);
            neighBours.Add(bottom);
        }

        if (pos.x > 0)
        {
            Vector2Int left = new Vector2Int(pos.x - 1, pos.y);
            neighBours.Add(left);
        }

        if (pos.x < (mapSize - 1))
        {
            Vector2Int right = new Vector2Int(pos.x + 1, pos.y);
            neighBours.Add(right);
        }

        return neighBours;
    }


    private void initMapForDest(Vector2Int dest)
    {
        pFields[dest.x, dest.y] = new PotentialFields(mapSize);

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                double attraction = Math.Sqrt((Math.Pow(Math.Abs(x - dest.x), 2)) + Math.Pow(Math.Abs(y - dest.y) , 2));
                pFields[dest.x, dest.y].p[x, y] = attraction + weightMap[x, y];
            }
        }

        pFields[dest.x, dest.y].p[dest.x, dest.y] = 0;
    }

    //void 
}
