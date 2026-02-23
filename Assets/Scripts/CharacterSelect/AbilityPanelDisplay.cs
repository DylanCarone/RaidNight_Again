using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPanelDisplay : MonoBehaviour
{
    [SerializeField] Image abilityImage;
    [SerializeField] TextMeshProUGUI abilityName;
    [SerializeField] TextMeshProUGUI abilityDescription;
    private Ability ability;

    public void Initialize(Ability ability)
    {
        this.ability = ability;
        abilityImage.sprite = ability.icon;
        abilityName.text = ability.name;
        abilityDescription.text = ability.description;
    }
}
