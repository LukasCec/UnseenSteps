using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAbilitiesData", menuName = "Data/Player Abilities", order = 1)]
public class PlayerAbilitiesData : ScriptableObject
{
    public bool canDoubleJump = false;
    public bool canDash = false;
    public bool canWallJump = false;
    public bool canWallSlide = false;
}
