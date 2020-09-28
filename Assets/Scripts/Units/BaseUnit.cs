using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AztecArmy.gameManager;
using AztecArmy.gridManager;

namespace AztecArmy.Units
{
    public class BaseUnit : Unit
    {
        public List<GameObject> unitPrefabs = new List<GameObject>();
        public GameObject unitToSpawn;
        public void SpawnUnitSelection(int unitType)
        {
            unitToSpawn = unitPrefabs[unitType];
            gameManager.selectionState = 3;
        }
        public void SpawnUnit(Transform targetTile)
        {
            //GameObject spawnedObject = Instantiate(unitToSpawn);
            Tile tile = targetTile.GetComponent<Tile>();
            Unit spawnedUnit = Instantiate(unitToSpawn).GetComponent<Unit>();
            spawnedUnit.transform.SetParent(tile.transform.parent);
            spawnedUnit.teamID = teamID;
            spawnedUnit.MoveToGridSpace(gridManager, tile.x, tile.z);
            active = false;
            unitToSpawn = null;
            gameManager.selectionState = 0;
        }
        protected override void Start()
        {
            base.Start();
            //TESTING

        }
    }
}