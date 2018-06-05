using UnityEngine;

[CreateAssetMenu(menuName = "Custom/TileDefenition")]
public class TileDefinition : ScriptableObject
{
    public Sprite TileSprite;
    public Sprite TileSpriteSelected;
    public AudioClip TileSelectAudio;
    public Color EffectColor;

    public int GetTypeId()
    {
        return GetInstanceID();
    }
}