using UnityEngine;
using System.Collections;

public class MovesetProjectile : Projectile
{

    void Update()
    {
        IncreaseAge();
    }

    public override void Launch()
    {
        LaunchMoveset();
    }

    private void LaunchMoveset()
    {
		this.transform.SetParent(firingWeapon.transform);
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterCorpus corpus = other.GetComponent<CharacterCorpus>() as CharacterCorpus;
        if (corpus != null)
        {
            Character otherCharacter = other.gameObject.GetComponent<Character>() as Character;
            if (!otherCharacter.Equals(firingCharacter))
            {
                corpus.TakeDamage(damage);
                corpus.TakeKnockback(worldLaunchVector, knockbackSpeed);
            }
        }
    }
}
