﻿using System;
using UnityEngine;
using System.Collections;

public class SlideCharacter : MonoBehaviour {
    
    private float health;
    private float armour;
    private int actions;
    private Guid guid;
    private int team;

    public Tile currentTile;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Setup(float Health, float Armour, int Actions, Guid guid, int Team)
    {
        health = Health;
        armour = Armour;
        actions = Actions;
        this.guid = guid;
        team = Team;
    }
}
