using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {
	protected CharacterComponentData componentData;

	public AudioClip clip;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void CastEffect(CharacterComponentData importComponentData){
		componentData = importComponentData;

		PlayCastSound ();
	}

	void PlayCastSound(){
		if (clip != null) {
	//		if (audioSource.clip != null){
	//			audioSource.PlayOneShot (audioSource.clip);
	//		}
			SoundEffectPlayer.instance.PlaySoundClip(clip);
		}
	}
}
