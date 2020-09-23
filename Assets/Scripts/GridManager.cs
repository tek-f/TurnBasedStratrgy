using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AztecArmy.gridManager
{
    public class GridManager : MonoBehaviour
    {
        #region Singleton - https://refactoring.guru/design-patterns/singleton/csharp/example#:~:text=Singleton%20is%20a%20creational%20design,the%20modularity%20of%20your%20code.
        // This makes the Class able to be accessed from ANYWHERE
        public static GridManager Instance = null;
        // NOTE: Try not to access 'Instance' from any 'Awake()' function in your project
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else // Optional - Fail Safe for if other GridManagers Exist
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
        #endregion

        public Tile m_prefab;
        public Tile[,] m_tiles;
        public int m_width = 1, m_depth = 1;
        public float m_spacing = 1f;
        public float m_rayDistance = 1000f;
        public float m_debugScale = .5f;

        void Start()
        {
            GenerateTiles();
        }

        Tile GetTile(Vector3 point, float radius)
        {
            var colliders = Physics.OverlapSphere(point, radius);
            foreach (var hit in colliders)
            {
                var tile = hit.GetComponent<Tile>();
                if (tile != null)
                {
                    return tile;
                }
            }
            return null;
        }
        Tile GetTile(Ray ray)
        {
            Tile tile = null;
            // Perform Raycast from Mouse Ray
            if (Physics.Raycast(ray, out var hit, m_rayDistance))
            {
                // Try getting Tile component from the thing we hit
                tile = hit.collider.GetComponent<Tile>();
            }
            return tile;
        }

        void OnDrawGizmos()
        {
            if (m_tiles == null)
                return;
            Gizmos.color = Color.red;
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Gizmos.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * m_rayDistance);
            Gizmos.color = Color.blue;
            var mouseTile = GetTile(mouseRay);
            Tile endTile = null;
            if (mouseTile != null)
            {
                // Hit a tile!
                var center = mouseTile.transform.position;
                var size = Vector3.one * m_spacing;
                Gizmos.DrawCube(center, size);
                endTile = mouseTile;
            }

            //VERY TEMPORARY!!! -Manny
            var startTile = GetTile(Vector3.zero, .5f);

            if (startTile != null && endTile != null)
            {
                var path = FindPath(startTile, endTile);
                if (path != null && path.Count > 0)
                {
                    var startPoint = path[0];
                    var size = Vector3.one * m_debugScale;
                    Gizmos.DrawCube(startPoint.Position, size);
                    for (int i = 1; i < path.Count; i++)
                    {
                        var endPoint = path[i];
                        Gizmos.DrawLine(startPoint.Position, endPoint.Position);
                        Gizmos.DrawCube(startPoint.Position, size);
                        startPoint = endPoint;
                    }
                }
            }
        }
        void GenerateTiles()
        {
            m_tiles = new Tile[m_width, m_depth];
            var halfWidth = m_width * .5f;
            var halfDepth = m_depth * .5f;
            var pivot = new Vector3(0.5f, 0f, 0.5f);
            for (int x = 0; x < m_width; x++)
            {
                for (int z = 0; z < m_depth; z++)
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
            //tile.Weight = 1;
            //tile.Parent = ;
            //tile.Cost = ;
            return tile;
        }

        public Tile GetTile(int x, int z)
        {
            if (x < 0 || x > m_width || z < 0 || z > m_depth)
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
            List<Tile> adjacencies;
            Tile current = start;

            // add start Tile to Open List
            OpenList.Add(start);

            while (OpenList.Count != 0 && !ClosedList.Exists(x => x.x == end.x && x.z == end.z))
            {
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacencies = GetAdjacentTiles(current);


                foreach (Tile n in adjacencies)
                {
                    if (!ClosedList.Contains(n) && !n.IsOccupied)
                    {
                        if (!OpenList.Contains(n))
                        {
                            n.Parent = current;
                            n.DistanceToTarget = Mathf.Abs(n.x - end.x) + Mathf.Abs(n.z - end.z);
                            n.Cost = n.Weight + n.Parent.Cost;
                            OpenList.Add(n);
                            OpenList = OpenList.OrderBy(Tile => Tile.F).ToList<Tile>();
                        }
                    }
                }
            }

            // construct path, if end was not closed return null
            if (!ClosedList.Exists(x => x.x == end.x && x.z == end.z))
            {
                return null;
            }

            // if all good, return path
            Tile temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null) return null;
            do
            {
                Path.Add(temp);
                temp = temp.Parent;
            } while (temp != start && temp != null);
            return Path;
        }

        private List<Tile> GetAdjacentTiles(Tile n)
        {
            List<Tile> temp = new List<Tile>();

            int col = (int)n.x;
            int row = (int)n.z;

            if (row + 1 < m_depth) 
                temp.Add(m_tiles[col, row + 1]);
            if (row - 1 >= 0) 
                temp.Add(m_tiles[col, row - 1]);
            if (col - 1 >= 0) 
                temp.Add(m_tiles[col - 1, row]);
            if (col + 1 < m_width) 
                temp.Add(m_tiles[col + 1, row]);

            return temp;
        }
    }
    #endregion
}