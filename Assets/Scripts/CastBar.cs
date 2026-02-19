using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CastBar : MonoBehaviour
{
    [SerializeField] private CombatEntity entity;
    [SerializeField] private GameObject castBarVisuals;
    [SerializeField] private TextMeshProUGUI castNameText;
    private Slider castBar;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        castBar = GetComponent<Slider>();
    }

    private void Start()
    {
        entity.OnCastStart += ShowCastBar;
        entity.OnCastComplete += HideCastBar;
        entity.OnCastInterrupted += HideCastBar;
    }

    private void OnDisable()
    {
        entity.OnCastStart -= ShowCastBar;
        entity.OnCastComplete -= HideCastBar;
        entity.OnCastInterrupted -= HideCastBar;
    }


    // Update is called once per frame
    void Update()
    {
        castBar.value = entity.CastProgress;
    }

    void ShowCastBar(string castName, float castTime)
    {
        castBarVisuals.SetActive(true);
        castNameText.text = castName;
    }

    void HideCastBar(string castName)
    {
        castBarVisuals.SetActive(false);
    }
}
