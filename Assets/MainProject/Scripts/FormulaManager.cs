using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormulaManager : MonoBehaviour
{
    public static FormulaManager instance;

    [Header("Force Formula")]
    [SerializeField] private GameObject forceUI;
    [SerializeField] private Slider massSlider;
    [SerializeField] private Slider gravitySlider;

    [Header("Density Formula")]
    [SerializeField] private GameObject densityUI;
    [SerializeField] private Slider d_massSlider;
    [SerializeField] private Slider volumeSlider;

    [Header("Velocity Formula")]
    [SerializeField] private GameObject velocityUI;
    [SerializeField] private Slider distanceSlider;
    [SerializeField] private Slider timePositionSlider;

    [Header("Elasticity Formula")]
    [SerializeField] private GameObject elasticityUI;
    [SerializeField] private Slider elasticKoefSlider;
    [SerializeField] private Slider stretchLengthSlider;

    [Header("Area Formula")]
    [SerializeField] private GameObject areaUI;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider widthSlider;

    [Header("Friction Formula")]
    [SerializeField] private GameObject frictionUI;
    [SerializeField] private Slider frictionKoefSlider;
    [SerializeField] private Slider weightSlider;

    private PhysicObject activeObject;
    private Formula activeFormula;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void RecieveObject(FormulaReciever sender, Formula formula, FormulaLimits limits)
    {
        activeFormula = formula;
        if(activeObject) ResetActiveObject();
        DisableAllUIs();

        if (sender.gameObject.TryGetComponent(out activeObject))
        {
            switch (formula)
            {
                case Formula.Force:
                    massSlider.onValueChanged.RemoveAllListeners();

                    SetSliderValues(massSlider, limits.massLimit, activeObject.mass);

                    massSlider.onValueChanged.AddListener((x) => { activeObject.mass = x; });

                    forceUI.SetActive(true);

                    break;
                case Formula.Density:
                    d_massSlider.onValueChanged.RemoveAllListeners();
                    volumeSlider.onValueChanged.RemoveAllListeners();

                    SetSliderValues(d_massSlider, limits.massLimit, activeObject.mass);
                    SetSliderValues(volumeSlider, limits.volumeLimit, activeObject.volume);

                    d_massSlider.onValueChanged.AddListener((x) => { activeObject.mass = x; });
                    volumeSlider.onValueChanged.AddListener((x) => { activeObject.volume = x; });

                    densityUI.SetActive(true);

                    break;
                case Formula.Velocity:
                    distanceSlider.onValueChanged.RemoveAllListeners();
                    timePositionSlider.onValueChanged.RemoveAllListeners();
                    
                    activeObject.doRecords = false;
                    activeObject.timePosition = 0;

                    SetSliderValues(distanceSlider, limits.timePositionLimit, activeObject.mass);
                    SetSliderValues(timePositionSlider, limits.timePositionLimit, activeObject.timePosition);

                    timePositionSlider.onValueChanged.AddListener((x) => { activeObject.timePosition = x; });


                    velocityUI.SetActive(true);

                    break;
                case Formula.Elasticity:
                    elasticKoefSlider.onValueChanged.RemoveAllListeners();
                    stretchLengthSlider.onValueChanged.RemoveAllListeners();

                    SetSliderValues(elasticKoefSlider, limits.elastityKoefLimit, activeObject.bounciness);

                    elasticKoefSlider.onValueChanged.AddListener((x) => { activeObject.bounciness = x; });

                    elasticityUI.SetActive(true);

                    break;
                case Formula.Area:

                    heightSlider.onValueChanged.RemoveAllListeners();
                    widthSlider.onValueChanged.RemoveAllListeners();

                    SetSliderValues(heightSlider, limits.heightLimit, activeObject.height);
                    SetSliderValues(widthSlider, limits.widthtLimit, activeObject.width);

                    heightSlider.onValueChanged.AddListener((x) => { activeObject.height = x; });
                    widthSlider.onValueChanged.AddListener((x) => { activeObject.width = x; });

                    areaUI.SetActive(true);

                    break;

                case Formula.Friction:
                    frictionKoefSlider.onValueChanged.RemoveAllListeners();

                    SetSliderValues(frictionKoefSlider, limits.frictionKoefLimit, activeObject.bounciness);

                    frictionKoefSlider.onValueChanged.AddListener((x) => { activeObject.friction = x; });

                    frictionUI.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetSliderValues(Slider slider, Vector2 limits, float currentValue)
    {
        slider.transform.Find("Min").GetComponent<TMP_Text>().text = limits.x.ToString();
        slider.transform.Find("Max").GetComponent<TMP_Text>().text = limits.y.ToString();
        slider.minValue = limits.x;
        slider.maxValue = limits.y;
        slider.value = currentValue;
    }

    private void DisableAllUIs()
    {
        forceUI.SetActive(false);
        densityUI.SetActive(false);
        velocityUI.SetActive(false);
        elasticityUI.SetActive(false);
        areaUI.SetActive(false);
        frictionUI.SetActive(false);
    }

    private void ResetActiveObject()
    {
        activeObject.doRecords = true;
        
    }
}
