using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public class IndicatorUI : UIBase
    {
        public static IndicatorUI Instance => UIManager.Singleton.GetUI<IndicatorUI>(UIList.IndicatorUI);

        public IndicatorUI_Item indicatorItemPrefab;

        private Dictionary<Transform, IndicatorUI_Item> indicators = new Dictionary<Transform, IndicatorUI_Item>();

        public void AddIndicator(Transform target)
        {
            if (indicators.ContainsKey(target)) return;

            IndicatorUI_Item cloneIndicatorItem = Instantiate(indicatorItemPrefab, transform);
            cloneIndicatorItem.gameObject.SetActive(true);
            cloneIndicatorItem.Target = target;
            indicators.Add(target, cloneIndicatorItem);
        }

        public void RemoveIndicator(Transform target)
        {
            if (indicators.ContainsKey(target)) return;

            Destroy(indicators[target].gameObject);
            indicators.Remove(target);
        }

        private void Awake()
        {
            indicatorItemPrefab.gameObject.SetActive(false);
        }

        private void Update()
        {
            foreach (var indicator in indicators)
            {
                indicator.Value.UpdateIndicator();
            }
        }
    }
}
