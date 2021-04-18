using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiSequence : MonoBehaviour
{
	public List<ParticleSystem> effects;
	public bool sequenceComplete;
	private int particlesCount;
	private int count;
	public void RandomShoot()
	{
		if (effects == null || effects.Count == 0)
			return;
		particlesCount = effects.Count;

		effects.Shuffle();
		foreach(ParticleSystem particle in effects)
		{
			StartCoroutine(EffectPlay(UnityEngine.Random.Range(0, 0.5f),particle));
		}
	}

	IEnumerator EffectPlay(float interval,ParticleSystem particle)
	{
		yield return new WaitForSeconds(interval);
		particle.Play();
		var main = particle.main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	private void OnParticleSystemStopped()
	{
		count++;
		if(count == particlesCount)
		{
			sequenceComplete = true;
		}
	}
}
