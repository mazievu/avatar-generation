using System;

namespace LifeSim.Presentation.UI
{
    /// <summary>
    /// A data structure to define a single option in the generic ChoiceModal.
    /// </summary>
    public class ChoiceOptionData
    {
        public string Text { get; set; }
        public Action OnClick { get; set; }
        public bool IsInteractable { get; set; } = true;

        public ChoiceOptionData(string text, Action onClick, bool isInteractable = true)
        {
            Text = text;
            OnClick = onClick;
            IsInteractable = isInteractable;
        }
    }
}
