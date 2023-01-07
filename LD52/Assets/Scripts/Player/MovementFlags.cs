using System;

namespace LD52.Player
{
    [Flags]
    public enum MovementFlags : short
    {
        None = 0,
        Dash = 1,
        WallJump = 2,
        Roll = 4
    }
}