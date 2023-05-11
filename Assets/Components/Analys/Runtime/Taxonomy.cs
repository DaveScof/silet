using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Taxonomy
{
    public string name;
    public string id;
    
    [SerializeField]
    List<Reporter> reporters;

    public List<Reporter> Reporters => reporters;

}

