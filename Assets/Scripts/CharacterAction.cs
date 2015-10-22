using System;
using System.Collections;
using System.Collections.Generic;

public abstract class CharacterAction
{
    public string name;
    public abstract void StartAction();
    public abstract void EndAction();
}
