using UnityEngine;
using System.Collections.Generic;

public class GridController : MonoBehaviour
{

    private Tile[][] _grid;
    public int sizeOfArray;
    public GameObject tilePrefab;
    public GameObject TestPlayerGameObject;

    public static GridController Singelton;

    void Awake()
    {
        _grid = new Tile[sizeOfArray][];
        var pos = new Vector3();
        for (var x = 0; x < sizeOfArray; x++)
        {
            _grid[x] = new Tile[sizeOfArray];
            for (var y = 0; y < sizeOfArray; y++)
            {
                var go = Instantiate(tilePrefab);
                go.transform.parent = transform;
                pos.x = x;
                pos.z = y;
                pos.y = 0;
                go.transform.position = pos;
                _grid[x][y] = go.GetComponent<Tile>();
                _grid[x][y].SetUpTile(x, y);
                go.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
            }
        }
        Singelton = this;
    }

    public Tile GetTile(int x, int y)
    {
      return  _grid[x][y];
    }
    
    public static int FindLowest(List<Tile> tiles)
    {
        if (tiles.Count < 0)
            return -1;

        var lowest = tiles[0];
        var index = 0;
        for (var i = 0; i < tiles.Count; i++)
        {
            if (!(tiles[i].F < lowest.F)) continue;
            lowest = tiles[i];
            index = i;
        }

        return index;
    }

    public static List<Tile> FinalPath(Tile tile)
    {
        var path = new List<Tile>();

        var current = tile;
        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }

    public List<Tile> FindPathToUnit(Tile start, Tile end)
    {
        var openList = new List<Tile>();
        var closedList = new List<Tile>();

        var seenTiles = new Dictionary<string, Tile>();

        foreach (var t in _grid)
        {
            for (var y = 0; y < _grid[0].Length; y++)
            {
                t[y].ResetTile();
            }
        }

        var xDifToFinal = Mathf.Abs(end.X - start.X);
        var yDifToFinal = Mathf.Abs(end.Y - start.Y);

        var dsqrToFinal = (int)Mathf.Pow(xDifToFinal, 2) + (int)Mathf.Pow(yDifToFinal, 2);

        start.H = dsqrToFinal;
        start.G = 0;
        start.F = dsqrToFinal;


        openList.Add(start);

        while(openList.Count > 0)
        {
            var q = openList[FindLowest(openList)];

            if(q == end)
            {
                return FinalPath(q);
            }

            q.HasBeenCheckedBySearch = true;
            openList.Remove(q);

            closedList.Add(q);

            if(!seenTiles.ContainsKey(q.Name))
                seenTiles.Add(q.Name, q);

            var neighbors = Neighbors(q);

            foreach (var t in neighbors)
            {
                if (t == end)
                {
                    t.Parent = q;

                    return FinalPath(t);
                }

                if (closedList.Contains(t))
                {
                    continue;
                }
                t.Parent = q;

                var xDif = Mathf.Abs(q.X - t.X);
                var yDif = Mathf.Abs(q.Y - t.Y);
                var dsqr = Mathf.Pow(xDif, 2) + Mathf.Pow(yDif, 2);
                var xDifToFinal_ = Mathf.Abs(end.X - t.X);
                var yDifToFinal_ = Mathf.Abs(end.Y - t.Y);
                var dsqrToFinal_ = Mathf.Pow(xDifToFinal_, 2) + Mathf.Pow(yDifToFinal_, 2);

                var tentativeG = q.G + dsqr;
                if (t.G == 0.0f)
                {
                    t.G = tentativeG;
                }

                t.H = dsqrToFinal_;
                t.F = t.G + t.H;


                if (!seenTiles.ContainsKey(t.Name))
                {
                    openList.Add(t);
                }

                if ((!(tentativeG <= t.G))) continue;
                t.G = tentativeG;
                t.F = t.G + t.H;
            }
        }



        return null;
    }

    public List<Tile> Neighbors(Tile center)
    {
        var neighbors = new List<Tile>();

        if ((center.Y - 1) >= 0)
        {
            neighbors.Add(_grid[center.X][center.Y -1]);

            if ((center.X - 1) >= 0)
                neighbors.Add(_grid[center.X - 1][center.Y - 1]);

            if ((center.X + 1) < sizeOfArray)
                neighbors.Add(_grid[center.X + 1][center.Y - 1]);
        }

        if ((center.X - 1) >= 0)
            neighbors.Add(_grid[center.X - 1][center.Y]);

        if ((center.X + 1) < sizeOfArray)
            neighbors.Add(_grid[center.X][center.Y + 1]);

        if ((center.Y + 1) >= sizeOfArray) return neighbors;
        neighbors.Add(_grid[center.X][center.Y + 1]);

        if ((center.X - 1) >= 0)
            neighbors.Add(_grid[center.X - 1][center.Y + 1]);

        if ((center.X + 1) < sizeOfArray)
            neighbors.Add(_grid[center.X + 1][center.Y + 1]);

        return neighbors;
    }


}