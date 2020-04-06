using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventSystem))]
public class EventSystemReceiver : MonoBehaviour
{
    public static EventSystemReceiver Instance;
    [SerializeField] GameObject _PanelParent;
    EventSystem _EventSystem;
    
    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _EventSystem = GetComponent<EventSystem>();
    }

    public void SelectNewButton(int panel)
    {
        _EventSystem.SetSelectedGameObject(_PanelParent.transform.GetChild(panel).GetComponentInChildren<Button>().gameObject);
    }
}
