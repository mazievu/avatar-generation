using UnityEngine;
using TMPro;
using System.Collections;

namespace LifeSim.Presentation.UI.Effects
{
    public class IncomeFloat : MonoBehaviour
    {
        [SerializeField] TMP_Text txt;
        public void Show(int amount)
        {
            txt.text = $"+${amount:n0}";
            StartCoroutine(Float());
        }

        IEnumerator Float()
        {
            var start = transform.position;
            var end = start + Vector3.up * 100f;
            var t=0f;
            while (t<1f)
            {
                t+=Time.deltaTime*1.5f;
                transform.position = Vector3.Lerp(start,end,t);
                txt.alpha = 1-t;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
