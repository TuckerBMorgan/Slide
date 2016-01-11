using System;
using System.Collections;
using System.Collections.Generic;

public abstract class CharacterAction
{
    public string name;
    public abstract bool ValidateSelection(Entity entity);
    public abstract void PreformAction(Entity entity);
    public abstract void StartAction();
    public abstract void EndAction();
    public virtual void DrawInspector()
    {

    }
}
public interface Entity
{
    string GetEntityType();
    UnityEngine.MonoBehaviour GetUnityObject();
    Tile getCurrentTile();
}