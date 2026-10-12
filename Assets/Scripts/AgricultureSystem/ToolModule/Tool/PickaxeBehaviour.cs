using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeBehaviour : BaseToolBehaviour
{
    protected override int Layer => LayerMask.GetMask("Rock");
}
