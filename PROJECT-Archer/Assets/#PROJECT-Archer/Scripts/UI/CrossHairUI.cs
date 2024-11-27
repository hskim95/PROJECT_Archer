using UnityEngine;

namespace Archer
{
    public class CrossHairUI : UIBase
    {
        [SerializeField] CharacterBase character;
        [SerializeField] GameObject crossHair;
        public GameObject CrossHair => crossHair;
        Vector3 zoomedScale = Vector3.one * 3f;


        public void Awake()
        {
            character = FindAnyObjectByType<CharacterBase>();
            crossHair = crossHair != null ? crossHair : transform.GetChild(0).gameObject;
        }

        public void Update()
        {
            if (crossHair.activeSelf != character.IsEquipDone)
            {
                crossHair.SetActive(character.IsEquipDone);
            }
            crossHair.transform.localScale = (CameraSystem.Singleton.IsZoom ? zoomedScale : Vector3.one) + 0.2f * Mathf.Cos(Time.time * 5f) * Vector3.one;
        }
    }
}
