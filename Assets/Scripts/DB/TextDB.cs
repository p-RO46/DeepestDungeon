using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public struct NameImage
{
    public string name;
    public Image image;
}

[CreateAssetMenu(fileName = "TextDB", menuName = "Scriptable Objects/TextDB")]
public class TextDB : ScriptableObject, ISerializationCallbackReceiver
{
    [Header("Malt")]
    public List<TextAsset> malt_start = new List<TextAsset>();
    public List<TextAsset> malt_gift = new List<TextAsset>();
    public List<TextAsset> malt_trap = new List<TextAsset>();
    public TextAsset malt_thirst;
    public TextAsset malt_watered;

    [Header("Conversation")]
    public TextAsset starting;
    public List<TextAsset> floor_start = new List<TextAsset>();
    public List<TextAsset> ending = new List<TextAsset>();
    public TextAsset deadEnd;

    [Header("Speaker Name & Sprite")]
    // 직렬화를 위한 리스트
    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<Sprite> values = new List<Sprite>();

    // 직렬화할 수 없는 Dictionary
    [NonSerialized]
    public Dictionary<string, Sprite> name_sprite = new Dictionary<string, Sprite>();

    // 직렬화되기 전에 호출됨
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in name_sprite)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    // 디시리얼라이즈된 후에 호출됨
    public void OnAfterDeserialize()
    {
        name_sprite = new Dictionary<string, Sprite>();

        for (int i = 0; i < keys.Count; i++)
        {
            name_sprite[keys[i]] = values[i];
        }
    }
}
