using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiController : Controller {

    public List<SlideCharacter> spottedEnemies;    
	// Use this for initialization
	void Start () {
        _state = ControllerState.WaitingForCharacterSelection;
        name = "AiController";
        spottedEnemies = new List<SlideCharacter>();
        RuneManager.Singelton.AddListener(typeof(RuneManager.MoveEvent), OnCharacterMove);
	}

	// Update is called once per frame
	void Update () {
	
	}

    public override void StartTurn()
    {
        _state = ControllerState.WaitingForTileSelection;
        selectedCharacter = crewMembers[0];
      //  ConflictController.Instance.AddPulseMaterial(selectedCharacter.GetComponent<Renderer>());
    }

    public void CanKill()
    {
        for(int i = 0;i<crewMembers.Count;i++)
        {

        }
    }

    public void CanHurt()
    {
        for(int i = 0;i<crewMembers.Count;i++)
        {

        }
    }

    public void WillLikelyDie()
    {

    }
    
    public void OnCharacterMove(RuneManager.Rune rune, System.Action action)
    {
        RuneManager.MoveEvent moveEvent = (RuneManager.MoveEvent)rune;
        if(moveEvent.mover.Team !=team)
        {
            if(!spottedEnemies.Contains(moveEvent.mover))
            {
                spottedEnemies.Add(moveEvent.mover);
            }
        }
    }
}
