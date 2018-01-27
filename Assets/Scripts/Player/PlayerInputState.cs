using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayerInputState
{
    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    public bool JumpDown { get; set; }
    public bool JumpUp { get; set; }
    public float Broadcast { get; set; }
}