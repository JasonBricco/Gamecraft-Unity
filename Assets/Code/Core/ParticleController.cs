using UnityEngine;

public sealed class ParticleController : MonoBehaviour 
{
	private ParticleSystem system;
	private ParticleSystem.Particle[] particles;

	private void Awake()
	{
		system = GetComponent<ParticleSystem>();
		particles = new ParticleSystem.Particle[system.maxParticles];
	}

	private void Update()
	{
		Vector3 globalPos = Camera.main.transform.position;
		globalPos.y = 140.0f;

		system.transform.position = globalPos;

		int count = system.GetParticles(particles);

		for (int i = 0; i < count; i++)
		{
			Vector3i pos = Utils.GetBlockPos(particles[i].position);
			Block block = Map.GetBlockSafe(pos.x, pos.y, pos.z);

			if (block.BlockParticles())
				block.KillParticle(pos.y, ref particles[i]);
		}

		system.SetParticles(particles, count);
	}
}
