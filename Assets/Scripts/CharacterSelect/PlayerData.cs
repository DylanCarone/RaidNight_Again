using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int PlayerIndex { get; set; }

    public Color PlayerColor { get; set; }= Color.white;
    
    public CharacterData characterData;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
