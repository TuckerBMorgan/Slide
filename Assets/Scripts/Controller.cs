using System;
using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
    public enum ControllerType
    {
        Player,
        AI
    }

    public enum  ControllerState
    {
        WaitingForTurn,
        WaitingForTileSelection,
        WaitingForCharacterSelection,
        WaitingForActionToFinish
    }

    protected int team;
    protected Guid guid;
    protected List<SlideCharacter> crewMembers;
    protected ControllerState _state;

    void Awake()
    {
        crewMembers = new List<SlideCharacter>();
        _state = ControllerState.WaitingForTurn;
    }
	// Use this for initialization
	void Start () {
    
    }
	
	// Update is called once per frame, 
	void Update () {
	
	}

    public void AddCrewMember(SlideCharacter crew)
    {
        crewMembers.Add(crew);
    }

    public void Setup(int Team, Guid guid)
    {
        team = Team;
        this.guid = guid;
    }

    public virtual void TileSelected(Tile tile)
    {
    }


    public virtual void StartTurn()
    {
        
    }

    public virtual void EndTurn()
    {
        
    }


    
}
