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
        if (entity == selectedCharacter) return;

        AbilityButtonControl.Instance.ChangeSelectedCharacter(slide);
        ConflictController.Instance.RemovePulseMaterial(selectedCharacter.GetComponent<Renderer>());
        selectedCharacter = entity as SlideCharacter;
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
        selectedCharacter = crewMembers[0];
        ConflictController.Instance.AddPulseMaterial(selectedCharacter.GetComponent<Renderer>());
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
