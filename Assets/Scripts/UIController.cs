using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [System.Serializable]
    public class BaseUI
    {
        public TeamColor team;
        public Slider droneCountSlider;
        public TextMeshProUGUI droneCountText;
    }

    [Header("Bases")]
    [SerializeField] private BaseUI[] baseUIs;
    [SerializeField] private BaseManager[] bases;

    [Header("Resource Count Display")]
    [SerializeField] private TextMeshProUGUI blueTeamText;
    [SerializeField] private TextMeshProUGUI redTeamText;

    private void Start()
    {
        foreach (var ui in baseUIs)
        {
            var baseManager = GetBaseByTeam(ui.team);
            if (baseManager != null)
            {
                ui.droneCountSlider.onValueChanged.AddListener((value) => OnDroneSliderChanged(ui.team, value));
                ui.droneCountSlider.value = baseManager.Drones.Count;
                UpdateText(ui, (int)ui.droneCountSlider.value);
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

    private IEnumerator UpdateResourceCounts()
    {
        while (true)
        {
            foreach (var b in bases)
            {
                if (b.TeamColorValue == Color.blue)
                    blueTeamText.text = $"Blue: {b.CollectedResources}";
                else if (b.TeamColorValue == Color.red)
                    redTeamText.text = $"Red: {b.CollectedResources}";
            }

            yield return new WaitForSeconds(0.25f);
        }
    }
}