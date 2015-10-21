using UnityEngine;
using System.Collections.Generic;

public class PlayerController : Controller
{

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void TileSelected(Tile tile)
    {
        if (_state != ControllerState.WaitingForTileSelection) return;
        if (selectedCharacter == null) return;

        Debug.Log(tile.name);
        Debug.Log(selectedCharacter.currentTile);
        var moves = GridController.Singelton.GetRunedPath(selectedCharacter, selectedCharacter.currentTile, tile);
        Debug.Log(moves.Count);
        for (var index = 0; index < moves.Count; index++)
        {
            var t = moves[index];
            RuneManager.Singelton.ExecuteRune(t);
        }
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
}
