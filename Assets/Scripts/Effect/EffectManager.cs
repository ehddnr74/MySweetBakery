using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public EffectPool effectPool;

    public void ShowEffect(string effectName, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, float lifetime)
    {
        effectPool.GetObject(effectName, parent, localPosition, localRotation, localScale, lifetime);
    }
}
