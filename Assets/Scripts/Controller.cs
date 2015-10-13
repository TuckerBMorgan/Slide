using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
    private List<SlideCharacter> team; 

	// Use this for initialization
	void Start () {
        team = new List<SlideCharacter>();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void StartTurn()
    {
        
    }

    public virtual void EndTurn()
    {
        
    }
}
