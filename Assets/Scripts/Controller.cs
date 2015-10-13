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


    private int team;
    private Guid guid;
    private List<SlideCharacter> crewMembers;

    void Awake()
    {
        crewMembers = new List<SlideCharacter>();
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

    public virtual void StartTurn()
    {
        
    }

    public virtual void EndTurn()
    {
        
    }
}
