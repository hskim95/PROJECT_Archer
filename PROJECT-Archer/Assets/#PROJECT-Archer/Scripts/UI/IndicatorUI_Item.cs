using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Archer
{
    public class IndicatorUI_Item : MonoBehaviour
    {
        public Transform Target { get; set; }

        public GameObject insideGroup, outsideGroup;
        public TextMeshProUGUI outsideDistanceText;

        public void UpdateIndicator()
        {
            if (Target == null) return;

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(Target.position);
            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(Target.position);

            bool isInsideView = viewportPosition.x >= 0 && viewportPosition.x <= 1f && viewportPosition.y >= 0 
                && viewportPosition.y <= 1f && viewportPosition.z > 0;

            insideGroup.SetActive(isInsideView);
            outsideGroup.SetActive(!isInsideView);

            if (!isInsideView) // OUTSIDE
            {
                float distance = Vector3.Distance(Camera.main.transform.position, Target.position);
                outsideDistanceText.text = $"{distance:0.00}m";

                // Border Line Attach
                viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.03f, 1 - 0.03f);
                viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.05f, 1 - 0.05f);

                if (viewportPosition.z <= 0)
                {
                    if (viewportPosition.y > 1f)
                    {
                        viewportPosition.y = 0.95f;
                    }
                    else
                    {
                        viewportPosition.y = 0.05f;
                    }
                }

                transform.position = Camera.main.ViewportToScreenPoint(viewportPosition);
            }
            else // INSIDE
            {
                transform.position = screenPosition;
            }
        }
    }
}
