using UnityEngine;
using System.Collections;

public class MoveAction : CharacterAction {


    public SlideCharacter character;

    public void Setup(SlideCharacter character)
    {
        this.character = character;
    }

    public override bool ValidateSelection(Entity entity)
    {
        return entity.GetEntityType() == "Tile"  && character.GetActionPoints() > 0;
    }

    public override void PreformAction(Entity entity)
    {
        var moves = GridController.Singelton.GetRunedPath(character, character.currentTile, entity as Tile);
        for(var index = 0;index < moves.Count;index++)
        {
            var t = moves[index];
            RuneManager.Singelton.ExecuteRune(t);
        }

        var runeSetAp = new RuneManager.SetActionPoint(character.GetActionPoints() - 1, character);
        RuneManager.Singelton.ExecuteRune(runeSetAp);

    }

    public override void StartAction()
    {
        
    }

    public override void EndAction()
    {

    }

}
