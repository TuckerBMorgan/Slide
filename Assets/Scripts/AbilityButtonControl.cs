using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AbilityButtonControl : MonoBehaviour {

    public List<GameObject> buttons;
    public static AbilityButtonControl Instance;
    
    public void Awake()
    {
        Instance = this;
        for (int i = 0; i < transform.childCount; i++)
        {
            buttons.Add(transform.GetChild(i).gameObject);
            buttons[i].SetActive(false);
        }
    }

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
        string[] keys = characters.allowedActions.Keys.ToArray();
        for(int i = 0;i<characters.allowedActions.Count;i++)
        {
            string iconName = keys[i];
            Sprite img = Resources.Load<Sprite>("Icons/Spells/" + iconName);
            buttons[i].SetActive(true);
            buttons[i].GetComponent<Image>().sprite = img;
            buttons[i].name = iconName;
        }
    }
}
