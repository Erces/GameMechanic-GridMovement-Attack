using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node
{
    private Vector3 worldPosition;
    private int gridX;
    private int gridY;
    private float height;
    private bool walkable;
    private Node parent;
    private bool hasEntity;
    private Entity entity;

    public int gCost;
    public int hCost;
    public int fCost 
    { 
        get 
        { 
            return gCost + hCost; 
        } 
    }

    public Node(Vector3 worldPosition, int gridX, int gridY, float height, bool walkable)
    {
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.height = height;
        this.walkable = walkable;
    }


    public Vector3 GetWorldPosition() => worldPosition;
    public void SetWorldPosition(Vector3 newPosition)
    {
        worldPosition = newPosition;
    }

    public float GetHeight() => height;
    public void SetHeight(float height)
    {
        this.height = height;
    }

    public Vector2 GetCoordinates() => new Vector2(gridX,gridY);
    public void GetCoordinates(out int x, out int y)
    {
        x = gridX;
        y = gridY;
    }
    public void SetCoordinates(int x, int y)
    {
        this.gridX = x;
        this.gridY = y;
    }

    public Node GetParent() => parent;
    public void SetParent(Node parent)
    {
        this.parent = parent;
    }

    public bool GetWalkable() => walkable;
    public void SetWalkable(bool walkable)
    {
        this.walkable = walkable;
    }

    public bool GetHasEntity() => hasEntity;
    public void SetHasEntity(bool hasEntity)
    {
        this.hasEntity = hasEntity;
    }

    public Entity GetEntity() => entity;
    public void SetEntity(Entity entity)
    {
        this.entity = entity;
    }

}
