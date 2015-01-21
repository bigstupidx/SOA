﻿using UnityEngine;
using System.Collections;

public class BuildState : GameState
{
    private CashBasherManager manager;

    private BlockInventory selectedInventory;

    private int myTeam;

    private TileSet tileSet;

    private GameObject spawnIndicator;

    private Vector3 lastTouchPos;

    private bool cameraFocusLeft;

    public BuildState(CashBasherManager gameManager, int team, TileSet yourSet, GameObject indicator)
    {
        manager = gameManager;
        myTeam = team;
        tileSet = yourSet;
        spawnIndicator = indicator;

        cameraFocusLeft = myTeam == 0;
    }

    public void Prepare()
    {

    }

    public void GetClickedOn()
    {
        Ray clickRay = manager.guiCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(clickRay, out hit))
        {
            BlockInventory inventory = hit.collider.gameObject.GetComponent<BlockInventory>();

            if (inventory)
            {
                if (inventory.IsSelected())
                {
                    DeselectInventory(inventory);
                }
                else
                {
                    SelectInventory(inventory);
                }

                return;
            }
        }

        lastTouchPos = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);
        manager.cameraMan.enabled = false;
    }

    public void GetHeldOn()
    {
        if (selectedInventory)
        {
            Vector3 position = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            if (tileSet.CanPlace(position))
            {
                spawnIndicator.SetActive(true);
            }
            else
            {
                spawnIndicator.SetActive(false);
            }

            position.x = Mathf.Floor(position.x) + 0.5f;
            position.y = Mathf.Floor(position.y) + 0.5f;
            position.z = 0;

            spawnIndicator.transform.position = position;
        }
        else
        {
            Vector3 nextPos = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 translation = new Vector3(lastTouchPos.x - nextPos.x, 0f);

            if (cameraFocusLeft && translation.x + manager.playerCamera.transform.position.x < -10f)
            {
                translation.x = -10f - manager.playerCamera.transform.position.x;
            }
            else if (!cameraFocusLeft && translation.x + manager.playerCamera.transform.position.x > 10f)
            {
                translation.x = 10f - manager.playerCamera.transform.position.x;
            }

            manager.playerCamera.transform.Translate(translation);
        }
    }

    public void GetReleasedOn()
    {
        if (selectedInventory)
        {
            spawnIndicator.SetActive(false);

            Ray clickRay = manager.playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(clickRay, out hit))
            {
                Breakable breakable = hit.collider.gameObject.GetComponent<Breakable>();

                if (breakable)
                {
                    return;
                }
            }

            Vector3 position = manager.playerCamera.ScreenToWorldPoint(Input.mousePosition);

            if (tileSet.CanPlace(position))
            {
                PlaceBlock(manager.playerCamera.ScreenToWorldPoint(Input.mousePosition), selectedInventory);
            }
        }
        else
        {
            manager.cameraMan.enabled = true;

            if (cameraFocusLeft && manager.playerCamera.transform.position.x > -5f)
            {
                manager.cameraMan.FollowPosition(new Vector3(10f, 0f, 0f));
                cameraFocusLeft = false;
            }
            else if (!cameraFocusLeft && manager.playerCamera.transform.position.x < 5f)
            {
                manager.cameraMan.FollowPosition(new Vector3(-10f, 0f, 0f));
                cameraFocusLeft = true;
            }
        }
    }

    public void End()
    {

    }

    void SelectInventory(BlockInventory inventory)
    {
        selectedInventory = inventory;
        inventory.Select();
    }

    void PlaceBlock(Vector3 position, BlockInventory inventory)
    {
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.5f;
        position.z = 0;

        GameObject block = Network.Instantiate(inventory.GetBlock(), position, new Quaternion(), 0) as GameObject;

        Breakable b = block.GetComponent<Breakable>();
        b.SetTeam(myTeam);

        inventory.Deselect(true);
        selectedInventory = null;
    }

    void DeselectInventory(BlockInventory inventory)
    {
        inventory.Deselect(false);
        selectedInventory = null;
    }

}
