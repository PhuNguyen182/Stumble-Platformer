using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TestParenting : MonoBehaviour
{
    public GameObject test;

    [Button]
    public void ParentButton()
    {
        test.transform.SetParent(transform);
    }

    [Button]
    public void DeparentButton()
    {
        test.transform.SetParent(null);
    }
}
