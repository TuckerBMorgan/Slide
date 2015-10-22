using UnityEngine;
using System.Collections;

public class BasicAttack : CharacterAction {

    public enum RangeType
    {
        Melee,
        Ranged
    }


    private SlideCharacter character;
    private RangeType range;

    public void Setup(RangeType range, SlideCharacter character)
    {
        this.range = range;
    }

    public bool Validate(SlideCharacter slide)
    {
        

        return false;
    }

    public void PreformAttack(SlideCharacter character)
    {

        var damageEvent = new RuneManager.DamageEvent(this.character, character, this.character.Damage);
    }

    public override void StartAction()
    {
    }

    public override void EndAction()
    {
        
    }
}
