using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] private PlayerCombatEntity player;
    [SerializeField] private Image fillArea;
    private Slider resourceBar;
    
    private void Awake()
    {
        resourceBar = GetComponent<Slider>();
    }

    private void Start()
    {
        //Debug.Log(entity.CurrentHealth);
        resourceBar.maxValue = player.MaxResoruce;
        resourceBar.value = player.CurrentResource;
        player.OnResourceChanged += UpdateResource;
        ChangeColor();
        
    }

    void ChangeColor()
    {
        var playerResource = player.ResourceType;
        switch (playerResource)
        {
            case ResourceType.Mana:
                fillArea.color = MyPalette.ManaBlue;
                break;
            case ResourceType.Rage:
                fillArea.color = MyPalette.RageRed;
                break;
            case ResourceType.Energy:
                fillArea.color = MyPalette.EnergyYellow;
                break;
            
        }
    }

    private void OnDisable()
    {
        player.OnResourceChanged -= UpdateResource;
    }


    private void UpdateResource(float current)
    {
        resourceBar.maxValue = player.MaxResoruce;
        ChangeColor();
        resourceBar.value = current;
    }
}


public static class MyPalette
{
    public static readonly Color ManaBlue = HexToColor("64A8EE");
    public static readonly Color RageRed = HexToColor("E06772");
    public static readonly Color EnergyYellow = HexToColor("F1E953");

    private static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
            return color;
        return Color.white;
    }
}