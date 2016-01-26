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

        for (int i = 0; i < transform.childCount; i++)
        {
            buttons.Add(transform.GetChild(i).gameObject);
            buttons[i].SetActive(false);
        }
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void ChangeSelectedCharacter(SlideCharacter characters)
    {
        for(int i = 0;i<characters.offensiveActions.Count;i++)
        {
            string name = characters.offensiveActions[i].name;
            Sprite img = Resources.Load<Sprite>("Icons/Spells/" + name);
            buttons[i].SetActive(true);
            buttons[i].GetComponent<Image>().sprite = img;
            buttons[i].name = name;
        }
    }
}
