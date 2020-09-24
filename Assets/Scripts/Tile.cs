using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.gridManager
{
    public class Tile : MonoBehaviour
    {
        public Tile previousTile = null;
        public int x, z;
        public float DistanceToTarget = 0f;
        public float Cost = 1f;
        public float Weight = 1f;
        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
                    return -1;
            }
        }
        public Vector3 m_pivotOffset = new Vector3(0f, .5f, 0f);
        public bool IsOccupied => transform.childCount > 0;
        public Vector3 PivotPoint => transform.position + m_pivotOffset;
        public Vector3 Position => transform.position;
    }
}