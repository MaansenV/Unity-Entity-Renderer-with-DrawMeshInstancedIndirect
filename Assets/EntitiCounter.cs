using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntitiCounter : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CountEntities()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //Query all entities with the SpriteData component
        var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<SpriteData>());
        //Get the number of entities in the query
        int entityCount = query.CalculateEntityCount();
        text.SetText(entityCount.ToString());
    }
}