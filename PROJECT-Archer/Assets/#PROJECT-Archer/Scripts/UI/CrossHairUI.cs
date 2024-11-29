using UnityEngine;

namespace Archer
{
    public class CrossHairUI : UIBase
    {
        public static CrossHairUI Instance => UIManager.Singleton.GetUI<CrossHairUI>(UIList.CrossHairUI);

        public bool IsDrawCrosshair { set => crossHair.SetActive(value); }

        [SerializeField] GameObject crossHair;
        UnityEngine.UI.Image[] images;

        public GameObject CrossHair => crossHair;
        Vector3 zoomedScale = Vector3.one * 3f;


        public void Awake()
        {
            if (crossHair == null)
            {
                crossHair = transform.GetChild(0).gameObject;
            }
            images = crossHair.GetComponentsInChildren<UnityEngine.UI.Image>();
        }

        public void Update()
        {
            crossHair.transform.localScale = (CameraSystem.Instance.IsZoom ? zoomedScale : Vector3.one) + 0.2f * Mathf.Cos(Time.time * 5f) * Vector3.one;
        }

        public void SetCrosshairColor(Color color)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = color;
            }
        }
    }
}
