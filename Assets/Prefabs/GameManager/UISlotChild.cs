using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlotChild : MonoBehaviour
{
    [SerializeField] private Sprite slotWithChild;

    [SerializeField] private Image[] slots;
    [SerializeField] private GameObject _exitParticle;

    private int x = 0;

    public bool IsSlotComplete { get; private set; }
    public void AddChildInSlot()
    {
        slots[x].GetComponent<Image>().sprite = slotWithChild;
        x++;
        
        IsSlotComplete = x == slots.Length;
        if (IsSlotComplete)
        {
            _exitParticle.SetActive(true);
        }
        
        x = Mathf.Clamp(x, 0, slots.Length);
    }
    // Start is called before the first frame update
    void Awake()
    {
        _exitParticle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
