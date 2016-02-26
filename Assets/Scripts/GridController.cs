using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GridController : MonoBehaviour
{

    public static char WalkableTile = '0';

    public static char NonWalkableTile = '|';

    private Tile[][] _grid;
    public int sizeOfArray;
    public GameObject tilePrefab;
    public GameObject TestPlayerGameObject;
    
    public List<Tile> team1SpawnTiles;
    public int team1SpawnedPlayers;

    public List<Tile> team2SpawnTiles;
    public int team2SpawnedPlayers;

    public static GridController Singelton;

    private void Awake()
    {
        Singelton = this;
    }

    public Tile GetTile(int x, int y)
    {
        return _grid[x][y];
    }

    public Tile[][] GetGrid()
    {
        return _grid;
    }
    
    public static void DisplayMoveRange(SlideCharacter slide)
    {
        Tile[][] grid = GridController.Singelton.GetGrid();
        for (int x = 0; x < grid.Length; x++)
        {
            for (int y = 0; y < grid[0].Length; y++)
            {
                grid[x][y].SetToColor();
                if (slide.allowedActions["Move"].ValidateSelection(grid[x][y]))
                {
                    grid[x][y].SetToWhite();
                }
            }
        }
    }

    public static void ResetGridColor()
    {
        Tile[][] grid = GridController.Singelton.GetGrid();
        for (int x = 0; x < grid.Length; x++)
        {
            for (int y = 0; y < grid[0].Length; y++)
            {
                grid[x][y].SetToColor();
            }
        }
    }

    public List<RuneManager.MoveEvent> GetRunedPath(SlideCharacter slideCharacter, Tile start, Tile end)
    {

        var path = FindPathToUnit(start, end);
        var moves = new List<RuneManager.MoveEvent>();
        var previous = path[0];
        for (var i = 1; i < path.Count; i++)
        {
            if (path[i] == end)
            {

                var move_ = new RuneManager.MoveEvent(slideCharacter, previous, path[i]);
                moves.Add(move_);
                return moves;
    
            }

            var move = new RuneManager.MoveEvent(slideCharacter, previous, path[i]);
            moves.Add(move);
            previous = path[i];
        }
        return moves;
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
                if(t.Occupied || !t.Walkable)
                {
                    continue;
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
            neighbors.Add(_grid[center.X + 1][center.Y]);

        if ((center.Y + 1) >= sizeOfArray) return neighbors;
        neighbors.Add(_grid[center.X][center.Y + 1]);

        if ((center.X - 1) >= 0)
            neighbors.Add(_grid[center.X - 1][center.Y + 1]);

        if ((center.X + 1) < sizeOfArray)
            neighbors.Add(_grid[center.X + 1][center.Y + 1]);

        return neighbors;
    }

    public Tile[][] ProduceGridFromFile(string fileName)
    {
        TextAsset gridAsText = Resources.Load("Maps/" + fileName) as TextAsset;
        team1SpawnTiles = new List<Tile>();
        team2SpawnTiles = new List<Tile>();
        
        char[] ar = gridAsText.text.ToCharArray();

        int xSize = 0; //int.TryParse(string.Concat([ar[0], ar[1]]));
        StringBuilder sb = new StringBuilder();
        sb.Append(ar[0]);
        sb.Append(ar[1]);

        int.TryParse(sb.ToString(), out xSize);
        sb.Remove(0, sb.Length);
        sb.Append(ar[3]);
        sb.Append(ar[4]);
        int ySize = 0;
        int.TryParse(sb.ToString(), out ySize); 
        
        
        _grid = new Tile[xSize][];

        for (int i = 0; i < xSize; i++) 
        {
            _grid[i] = new Tile[ySize];
        }
        int tileCount = 0;
        var pos = new Vector3();
        for (int i = 5; i < ar.Length; i++)
        {
            if (ar[i] == '\n' || ar[i] == ',' || ar[i] == ' ' || ar[i] == 13)
                continue;
            var go = Instantiate(tilePrefab);
            
            if(ar[i] == WalkableTile)
            {
                go.GetComponent<Tile>().Walkable = true;

                go.GetComponent<Tile>().keepColor = new Color(Random.value, Random.value, Random.value);
            }
            else if(ar[i] == NonWalkableTile)
            {
                go.GetComponent<Tile>().Walkable = false;

                go.GetComponent<Tile>().keepColor = Color.black;
                go.transform.eulerAngles = new Vector3(90,0,0);
            }
            else 
            {
                go.GetComponent<Tile>().Walkable = true;
                go.GetComponent<Tile>().SpawnTile = true;

                int team = int.Parse(ar[i].ToString());
                go.GetComponent<Tile>().Team = team;
                if(team == 1)
                {
                    team1SpawnTiles.Add(go.GetComponent<Tile>());
                }
                else
                {
                    team2SpawnTiles.Add(go.GetComponent<Tile>());
                }

                go.GetComponent<Tile>().keepColor = new Color(Random.value, Random.value, Random.value);
            }

            go.transform.parent = transform;
            pos.x = tileCount - ((tileCount / xSize) * 10);
            pos.z = tileCount / ySize;
            pos.y = 0;
            go.transform.position = pos;
            go.GetComponent<Tile>().SetToColor();
            go.name = pos.x + "," + pos.z;

            _grid[tileCount - ((tileCount / xSize) * 10)][tileCount / ySize] = go.GetComponent<Tile>();
            _grid[tileCount - ((tileCount / xSize) * 10)][tileCount / ySize].SetUpTile(tileCount - ((tileCount / xSize) * 10),tileCount / ySize);
            tileCount++;
        }

        return _grid;
    }

    public Tile GetTeamSpawnPoints(int team)
    {
        if(team == 0)
        {
            team1SpawnedPlayers++;
            return team1SpawnTiles[team1SpawnedPlayers -1];
        }
        team2SpawnedPlayers++;
        return team2SpawnTiles[team2SpawnedPlayers - 1];
    }

}