using UnityEngine;
using System.Collections;

public class AiController : Controller {

	// Use this for initialization
	void Start () {
        _state = ControllerState.WaitingForCharacterSelection;
        name = "AiController";
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
}
