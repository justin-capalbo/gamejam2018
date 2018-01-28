using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public interface IMover
{
    void Move(float h, float v);

    Vector3 GetSpeed();
}

