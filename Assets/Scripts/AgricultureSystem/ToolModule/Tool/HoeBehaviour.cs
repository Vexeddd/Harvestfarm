using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoeBehaviour : BaseToolBehaviour
{
    protected override int Layer => LayerMask.GetMask("Farmland");
}
