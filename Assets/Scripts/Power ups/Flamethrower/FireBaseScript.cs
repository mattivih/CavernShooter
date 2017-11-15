using UnityEngine;
using System.Collections.Generic;
using System.Collections;

    [System.Serializable]
    public struct RangeOfIntegers
    {
        public int Minimum;
        public int Maximum;
    }

    [System.Serializable]
    public struct RangeOfFloats
    {
        public float Minimum;
        public float Maximum;
    }

    public class FireBaseScript : MonoBehaviour
    {
        /*[Tooltip("How long the effect lasts. Once the duration ends, the script lives for StopTime and then the object is destroyed.")]
        public float Duration = 3.0f;*/

        [Tooltip("Particle systems that must be manually started and will not be played on start.")]
        public ParticleSystem[] ManualParticleSystems;
		
		private float startTimeMultiplier;
        private float startTimeIncrement;

        private float stopTimeMultiplier;
        private float stopTimeIncrement;

        private GameObject p;

		private IEnumerator CleanupEverythingCoRoutine()
        {
            // 2 extra seconds just to make sure animation and graphics have finished ending
            yield return new WaitForSeconds(0.0f);

            GameObject.Destroy(gameObject);
        }

        private void StartParticleSystems()
        {
            foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
            {
                if (ManualParticleSystems == null || ManualParticleSystems.Length == 0 ||
                    System.Array.IndexOf(ManualParticleSystems, p) < 0)
                {
                    p.Play();
                }
            }
        }

        protected virtual void Awake()
        {
            Starting = true;
            int fireLayer = UnityEngine.LayerMask.NameToLayer("FireLayer");
            UnityEngine.Physics.IgnoreLayerCollision(fireLayer, fireLayer);
        }

        protected virtual void Start()
        {
			StartParticleSystems();

            // If we implement the ICollisionHandler interface, see if any of the children are forwarding
            // collision events. If they are, hook into them.
            ICollisionHandler handler = (this as ICollisionHandler);
            if (handler != null)
            {
                FireCollisionForwardScript collisionForwarder = GetComponentInChildren<FireCollisionForwardScript>();
                if (collisionForwarder != null)
                {
					Debug.Log ("collision");
                    collisionForwarder.CollisionHandler = handler;
                }
            }
        }

    [PunRPC]
    public void UnSetParent(int viewId)
    {
        PhotonView.Find(viewId).gameObject.GetComponent<ParticleSystem>().emissionRate = 0;
    }


        public virtual void Stop()
        {
            // cleanup particle systems
            foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
            {
            if(p.gameObject.GetPhotonView())
            {
               gameObject.GetComponent<PhotonView>().RPC("UnSetParent", PhotonTargets.All, p.gameObject.GetPhotonView().viewID);
             

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
