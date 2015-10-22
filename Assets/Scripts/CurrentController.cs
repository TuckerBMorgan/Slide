using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CurrentController : MonoBehaviour {

    private Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (ConflictController.Instance.CurrentController != null)
        {
            text.text = "Current Player " + ConflictController.Instance.CurrentController.name + " in " + ConflictController.Instance.CurrentController._state + "\n"
                + "AP: " + ConflictController.Instance.CurrentController.SelectedCharacter.GetActionPoints();
        }
        else
        {
            text.text = "No Controller";
        }
	}
}
