using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AbilityButtonControl : MonoBehaviour {

    public List<GameObject> buttons;
    public static AbilityButtonControl Instance;
    
    public void Setup()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            buttons.Add(transform.GetChild(i).gameObject);
            buttons[i].SetActive(false);
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeSelectedCharacter(SlideCharacter characters)
    {
        for(int i = 0;i<characters.offensiveActions.Count;i++)
        {
            string name = characters.offensiveActions[i].name;
            Image img = Resources.Load("Icons/" + name) as Image;
            buttons[i].GetComponent<Image>().
            Debug.Log("Icons/" + name);
        }
    }
}
