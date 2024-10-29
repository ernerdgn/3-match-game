using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Tile : MonoBehaviour
{
    public int x, y;

    private Item _item;
    public Item item
    {
        get { return _item; }  // => _item

        set
        {
            if (_item == value) return;
            _item = value;
            icon.sprite = _item.sprite;
        }
    }
    public Image icon;
    public Button button;

    public Tile Left => x > 0 ? Board.Instance.tiles[x - 1, y] : null;
    public Tile Right => x < Board.Instance.width - 1 ? Board.Instance.tiles[x + 1, y] : null;
    public Tile Top => y > 0 ? Board.Instance.tiles[x, y - 1] : null;
    public Tile Bottom => y < Board.Instance.height - 1 ? Board.Instance.tiles[x, y + 1] : null;

    public Tile[] neighbours => new[]
    { Left, Right, Top, Bottom };

    private void Start() => button.onClick.AddListener(() => Board.Instance.Select(this));

    public List<Tile> GetNeighbourTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> { this, };

        if (exclude == null) exclude = new List<Tile> { this, };
        else exclude.Add(this);

        foreach (var neighbour in neighbours)
        {
            if (neighbour == null || exclude.Contains(neighbour) || neighbour.item != item) continue;
            result.AddRange(neighbour.GetNeighbourTiles(exclude));
        }

        return result;
    }
}