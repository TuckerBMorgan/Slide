using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public GameObject healthBar;
    public static UIController Singelton;

    void Awake()
    {
        Singelton = this;
    }

	// Use this for initialization
	void Start () {
	
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
