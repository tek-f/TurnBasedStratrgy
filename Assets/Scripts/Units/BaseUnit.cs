using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AztecArmy.Units
{
    public class BaseUnit : Unit
    {
        public List<GameObject> unitPrefabs = new List<GameObject>();
        public GameObject unitToSpawn;
        public void SpawnUnitSelection(int unitType)
        {
            unitToSpawn = unitPrefabs[unitType];
            gm.selectionState = 3;
        }
        public void SpawnUnit(Transform targetTile)
        {
            //GameObject spawnedObject = Instantiate(unitToSpawn);
            Unit spawnedUnit = Instantiate(unitToSpawn).GetComponent<Unit>();
            spawnedUnit.teamID = teamID;
            spawnedUnit.MoveToPosition(targetTile);
            active = false;
            unitToSpawn = null;
            gm.selectionState = 0;
        }
    }
}