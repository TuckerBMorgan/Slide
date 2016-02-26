using UnityEngine;
using System.Collections;

public class MoveAction : CharacterAction {


    public SlideCharacter character;
    public int moveRange;

    public void Setup(SlideCharacter character, int moveRange)
    {
        this.character = character;
        this.moveRange = moveRange;
    }

    public override bool ValidateSelection(Entity entity)
    {
        if (entity.GetEntityType() != "Tile")
            return false;

        Tile tile = entity as Tile;

        if (tile.Occupied)
            return false;

        if (!tile.Walkable)
            return false;

        Tile playerTile = character.currentTile;

        int xDif = Mathf.Abs(playerTile.X - tile.X);
        if (xDif > moveRange)
            return false;
        
        int yDif = Mathf.Abs(playerTile.Y - tile.Y);
        if (yDif > moveRange)
            return false;

        if ((xDif + yDif) > moveRange)
            return false;

        return character.GetActionPoints() > 0;
    }

    public override void PreformAction(Entity entity)
    {

        GridController.ResetGridColor();
        var runeSetAp = new RuneManager.SetActionPoint(character.GetActionPoints() - 1, character);
        RuneManager.Singelton.ExecuteRune(runeSetAp);
        
        var moves = GridController.Singelton.GetRunedPath(character, character.currentTile, entity as Tile);
        for(var index = 0;index < moves.Count;index++)
        {
            var t = moves[index];
            RuneManager.Singelton.ExecuteRune(t);
        }
        if(character.GetActionPoints() > 0)
        {
            var presentTiles = new RuneManager.PresentMoveTiles(character);
            RuneManager.Singelton.ExecuteRune(presentTiles);
        }
    }

    public override void StartAction()
    {
        
    }

    public override void EndAction()
    {

    }

}
