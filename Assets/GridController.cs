using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GridController : MonoBehaviour
{

    private Tile[][] Grid;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static int FindLowest(List<Tile> tiles)
    {
        if (tiles.Count < 0)
            return -1;

        Tile lowest = tiles[0];
        int index = 0;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].F < lowest.F)
            {
                lowest = tiles[i];
                index = i;
            }
        }

        return index;
    }

    public static List<Tile> FinalPath(Tile tile)
    {
        List<Tile> path = new List<Tile>();

        Tile current = tile;
        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }
        return path;
    }

    public List<Tile> FindPathToUnit(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        Dictionary<string, Tile> SeenTiles = new Dictionary<string, Tile>();

        for (int x = 0; x < Grid.Length; x++)
        {
            for (int y = 0; y < Grid[0].Length; y++)
            {
                Grid[x][y].ResetTile();
                if(Grid[x][y] == end)
                {
                    openList.Add(end);
                    return openList;
                }
            }
        }

        int xDifToFinal = Mathf.Abs(end.X - start.X);
        int yDifToFinal = Mathf.Abs(end.Y - start.Y);

        int DsqrToFinal = (int)Mathf.Pow(xDifToFinal, 2) + (int)Mathf.Pow(yDifToFinal, 2);

        start.H = DsqrToFinal;
        start.G = 0;
        start.F = DsqrToFinal;


        openList.Add(start);

        while(openList.Count > 0)
        {
            Tile q = openList[FindLowest(openList)];

            if(q == end)
            {
                return FinalPath(q);
            }

            q.HasBeenCheckedBySearch = true;
            openList.Remove(q);

            closedList.Add(q);
            SeenTiles.Add(q.Name, q);

            List<Tile> Neighbors = //


        }




    }
}