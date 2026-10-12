using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingModule : MonoBehaviour
{
    public List<ProductionRecipe> recipes = new List<ProductionRecipe>();

    private void Awake()
    {
        recipes = JsonConvert.DeserializeObject<List<ProductionRecipe>>(Resources.Load<TextAsset>("Data/cooking").text);
    }
}
