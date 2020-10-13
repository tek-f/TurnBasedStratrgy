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
        public GameObject SpawnRangedUnitButton, SpawnMeleeUnitButton;
        public void SpawnUnitSelection(int unitType)
        {
            unitToSpawn = unitPrefabs[unitType];
            gameManager.selectionState = 3;
        }
        public void SpawnUnit(Transform targetTile)
        {
            GameObject spawnedObject = Instantiate(unitToSpawn);
            Tile tile = targetTile.GetComponent<Tile>();
            Unit spawnedUnit = spawnedObject.GetComponent<Unit>();
            spawnedUnit.TeamID = teamID;
            spawnedUnit.MoveToGridSpace(gridManager, tile.x, tile.z);
            gameManager.AddUnitToList(spawnedUnit, teamID);
            active = false;
            unitToSpawn = null;
        }
        private void Start()
        {
            OnUnitSpawn(0);
        }
    }
}