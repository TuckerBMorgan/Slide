using UnityEngine;
using System.Collections;

public enum characterStatus
{
    normal,
    dazed,
    stunned,
    rage,
}

public enum stats
{
    health,
    armour,
    apcost,
    range,
    basicDamage,
    moveSpeed,
}

public class CharacterState {

    private int health;
    public int Health { get { return health; } set { health = value; } }

    private int armour;
    public int Armour { get { return armour; } set { armour = value; } }

    private int apcost;
    public int APCost { get { return apcost; } set { apcost = value; } }

    private int range;
    public int Range { get { return range; } set { range = value; } }

    private int basicDamage;
    public int BasicDamage { get { return basicDamage; } set { basicDamage = value; } }

    private int moveSpeed;
    public int MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    private characterStatus status;
    public characterStatus Status { get { return status; } set { status = value; } }

}
