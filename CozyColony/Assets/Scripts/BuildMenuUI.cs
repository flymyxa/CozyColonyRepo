using CozyWorld;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI; // for UI Button
using TMPro;

public class BuildMenuUI : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private List<BuildDef> availableBuilds;
    public UnityEvent<BuildDef> OnBuildSelected;

    private void Start()
    {
        foreach (BuildDef def in availableBuilds)
        {
            Button b = Instantiate(buttonPrefab, buttonContainer);
            b.GetComponentInChildren<TextMeshProUGUI>().text = def.displayName;
            b.onClick.AddListener(() => OnBuildSelected.Invoke(def));
        }
    }
}

