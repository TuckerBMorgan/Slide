using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : Controller
{

	// Use this for initialization
	void Start () {
        RuneManager.Singelton.AddListener(typeof(RuneManager.WaitForSelection), SelectionInput);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnSelctionAction(Entity entity)
    {
        if (entity.GetEntityType() != "SlideCharacter") return;
        var slide = entity as SlideCharacter;
        if (slide.Team != team) return;

        if (selectedCharacter)
        {
            ConflictController.Instance.RemovePulseMaterial(selectedCharacter.GetComponent<Renderer>());
        }
        selectedCharacter = entity as SlideCharacter;
        AbilityButtonControl.Instance.ChangeSelectedCharacter(slide);
        GridController.DisplayMoveRange(selectedCharacter);

        ConflictController.Instance.AddPulseMaterial(selectedCharacter.GetComponent<Renderer>());
    }

    public override void OnSecondaryAction(Entity entity)
    {
        if (_state != ControllerState.WaitingForSelection) return;
        if (selectedCharacter == null) return;
        if (selectedCharacter.Team != team) return;
        
        if (selectedCharacter.OnEntitySelection(entity))
        {
            _state = ControllerState.WaitingForActionToFinish;
            var getInput = new RuneManager.WaitForSelection(this);
            RuneManager.Singelton.ExecuteRune(getInput);
        }
    }

    public void TileSelected(Tile tile)
    {
        if (_state != ControllerState.WaitingForSelection) return;
        if (selectedCharacter == null) return;
        if (selectedCharacter.Team != team) return;
        if (selectedCharacter.GetActionPoints() == 0) return;
        var moves = GridController.Singelton.GetRunedPath(selectedCharacter, selectedCharacter.currentTile, tile);
        for (var index = 0; index < moves.Count; index++)
        {
            var t = moves[index];
            RuneManager.Singelton.ExecuteRune(t);
        }
        _state = ControllerState.WaitingForActionToFinish;

        var runeSetAp = new RuneManager.SetActionPoint(selectedCharacter.GetActionPoints() - 1, selectedCharacter);
        RuneManager.Singelton.ExecuteRune(runeSetAp);

        var getInput = new RuneManager.WaitForSelection(this);
        RuneManager.Singelton.ExecuteRune(getInput);
    }
    
    public override void StartTurn()
    {
        _state = ControllerState.WaitingForSelection;
        for (int i = 0; i < crewMembers.Count;i++ )
        {
            var setAp = new RuneManager.SetActionPoint(3, crewMembers[i]);
            RuneManager.Singelton.ExecuteRune(setAp);
            crewMembers[i].SetActionPoints(3);
            crewMembers[i].OnStartTurn();
        }
//        selectedCharacter = crewMembers[0];
       OnSelctionAction(crewMembers[0]);

    //    ConflictController.Instance.AddPulseMaterial(selectedCharacter.GetComponent<Renderer>());
    }

    public override void EndTurn()
    {
        _state = ControllerState.WaitingForTurn;
        if (selectedCharacter == null) return;
        ConflictController.Instance.RemovePulseMaterial(selectedCharacter.GetComponent<Renderer>());
    }

    public void SelectionInput(RuneManager.Rune rune, Action action)
    {
        var tInput = ((RuneManager.WaitForSelection) rune);

        if (tInput.controller == this)
        {
            _state = ControllerState.WaitingForSelection;
        }
        action();
    }
}
