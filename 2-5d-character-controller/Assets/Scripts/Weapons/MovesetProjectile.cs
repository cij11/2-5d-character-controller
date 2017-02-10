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
		this.transform.SetParent(characterTransform);
    }

	protected override void SetupSprite(){
		BoxCollider boxCollider = GetComponent<BoxCollider>() as BoxCollider;
		spriteTransform.localScale = boxCollider.size; 
        print(direction);
		if(direction == -1){
			spriteTransform.GetComponent<SpriteRenderer>().flipX = true;
		}
	}

    void OnTriggerEnter(Collider other)
    {
        CharacterCorpus corpus = other.GetComponent<CharacterCorpus>() as CharacterCorpus;
        if (corpus != null)
        {
            Character otherCharacter = other.gameObject.GetComponent<Character>() as Character;
			if (!otherCharacter.Equals(componentData.GetCharacter()))
            {
                corpus.TakeDamage(damage);
                corpus.TakeKnockback(worldLaunchVector, knockbackSpeed);
            }
        }
    }
}
