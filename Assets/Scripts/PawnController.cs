using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PawnController : MonoBehaviour {

    private int Health;
    private bool Targetable;
    public int Team;
    public int Range;
    public bool Aura;
    public List<RuneManager.Rune> onRangeEnterEvents;
    public List<RuneManager.Rune> onInRangeEvents;
    public List<RuneManager.Rune> onExitRange;

    void Setup(int Health, bool Targetable, int Team, int Range, bool Aura, List<RuneManager.Rune> onRangeEnterEvents, List<RuneManager.Rune> onInRangeEvents, List<RuneManager.Rune> onExitRange)
    {
        this.Health = Health;
        this.Targetable = Targetable;
        this.Team = Team;
        this.Range = Range;
        this.Aura = Aura;
        this.onRangeEnterEvents = onRangeEnterEvents;
        this.onInRangeEvents = onInRangeEvents;
        this.onExitRange = onExitRange;
        
//        RuneManager.Singelton.AddListener(typeof(RuneManager.MoveEvent), OnMoveEvent);

    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnMoveEvent(RuneManager.Rune moveEvent, System.Action action)
    {
        
        action();
    }
}