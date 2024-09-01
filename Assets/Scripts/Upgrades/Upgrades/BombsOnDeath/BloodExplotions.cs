using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BloodExplotionUpgrade", menuName = "Upgrades/BloodExplotionUpgrade")]

public sealed class BloodExplotions : Upgrade
{
    [Header("SpawnSettings")]
    [Range(0, 100)] [SerializeField] private int _chanseToSpawn; 
    [SerializeField] private float _dropStrength;

    [Header("ExplotionSettings")]
    [SerializeField] private Explotion _bloodExplotion;
    [SerializeField] private float _explotionDamage;
    [SerializeField] private float _explotionRadius;
    [SerializeField] private List<Effect> _effectsToApply;

    public override void Apply()
    {
        FindObjectOfType<EnemySpawnerSystem>().EnemyDied.AddListener(TryToSpawnExplotion);
    }

    private void TryToSpawnExplotion(EnemyHealth enemyHealth)
    {
        if (_chanseToSpawn > Random.Range(0, 100))
        {
            Explotion currentExplotion = Instantiate(_bloodExplotion, enemyHealth.transform.position, Quaternion.identity);

            currentExplotion.SetEffects(_effectsToApply);
            currentExplotion.SetExplotionDamage(_explotionDamage);
            currentExplotion.SetExplotionRaduis(_explotionRadius);

            currentExplotion.AwaitExplode(1.25f);

            currentExplotion.GetComponent<Rigidbody>().AddForce(Vector3.up * 1.25f + new Vector3(Random.Range(-_dropStrength, _dropStrength), 0f, Random.Range(-_dropStrength, _dropStrength)), ForceMode.Impulse);
        }
    }
}
