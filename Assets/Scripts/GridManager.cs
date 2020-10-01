using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AztecArmy.gridManager
{
    public class GridManager : MonoBehaviour
    {
        #region Singleton
        public static GridManager Instance = null;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
        #endregion

        public Tile m_prefab;
        public Tile[,] m_tiles;
        public int gridWidth = 1, gridDepth = 1;
        public float m_spacing = 1f;
        public float m_rayDistance = 1000f;
        public float m_debugScale = .5f;

        
        public void GenerateTiles()
        {
            m_tiles = new Tile[gridWidth, gridDepth];
            var halfWidth = gridWidth * .5f;
            var halfDepth = gridDepth * .5f;
            var pivot = new Vector3(0.5f, 0f, 0.5f);
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridDepth; z++)
                {
                    var position = new Vector3(x - halfWidth, 0f, z - halfDepth);
                    position += pivot;
                    position *= m_spacing;
                    var newTile = SpawnTile(position);
                    newTile.x = x;
                    newTile.z = z;
                    m_tiles[x, z] = newTile;
                }
            }
        }
        Tile SpawnTile(Vector3 position)
        {
            var instance = Instantiate(m_prefab.gameObject, position, Quaternion.identity, transform);
            var tile = instance.GetComponent<Tile>();
            return tile;
        }
        public Tile GetTile(int x, int z)
        {
            if (x < 0 || x > gridWidth || z < 0 || z > gridDepth)
            {
                return null; // Out of Range!
            }
            return m_tiles[x, z];
        }
        #region PathFinding
        public List<Tile> FindPath(Tile start, Tile end)
        {
            List<Tile> Path = new List<Tile>();
            List<Tile> OpenList = new List<Tile>();
            List<Tile> ClosedList = new List<Tile>();
            List<Tile> adjacentTiles;
            Tile current = start;

            // add start Tile to Open List
            OpenList.Add(start);
            while (OpenList.Count != 0 && !ClosedList.Exists(x => x.x == end.x && x.z == end.z))
            {
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacentTiles = GetAdjacentTiles(current);

                foreach (Tile tile in adjacentTiles)
                {
                    if (!ClosedList.Contains(tile) && !tile.IsOccupied)
                    {
                        if (!OpenList.Contains(tile))
                        {
                            tile.previousTile = current;
                            tile.DistanceToTarget = Mathf.Abs(tile.x - end.x) + Mathf.Abs(tile.z - end.z);
                            tile.Cost = tile.Weight + tile.previousTile.Cost;
                            OpenList.Add(tile);
                            OpenList = OpenList.OrderBy(Tile => Tile.F).ToList<Tile>();
                        }
                    }
                }
            }

            // construct path, if end was not closed return null
            if (!ClosedList.Exists(x => x.x == end.x && x.z == end.z))
                return null;

            Tile temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null)
                return null;
            do
            {
                Path.Add(temp);
                temp = temp.previousTile;
            }
            while (temp != start && temp != null);

            return Path;
        }
        private List<Tile> GetAdjacentTiles(Tile n)
        {
            List<Tile> temp = new List<Tile>();

            int col = n.x;
            int row = n.z;

            if (row + 1 < gridDepth)
                temp.Add(m_tiles[col, row + 1]);
            if (row - 1 >= 0) 
                temp.Add(m_tiles[col, row - 1]);
            if (col + 1 < gridWidth) 
                temp.Add(m_tiles[col + 1, row]);
            if (col - 1 >= 0) 
                temp.Add(m_tiles[col - 1, row]);

            return temp;
        }
    }
    #endregion
}