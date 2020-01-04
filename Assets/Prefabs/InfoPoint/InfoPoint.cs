using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPoint : MonoBehaviour
{
    [TextArea]
    public string infoText;

    private Text _txtComponent;
    private GameObject _panel;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _panel = transform.Find("Canvas/Panel").gameObject;
        _txtComponent = _panel.GetComponentInChildren<Text>();
        _txtComponent.text = infoText;

    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            _panel.SetActive(true);
            
        }

    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            _panel.SetActive(false);
            
        }
    }
}
