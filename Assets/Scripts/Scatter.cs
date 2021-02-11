using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scatter : MonoBehaviour
{

    [Header("Settings")]
    public int amount = 1;
    public float spread;
    public GameObject prefab;

    
    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(prefab, this.transform.position, this.transform.rotation).transform.Translate(new Vector3((Random.value-0.5f) * spread, (Random.value - 0.5f) * spread, (Random.value - 0.5f) * spread));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
