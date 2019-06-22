using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour {

    public Manager manager;
    public Button startDrawBtn;
    public Button clearBtn;

	void Start ()
    {
        startDrawBtn.onClick.AddListener(OnStartDrawClick);
        clearBtn.onClick.AddListener(OnClearClick);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            OnStartDrawClick();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            OnClearClick();
        }
    }

    private void OnStartDrawClick()
    {
        Debug.Log("OnStartDrawClick");
        manager.StartDraw();
    }

    private void OnClearClick()
    {
        Debug.Log("OnClearClick");
    }
}
