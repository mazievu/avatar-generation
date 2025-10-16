using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace LifeSim.Presentation.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ComicPanel : ModalBase
    {
        [SerializeField] Image overlay;
        [SerializeField] RectTransform content;
        [SerializeField] Button btnClose;
        [SerializeField] TMP_Text txtTitle;
        [SerializeField] TMP_Text txtBody;

        CanvasGroup _cg;

        void Awake(){ _cg = GetComponent<CanvasGroup>(); if (btnClose) btnClose.onClick.AddListener(Close); }
        public void SetText(string title, string body){ if (txtTitle) txtTitle.text=title; if (txtBody) txtBody.text=body; }
        public override void Open(){ gameObject.SetActive(true); StopAllCoroutines(); StartCoroutine(Fade(0f,1f,true)); }
        public override void Close(){ StopAllCoroutines(); StartCoroutine(Fade(_cg.alpha,0f,false)); }

        IEnumerator Fade(float a, float b, bool open)
        {
            _cg.blocksRaycasts = open;
            float t=0f, dur=0.2f;
            while(t<dur){ t+=Time.unscaledDeltaTime; _cg.alpha=Mathf.Lerp(a,b,t/dur);
                if (content) content.localRotation = Quaternion.Euler(0,0,Mathf.Lerp(open?5f:0f,0f,t/dur));
                yield return null;
            }
            _cg.alpha=b; if(!open) gameObject.SetActive(false);
        }
    }
}