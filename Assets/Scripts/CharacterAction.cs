﻿using System;
using System.Collections;
using System.Collections.Generic;

public abstract class CharacterAction  {
    public enum CharacterActionType
    {
        Move,
        Attack,
        Buff,
    }
    public abstract void StartAction();
    public abstract void EndAction();
}
