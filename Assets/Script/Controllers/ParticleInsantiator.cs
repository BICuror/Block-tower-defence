using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInsantiator : MonoBehaviour
{
    [SerializeField] private GameObject _particl;

    public void InstantiateParticle(GameObject gameObj)
    {
        Instantiate(_particl, gameObj.transform.position - new Vector3(0f, 0.5f, 0f), Quaternion.identity);
    }
}
