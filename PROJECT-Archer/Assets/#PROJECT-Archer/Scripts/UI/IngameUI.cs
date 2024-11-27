using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Archer
{
    public class IngameUI : UIBase
    {
        public static IngameUI Instance => UIManager.Singleton.GetUI<IngameUI>(UIList.IngameUI);

        public Image hpBar;
        public Image spBar;

        public TextMeshProUGUI hpText;
        public TextMeshProUGUI spText;

        public void SetHP(float current, float max)
        {
            hpBar.fillAmount = current / max;
            hpText.text = $"{current:0.0} / {max:0.0}"; // :0.0Àº Æ÷¸ä ¼³Á¤
        }

        public void SetSP(float current, float max)
        {
            spBar.fillAmount = current / max;
            spText.text = $"{current:0.0} / {max:0.0}";
        }

    }
}
