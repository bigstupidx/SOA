﻿using UnityEngine;
using System.Collections;

public class TileSet : MonoBehaviour
{

    public int width, height;

    public int treasureWidth, treasureHeight;

    public bool reverseX;

    protected Tile[,] tiles;

    private Vector3 minCoord, maxCoord, minTreasure, maxTreasure;

    void Awake()
    {
        tiles = new Tile[width, height];

        for (int j = 0; j < width; j++)
        {
            for (int k = 0; k < height; k++)
            {
                tiles[j, k] = new Tile(this, j, k);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        Vector3 pos = transform.position;

        if (!reverseX)
        {
            minCoord = pos;
            minTreasure = pos;

            maxCoord = new Vector3(pos.x + width, pos.y + height);
            maxTreasure = new Vector3(pos.x + treasureWidth, pos.y + treasureHeight);
        }
        else
        {
            minCoord = new Vector3(pos.x - width, pos.y);
            minTreasure = new Vector3(pos.x - treasureWidth, pos.y);

            maxCoord = new Vector3(pos.x, pos.y + height);
            maxTreasure = new Vector3(pos.x, pos.y + treasureHeight);
        }
    }

    public bool CanPlace(Vector3 position)
    {
        return minCoord.x < position.x && minCoord.y < position.y &&
               maxCoord.x > position.x && maxCoord.y > position.y &&
               !(minTreasure.x < position.x && minTreasure.y < position.y &&
               maxTreasure.x > position.x && maxTreasure.y > position.y);
    }

    public int GetXCoord(float xPos)
    {
        if (!reverseX)
        {
            return Mathf.FloorToInt(xPos - transform.position.x);
        }
        else
        {
            return Mathf.FloorToInt(transform.position.x - xPos);
        }
    }

    public int GetYCoord(float yPos)
    {
        return Mathf.FloorToInt(yPos - transform.position.y);
    }

    public Vector3 CenterOn(Vector3 position)
    {
        return tiles[GetXCoord(position.x), GetYCoord(position.y)].GetCenter();
    }

    public void LoadBlock(int x, int y, GameObject block)
    {
        GameObject placed = Instantiate(block, tiles[x, y].GetCenter(), new Quaternion()) as GameObject;

        Breakable breakable = placed.GetComponent<Breakable>();
        tiles[x, y].SetBlock(breakable);
    }

    public void RemoveBlock(GameObject block)
    {
        SaveFile.Instance().RemoveTile(tiles[GetXCoord(block.transform.position.x), GetYCoord(block.transform.position.y)]);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 mnc, mxc, mnt, mxt;

        Vector3 pos = transform.position;

        if (!reverseX)
        {
            mnc = pos;
            mnt = pos;

            mxc = new Vector3(pos.x + width, pos.y + height);
            mxt = new Vector3(pos.x + treasureWidth, pos.y + treasureHeight);
        }
        else
        {
            mnc = new Vector3(pos.x - width, pos.y);
            mnt = new Vector3(pos.x - treasureWidth, pos.y);

            mxc = new Vector3(pos.x, pos.y + height);
            mxt = new Vector3(pos.x, pos.y + treasureHeight);
        }

        Gizmos.color = Color.green;

        Gizmos.DrawLine(mnc, new Vector3(mnc.x, mxc.y));
        Gizmos.DrawLine(mxc, new Vector3(mnc.x, mxc.y));
        Gizmos.DrawLine(mnc, new Vector3(mxc.x, mnc.y));
        Gizmos.DrawLine(mxc, new Vector3(mxc.x, mnc.y));

        Gizmos.color = Color.yellow;

        Gizmos.DrawCube((mnt + mxt) / 2f, (mxt - mnt));
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
