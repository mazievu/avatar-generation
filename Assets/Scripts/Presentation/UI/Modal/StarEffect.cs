using UnityEngine;

namespace LifeSim.Presentation.UI.Effects
{
    public class StarEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem ps;
        public void Play(){ if(ps) ps.Play(); }
    }
}