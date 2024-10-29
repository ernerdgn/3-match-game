using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    public Row[] rows;
    public Tile[,] tiles { get; private set; }
    public int width => tiles.GetLength(0);
    public int height => tiles.GetLength(1);

    private readonly List<Tile> _selection = new List<Tile>();
    
    private void Awake() => Instance = this;

    private const float swap_duration = .25f;

    private void Start()
    {
        tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var tile = rows[i].tiles[j];
                tile.x = j;
                tile.y = i;

                tile.item = ItemDB.items[UnityEngine.Random.Range(0, ItemDB.items.Length)];

                tiles[j, i] = tile;
            }
        }
    }

    //private void Update()  //  placed to debug -> GetNeighbourTiles
    //{
    //    if (!Input.GetKeyDown(KeyCode.S)) return;

    //    foreach (var neighbour_tile in tiles[0, 0].GetNeighbourTiles()) neighbour_tile.icon.transform.DOScale(1.25f, swap_duration).Play();
    //}

    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile)) _selection.Add(tile);
        if (_selection.Count < 2) return;

        Debug.Log($"tiles selected at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");
        await Swap(_selection[0], _selection[1]);

        if (CanPop()) Pop();
        else await Swap(_selection[1], _selection[0]);

        _selection.Clear();
    }

    public async Task Swap(Tile start, Tile target)
    {
        var start_icon = start.icon;
        var target_icon = target.icon;

        var start_icon_transform = start_icon.transform;
        var target_icon_transform = target_icon.transform;

        var sequence = DOTween.Sequence();
        sequence.Join(start_icon_transform.DOMove(target_icon_transform.position, swap_duration)).Join(target_icon_transform.DOMove(start_icon_transform.position, swap_duration));

        await sequence.Play().AsyncWaitForCompletion();

        start_icon_transform.SetParent(target.transform);
        target_icon_transform.SetParent(start.transform);

        start.icon = target_icon;
        target.icon = start_icon;

        var start_item = start.item;  //reference

        start.item = target.item;
        target.item = start_item;
    }

    private bool CanPop()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (tiles[j,i].GetNeighbourTiles().Skip(1).Count() >= 2) return true;
            }
        }

        return false;
    }

    private async void Pop()
    {
        for (int i =0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var tile = tiles[j,i];

                var neighbour_tiles = tile.GetNeighbourTiles();

                if (neighbour_tiles.Skip(1).Count() < 2) continue;

                var deflate_sequence = DOTween.Sequence();
                
                foreach (var neighbour_tile in  neighbour_tiles)
                {
                    deflate_sequence.Join(neighbour_tile.icon.transform.DOScale(Vector3.zero, swap_duration));
                }

                await deflate_sequence.Play().AsyncWaitForCompletion();

                var inflate_sequence = DOTween.Sequence();

                foreach (var neighbour_tile in neighbour_tiles)
                {
                    neighbour_tile.item = ItemDB.items[UnityEngine.Random.Range(0, ItemDB.items.Length)];

                    inflate_sequence.Join(neighbour_tile.icon.transform.DOScale(Vector3.one, swap_duration));
                }

                await inflate_sequence.Play().AsyncWaitForCompletion();
            }
        }
    }
}
