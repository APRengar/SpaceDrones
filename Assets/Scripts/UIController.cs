using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider droneCountSlider;
    [SerializeField] private Text droneCountText;

    [Header("Controlled Base")]
    [SerializeField] private BaseManager baseManager;

    private void Start()
    {
        droneCountSlider.onValueChanged.AddListener(OnDroneCountChanged);
        droneCountSlider.value = baseManager.Drones.Count;
        UpdateDroneCountText((int)droneCountSlider.value);
    }

    private void OnDroneCountChanged(float newValue)
    {
        int count = Mathf.RoundToInt(newValue);
        baseManager.SetDroneCount(count);
        UpdateDroneCountText(count);
    }

    private void UpdateDroneCountText(int count)
    {
        if (droneCountText != null)
            droneCountText.text = "Drones: " + count;
    }
}

