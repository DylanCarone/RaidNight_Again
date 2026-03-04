using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_EffectIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image durationFill;

    private StatusEffect trackedEffect;
    
    public void Initialize(StatusEffect effect)
    {
        trackedEffect = effect;
        icon.sprite = effect.Icon;
    }

    private void Update()
    {
        if(trackedEffect == null) return;

        durationFill.fillAmount = trackedEffect.RemainingDuration / trackedEffect.Duration;
    }
}
