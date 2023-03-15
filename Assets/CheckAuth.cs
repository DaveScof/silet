using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAuth : MonoBehaviour
{
    private void Start()
    {
        KinetManager.Instance.AddListenerForSubscription(Checking);
    }

    public void Checking()
    {
        this.gameObject.SetActive(false);
    }
}
