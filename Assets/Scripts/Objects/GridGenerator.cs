using System;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Diamond Count Settings")]
    [SerializeField] private int maxDiamondsCount = 45;
    [SerializeField] private int minDiamondsCount = 25;

    [Header("Grid Parameters")]
    [SerializeField][Range(0.25f, 0.50f)] private float lavaChance = 0.25f;
    [SerializeField] private int rows = 8;
    [SerializeField] private int columns = 16;

    [Header("Tiles Prefabs")]
    [SerializeField] private GameObject lavaPrefab, safeIslandPrefab, diamondPrefab;

    [Header("Grid Parent Settings")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private float tileSize;
    [SerializeField] private Vector3 girdParentScale;
    [SerializeField] private Vector3 gridParentPosition;
    [SerializeField] private Vector3 YoffsetDiamond;

    private readonly List<Transform> islandPrefabsTracker = new(); //List to Keep track of spawned island tiles to generate diamonds on them.

    public enum TerrainType
    {
        Lava,
        Island
    }

    void Start()
    {
        GenerateGrid();
        SetGridSizeNPosition(gridParent); //setting Grid Size & Position after Generating, To center it & fit it on the screen.
        GenrateDiamonds();
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 tilePosition = new Vector3(
                    gridParent.position.x + j * tileSize,
                   gridParent.position.y,
                    gridParent.position.z + i * tileSize
                );

                var chosenTile = GetRandomTile();

                bool isIsland = chosenTile == TerrainType.Island;

                var tilePrefab = GetTilePrefab(chosenTile);

                GameObject tile = Instantiate(
                    tilePrefab,
                    tilePosition,
                    Quaternion.identity,
                    gridParent
                );

                if (isIsland)
                    islandPrefabsTracker.Add(tile.transform);
            }
        }
    }

    private void GenrateDiamonds()
    {
        int diamondCount = UnityEngine.Random.Range(minDiamondsCount, maxDiamondsCount);
        diamondCount = Mathf.Min(diamondCount, islandPrefabsTracker.Count);
        GameManager.Instance.SetTotalDiamonds(diamondCount);

        Shuffle(islandPrefabsTracker); //Fisher yates Shuffle to Randomize Islands ,
                                       //the position of the islands is to be used to generate the diamonds on first N islands.
        for (int k = 0; k < diamondCount; k++)
        {
            Instantiate(
                diamondPrefab,
                islandPrefabsTracker[k].position + YoffsetDiamond,
                Quaternion.identity,
                islandPrefabsTracker[k]
            );
        }


    }

    private TerrainType GetRandomTile()
    {
        var chance = UnityEngine.Random.value;
        return chance <= lavaChance ? TerrainType.Lava : TerrainType.Island;
    }

    private GameObject GetTilePrefab(TerrainType tileType)
    {
        switch (tileType)
        {
            case TerrainType.Lava:
                return lavaPrefab;
            case TerrainType.Island:
                return safeIslandPrefab;
            default:
                throw new ArgumentOutOfRangeException(nameof(tileType));
        }
    }

    private void SetGridSizeNPosition(Transform gridParent)
    {
        gridParent.localScale = girdParentScale;
        gridParent.position = gridParentPosition;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}
