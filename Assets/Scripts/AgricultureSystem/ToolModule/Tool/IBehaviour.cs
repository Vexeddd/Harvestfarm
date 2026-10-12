using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviour
{
    void OnEquip(ToolModule toolModule, ToolInstance toolInstance);

    ToolUseResolt Use(ToolInstance toolInstance);
}
