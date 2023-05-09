using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public List<Effect> _effect = new List<Effect>();

    private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EntityHealth>().GetHurt(damage);

            if (_effect.Count > 0)
            {
                EntityEffectManager effectManager = other.gameObject.GetComponent<EntityEffectManager>();

                for (int i = 0; i < _effect.Count; i++)
                {
                    effectManager.ApplyEffect(_effect[i]);
                }
            }

            gameObject.SetActive(false);
        }
        else if (other.gameObject.tag == "Terrain")
        {
            gameObject.SetActive(false);
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
