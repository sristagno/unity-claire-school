using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ProgressBar healthProgressBar, energyProgressBar, foodProgressBar;
    [SerializeField] private float decreaseFood = 1f, decreaseRate = 0.8f;

    [SerializeField] private float walkEnergy = 1f, runEnergy = 3f;

    private ClaireController _claireController;

    private UISlotChild _uiSlotChildScript;
    private bool _isClaireWalking, _isClaireRunning;
    private IEnumerator _energyCoroutine = null;
    // Start is called before the first frame update
    void Start()
    {
        _uiSlotChildScript = GetComponent<UISlotChild>();
        _claireController = GameObject.FindGameObjectWithTag("Player").GetComponent<ClaireController>();
        _claireController.IsWalkingChanged += (o, val) =>
        {
            _isClaireWalking = val;
            ManageEnergy();
        };
        _claireController.IsRunningChanged += (sender, val) => { _isClaireRunning = val; ManageEnergy(); };
        

        var hungerCoroutine = DecreaseHunger();
        StartCoroutine(hungerCoroutine);
        _claireController.IsKilled += (sender, args) =>
        {
            if (hungerCoroutine != null)
            {
                StopCoroutine(hungerCoroutine);
                hungerCoroutine = null;
            }

            if (_energyCoroutine != null)
            {
                StopCoroutine(_energyCoroutine);
                _energyCoroutine = null;
            }
        };

        _claireController.IsHurted += (sender, damage) => { DecreaseHealth(damage); };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FreeChild()
    {
        _uiSlotChildScript.AddChildInSlot();
    }

    private void DecreaseHealth(float damage)
    {
        healthProgressBar.Percentage -= damage;
        if (healthProgressBar.Percentage <= 0)
        {
            _claireController.Kill();
        }
    }

    private void ManageEnergy()
    {
        if (_isClaireWalking || _isClaireRunning)
        {
            if (_energyCoroutine == null)
            {
                _energyCoroutine = DecreaseEnergy();
                StartCoroutine(_energyCoroutine);
            }
        }
        else
        {
            
            if (_energyCoroutine != null)
            {
                StopCoroutine(_energyCoroutine);
                _energyCoroutine = null;
            }
        }
    }

    private IEnumerator DecreaseEnergy()
    {
        
        float decreaseEnergy = 0f;
        if (_isClaireWalking)
        {
            decreaseEnergy = walkEnergy;
        }
        else if (_isClaireRunning)
        {
            decreaseEnergy = runEnergy;
        }
        if (energyProgressBar != null)
        {
            while (energyProgressBar.Percentage > 0)
            {
                energyProgressBar.Percentage -= decreaseEnergy;
                yield return new WaitForSeconds(1.3f);
            }
            _claireController.Kill();
        }
    }

    private IEnumerator DecreaseHunger()
    {
        if (foodProgressBar != null)
        {
            while (foodProgressBar.Percentage > 0)
            {
                foodProgressBar.Percentage -= decreaseFood;
                yield return new WaitForSeconds(decreaseRate);
            }
            _claireController.Kill();
        }
    }
}
