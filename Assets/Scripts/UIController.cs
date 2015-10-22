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

    public void Setup()
    {
        RuneManager.Singelton.AddListener(typeof(RuneManager.SpawnEvent), OnCharacterSpawn);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnCharacterSpawn(RuneManager.Rune rune, System.Action action)
    {
        var go = Instantiate(healthBar);
        var spwn = rune as RuneManager.SpawnEvent;

        var character = ConflictController.Instance.CharactersInGame[spwn.guid];

        go.transform.position = character.transform.position + Vector3.up;
        go.SetActive(true);
        character.healthBar = go;

        action();
    }
}
