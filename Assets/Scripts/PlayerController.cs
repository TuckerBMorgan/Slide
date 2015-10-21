using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : Controller
{

	// Use this for initialization
	void Start () {
        RuneManager.Singelton.AddListener(typeof(RuneManager.WaitForTileInput), TileInput);	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void TileSelected(Tile tile)
    {
        if (_state != ControllerState.WaitingForTileSelection) return;
        if (selectedCharacter == null) return;
        
        var moves = GridController.Singelton.GetRunedPath(selectedCharacter, selectedCharacter.currentTile, tile);
        for (var index = 0; index < moves.Count; index++)
        {
            var t = moves[index];
            RuneManager.Singelton.ExecuteRune(t);
        }
        _state = ControllerState.WaitingForActionToFinish;
        var getInput = new RuneManager.WaitForTileInput(this);
        RuneManager.Singelton.ExecuteRune(getInput);



    }

    public override void CharacterSelected(SlideCharacter character)
    {
        selectedCharacter = character;
    }

    public override void StartTurn()
    {
        _state = ControllerState.WaitingForTileSelection;
        selectedCharacter = crewMembers[0];
        ConflictController.Instance.AddPulseMaterial(selectedCharacter.GetComponent<Renderer>());
    }

    public override void EndTurn()
    {
        _state = ControllerState.WaitingForTurn;
    }

    public void TileInput(RuneManager.Rune rune, Action action)
    {
        var tInput = ((RuneManager.WaitForTileInput) rune);

        if (tInput.controller == this)
        {
            _state = ControllerState.WaitingForTileSelection;
        }
        action();
    }
}
