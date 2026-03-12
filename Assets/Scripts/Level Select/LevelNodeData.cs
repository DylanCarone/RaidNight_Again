using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Node", menuName = "World Map/Level Node")]
public class LevelNodeData : ScriptableObject
{
    public string levelName;
    public SceneAsset scene;
    public Sprite previewImage;
    public bool isUnlocked = false;
    public bool isCompleted = false;

}
