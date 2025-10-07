using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radial_Selection_Operation : MonoBehaviour
{
    [SerializeField]
    private GameObject ControlPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Radial_Operations(int option)
    {
        switch (option)
        {
            case 1:
                SetControlPanelVisibility();
                break;
            case 2:
                Debug.Log("Case 2");
                break;
            // You can have any number of case statements.
            default:
                Debug.Log("Default Case");
                break;
        }
    }

    void SetControlPanelVisibility()
    {
        if (ControlPanel.activeSelf)
        {
            ControlPanel.SetActive(false);
        }
        else
        {
            ControlPanel.SetActive(true);
        }
    }
}
