using UnityEngine;

namespace LifeSim.Presentation.UI.Effects
{
    public class SmokeEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem ps;
        public void Play(Vector3 pos)
        {
            if (!ps) return;
            ps.transform.position = pos;
            ps.Play();
        }
    }
}
