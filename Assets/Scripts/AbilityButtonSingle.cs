using UnityEngine;
using System.Collections;

public class AbilityButtonSingle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick()
    {
        ConflictController.Instance.CurrentController.SelectedCharacter.OnAbilityButtonPressed(name);
    }
}
