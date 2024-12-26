using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightStock : MonoBehaviour
{
    public int currentStock { get; private set; }

    private List<CanvasGroup> lightStocks = new List<CanvasGroup>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStock = transform.childCount;
        for (int i = 0; i < currentStock; i++)
        {
            lightStocks.Add(transform.GetChild(i).GetComponent<CanvasGroup>());
        }
    }

    public void UseLight()
    {
        if (currentStock == 0) return;
        lightStocks[currentStock - 1].alpha = 0;
        currentStock--;
    }
}
