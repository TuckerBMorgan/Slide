using UnityEngine;
using System.Collections.Generic;

public class PlayerController : Controller
{
    public SlideCharacter SelectedCharacterController;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void TileSelected(Tile tile)
    {
        if (_state != ControllerState.WaitingForTileSelection) return;
        var moves = GridController.Singelton.GetRunedPath(SelectedCharacterController, SelectedCharacterController.currentTile, tile);

        for (var index = 0; index < moves.Count; index++)
        {
            var t = moves[index];
            RuneManager.Singelton.ExecuteRune(t);
        }
    }

    public override void StartTurn()
    {

    }

    public override void EndTurn()
    {

    }
}
