using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnityManager : MonoBehaviour {

    public static EnityManager Singleton;

    public Dictionary<Guid, SlideCharacter> characters;
    
    public Dictionary<Guid, GameObject> avatars;

    public void Awake()
    {
        Singleton = this;
    }

}
