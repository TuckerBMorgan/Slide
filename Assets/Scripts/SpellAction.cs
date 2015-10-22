using UnityEngine;
using System.Collections;

public class SpellAction : CharacterAction {

    public enum TargetType
    {
        Tile,
        Character,
        Either,
        Self
    }

    


    public override void StartAction()
    {
        
    }

    public override void EndAction()
    {
        
    }

    public bool ValidateTargetSelection()
    {



        return false;
    }


}
