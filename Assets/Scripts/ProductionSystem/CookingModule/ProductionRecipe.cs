using System;
using System.Collections.Generic;

/// <summary>
/// Õº÷Ω¿‡
/// </summary>
[Serializable]
public class ProductionRecipe
{
    public int id;
    public string name;
    public string description;

    public float duration;
    public bool require_fuel;

    public List<ItemRequirement> inputs;
    public List<ItemProduction> outputs;
}
