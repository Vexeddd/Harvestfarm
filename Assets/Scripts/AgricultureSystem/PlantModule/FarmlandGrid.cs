using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmlandGrid
{
    public float cellSize = 5f;
    public Vector2Int gridSize = new Vector2Int();
    public Vector3 originPos = new Vector3Int();

    public Dictionary<Vector2Int, FarmlandTile> tiles = new Dictionary<Vector2Int, FarmlandTile>();


    public void CreateFarmTile(Vector3 originPos, int width, int height)
    {
        this.originPos = originPos;  
        this.gridSize = new Vector2Int(width, height);

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(this.originPos.x + cellSize * x + cellSize / 2f,
                                          this.originPos.y,
                                          this.originPos.z + cellSize * z + cellSize / 2f);

                GameObject go = GameObject.Instantiate(AgricultureSystem.Instance.soil_prefab);
                go.transform.position = pos;
                go.name = $"soil({x},{z})";

                FarmlandTile tile = go.GetComponent<FarmlandTile>();
                if (tile == null)
                {
                    tile = go.AddComponent<FarmlandTile>();
                }
                tile.worldPos = pos;
                tile.gridPos = new Vector2Int(x, z);
                var grid = new Vector2Int(x, z);
                tiles.Add(grid, tile);
            }
        }
    }
}
