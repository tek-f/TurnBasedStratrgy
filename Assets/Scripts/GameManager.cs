using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AztecArmy.Units;


public class GameManager : MonoBehaviour
{
    public static Unit selectedUnit;
    int selectionState = 0;//used to determine what kind of selection state the player is in, i.e. selecting units, selecting where to move the selectedUnit

    Camera mainCamera;
    /*void SelectObject(GameObject clicked)
    {
        selectedUnit = clicked.GetComponent<Unit>();
        selectedUnit.GetComponent<Unit>().OnSelection();
    }*/
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
                                
                            }
                            else
                            {
                                
                            }
                        }
                    }
                    break;
                case 1://selecting movement

                    break;
                case 2://selecting attack target

                    break;
            }
    }
}
