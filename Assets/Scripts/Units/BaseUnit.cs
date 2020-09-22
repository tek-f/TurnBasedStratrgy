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
            GameObject spawnedUnit = Instantiate(unitToSpawn, targetTile.position, unitToSpawn.transform.rotation);
            //spawnedUnit.GetComponent<Unit>().MoveToTile(targetTile);
            unitToSpawn = null;
            gm.selectionState = 0;
        }
    }
}