using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiThemeApplier : MonoBehaviour
{
    public UiTheme theme;

    // Main
    public Image bgWholeScreen;
    public Image bgBottomBar;
    public TextMeshProUGUI txtHeaderLeft;   // Date
    public TextMeshProUGUI txtPillMoney;
    public TextMeshProUGUI txtPillIncome;
    public Image pillBg;
    public Image pillStroke;
    public Image btnStoryBg;
    public Image btnSettingsBg;

    public Image modalOverlay;
    public Image modalContentBg;

    public void Apply()
    {
        if (!theme) return;
        if (bgWholeScreen) bgWholeScreen.color = theme.bgScreen;
        if (bgBottomBar)   bgBottomBar.color   = theme.bottomBarBg;

        if (txtHeaderLeft) txtHeaderLeft.color = theme.hudText;
        if (txtPillMoney)  txtPillMoney.color  = theme.hudText;
        if (txtPillIncome) txtPillIncome.color = theme.incomeGreen;
        if (pillBg)        pillBg.color        = theme.pillBg;
        if (pillStroke)    pillStroke.color    = theme.pillStroke;
        if (btnStoryBg)    btnStoryBg.color    = theme.storyBg;
        if (btnSettingsBg) btnSettingsBg.color = theme.settingsBg;

        // Font size
        if (txtHeaderLeft) txtHeaderLeft.fontSize = theme.title;
        if (txtPillMoney)  txtPillMoney.fontSize  = theme.title;
        if (txtPillIncome) txtPillIncome.fontSize = theme.label;
    }
}
