using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerEnvironment : MonoBehaviour {

	[Tooltip("How long the script takes to fully start. This is used to fade in animations and sounds, etc.")]
	public float StartTime = 0.0f;
	[Tooltip("How long the script takes to fully stop. This is used to fade out animations and sounds, etc.")]
	public float StopTime = 0.0f;
	[Tooltip("How long the effect lasts. Once the duration ends, the script lives for StopTime and then the object is destroyed.")]
	public float Duration = 5.0f;
	public ParticleSystem[] ManualParticleSystems;

	private float startTimeMultiplier = 0;
	private float startTimeIncrement = 0;
	private float stopTimeMultiplier = 0;
	private float stopTimeIncrement = 0;
		
	// Update is called once per frame
	protected virtual void Update()
	{
		// reduce the duration
		Duration -= Time.deltaTime;
		if (Stopping)
		{
			// increase the stop time
			stopTimeIncrement += Time.deltaTime;
			if (stopTimeIncrement < StopTime)
			{
				StopPercent = stopTimeIncrement * stopTimeMultiplier;
			}
		}
		else if (Starting)
		{
			// increase the start time
			startTimeIncrement += Time.deltaTime;
			if (startTimeIncrement < StartTime)
			{
				StartPercent = startTimeIncrement * startTimeMultiplier;
			}
			else
			{
				Starting = false;
			}
		}
		else if (Duration <= 0.0f)
		{
			// time to stop, no duration left
			Stop();
		}
	}

	public virtual void Stop()
	{
		if (Stopping)
		{
			return;
		}
		Stopping = true;

		// cleanup particle systems
		foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
		{
			p.Stop();
		}

		StartCoroutine(CleanupEverythingCoRoutine());
	}

	private IEnumerator CleanupEverythingCoRoutine()
	{
		// 2 extra seconds just to make sure animation and graphics have finished ending
		yield return new WaitForSeconds(StopTime + 2.0f);

		//GameObject.Destroy(gameObject);
	}

	private void StartParticleSystems()
	{
		foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>()) {
			if (ManualParticleSystems == null || ManualParticleSystems.Length == 0 ||
			    System.Array.IndexOf (ManualParticleSystems, p) < 0) {
				/*if (p.main.startDelay == 0.0f) {
					// wait until next frame because the transform may change
					p.main.startDelay = 0.01f;
				}*/
				p.Play ();
			}
		}
	}

	public bool Starting
	{
		get;
		private set;
	}

	public float StartPercent
	{
		get;
		private set;
	}

	public bool Stopping
	{
		get;
		private set;
	}

	public float StopPercent
	{
		get;
		private set;
	}
}
