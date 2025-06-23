using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [System.Serializable]
    public class BaseUI
    {
        public TeamColor team;
        public Slider droneCountSlider;
        public TextMeshProUGUI droneCountText;
        public TMP_Text resourceText;
    }

    [Header("Bases")]
    [SerializeField] private BaseUI[] baseUIs;
    [SerializeField] private BaseManager[] bases;

    [SerializeField] private Slider droneSpeedSlider;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 10f;

    [SerializeField] private TMP_InputField spawnRateInputField;
    [SerializeField] private ResourceSpawner resourceSpawner;

    // [SerializeField] private Toggle showPathsToggle;

    private void Start()
    {
        droneSpeedSlider.onValueChanged.AddListener(OnDroneSpeedChanged);
        OnDroneSpeedChanged(droneSpeedSlider.value);

        spawnRateInputField.onEndEdit.AddListener(OnSpawnRateChanged);

        // showPathsToggle.onValueChanged.AddListener(OnShowPathsChanged);
        // OnShowPathsChanged(showPathsToggle.isOn);

        foreach (var ui in baseUIs)
        {
            var baseManager = GetBaseByTeam(ui.team);
            if (baseManager != null)
            {
                ui.droneCountSlider.onValueChanged.AddListener((value) => OnDroneSliderChanged(ui.team, value));
                ui.droneCountSlider.value = baseManager.Drones.Count;
                UpdateText(ui, (int)ui.droneCountSlider.value);

                baseManager.OnResourceChanged += (_) => UpdateResourceText(ui, baseManager.CollectedResources);
                UpdateResourceText(ui, baseManager.CollectedResources);
            }
        }
    }

    private void OnDroneSliderChanged(TeamColor team, float newValue)
    {
        int count = Mathf.RoundToInt(newValue);

        var ui = GetUIByTeam(team);
        if (ui != null)
            UpdateText(ui, count);

        var baseManager = GetBaseByTeam(team);
        if (baseManager != null)
            baseManager.SetDroneCount(count);
    }

    private void UpdateText(BaseUI ui, int count)
    {
        if (ui.droneCountText != null)
            ui.droneCountText.text = $"{ui.team} Drones: {count}";
    }

    private BaseManager GetBaseByTeam(TeamColor team)
    {
        foreach (var b in bases)
            if (b != null && b.TeamColorValue == GetColorByTeam(team))
                return b;
        return null;
    }

    private BaseUI GetUIByTeam(TeamColor team)
    {
        foreach (var ui in baseUIs)
            if (ui.team == team)
                return ui;
        return null;
    }

    private Color GetColorByTeam(TeamColor team)
    {
        return team == TeamColor.Blue ? Color.blue : Color.red;
    }

    private void UpdateResourceText(BaseUI ui, int amount)
    {
        if (ui.resourceText != null)
            ui.resourceText.text = $"{amount}";
    }

    private void OnDroneSpeedChanged(float value)
    {
        float speed = Mathf.Lerp(minSpeed, maxSpeed, value);

        foreach (var baseManager in FindObjectsOfType<BaseManager>())
        {
            foreach (var drone in baseManager.Drones)
            {
                var agent = drone.GetComponent<NavMeshAgent>();
                if (agent != null)
                    agent.speed = speed;
            }
        }
    }

    private void OnSpawnRateChanged(string value)
    {
        if (float.TryParse(value, out float spawnInterval) && spawnInterval > 0)
        {
            resourceSpawner.SetSpawnInterval(spawnInterval);
        }
    }

    
    // private void OnShowPathsChanged(bool isOn)
    // {
    //     foreach (var baseManager in FindObjectsOfType<BaseManager>())
    //     {
    //         foreach (var drone in baseManager.Drones)
    //         {
    //             var agent = drone.GetComponent<NavMeshAgent>();
    //             if (agent != null)
    //                 // agent.isPathStale = false;
    //                 agent.updatePosition = true;
    //                 agent.updateRotation = true;

    // #if UNITY_EDITOR
    //             UnityEditor.EditorUtility.SetSelectedRenderState(agent.GetComponent<Renderer>(), isOn 
    //                 ? UnityEditor.EditorSelectedRenderState.Wireframe 
    //                 : UnityEditor.EditorSelectedRenderState.Hidden);
    // #endif
    //         }
    //     }
    // }
}