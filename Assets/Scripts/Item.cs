using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Match-3 Game/Item")]
public sealed class Item : ScriptableObject
{
    public Sprite sprite;
    public int value;
}
