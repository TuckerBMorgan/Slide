using UnityEngine;
using System.Collections;

public class ConflictController : MonoBehaviour
{
    public Controller CurrentController;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShiftTurn(Controller newController)
    {
        CurrentController.EndTurn();
        newController.StartTurn();
        CurrentController = newController;
    }
}
