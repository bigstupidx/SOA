﻿using UnityEngine;
using System.Collections;

public class EditorTileSet : TileSet
{
    public BlockType PlaceAndSaveBlock(Vector3 position, GameObject block)
    {
        int x = GetXCoord(position.x);
        int y = GetYCoord(position.y);

        GameObject placed = Instantiate(block, tiles[x, y].GetCenter(), new Quaternion()) as GameObject;

        Breakable breakable = placed.GetComponent<Breakable>();
        tiles[x, y].SetBlock(breakable);

        SaveFile.Instance().AddTile(tiles[x, y]);

        return breakable.type;
    }

    public void Clear()
    {
        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                if (!tiles[j, k].Empty())
                {
                    tiles[j, k].DestroyBlock();   
                }
            }
        }
    }

    public void Save()
    {
        SaveFile.Instance().ClearTileList();

        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                if (!tiles[j, k].Empty())
                {
                    SaveFile.Instance().AddTile(tiles[j, k]);
                }
            }
        }
    }
}
