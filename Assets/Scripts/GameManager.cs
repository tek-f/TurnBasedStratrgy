using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AztecArmy.Units;

namespace AztecArmy.gameManager
{
    public class GameManager : MonoBehaviour
    {
        public Unit selectedUnit;
        public int selectionState = 0;//used to determine what kind of selection state the player is in, i.e. selecting units, selecting where to move the selectedUnit
        Camera mainCamera;
        void Start()
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }
        void Update()
        {
            switch (selectionState)
            {
                case 0://selecting units
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.gameObject.GetComponent<Unit>() != null)
                            {
                                if (selectedUnit != null)
                                {
                                    selectedUnit.OnDeSelection();
                                }
                                selectedUnit = hit.transform.gameObject.GetComponent<Unit>();
                                selectedUnit.OnSelection();
                            }
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                    {
                        selectedUnit.OnDeSelection();
                        selectedUnit = null;
                    }
                    break;
                case 1://selecting movement
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.gameObject.CompareTag("Tile") && selectedUnit != null)
                            {
                                selectedUnit.MoveToTile(hit.transform);
                            }
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                    {
                        selectionState = 0;
                    }
                    break;
                case 2://selecting attack target
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.gameObject.GetComponent<Unit>() != null)
                            {
                                selectedUnit.BasicAttack(hit.transform.gameObject.GetComponent<Unit>());
                            }
                        }
                    }
                    if (Input.GetButtonDown("Cancel"))
                    {
                        selectionState = 0;
                    }
                    break;
                case 3://selecting target for spawn ability, specific to the base unit
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.GetComponent<Tile>() != null)
                            {
                                selectedUnit.gameObject.GetComponent<BaseUnit>().SpawnUnit(hit.transform);
                            }
                        }
                    }
                    break;
                default:
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.LogError("Selection State Out of Range");
                    }
                    break;
            }
        }
    }
}