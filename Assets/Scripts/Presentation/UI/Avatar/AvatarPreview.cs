using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LifeSim.Presentation.UI
{
    public class AvatarPreview : MonoBehaviour
    {
        [Header("Refs")]
        public AvatarConfig config;           // kéo asset vào đây
        public RectTransform layerRoot;       // chỗ spawn các Image
        public AvatarAge age = AvatarAge.Adult;

        [Header("Kích thước hiển thị")]
        public Vector2 size = new Vector2(360, 360);

        readonly List<GameObject> _spawned = new();

        public void Redraw()
        {
            if (!config || !layerRoot) return;
            // dọn cũ
            foreach (var go in _spawned) if (go) Destroy(go);
            _spawned.Clear();

            var set = config.GetSet(age);
            // (z, sprite, name)
            var layers = new List<(int z, Sprite s, string n)>();

            // Hair back (sau head)
            if (set.hairBack) layers.Add((config.z_hairBack, set.hairBack, "hairBack"));

            // Facial features
            if (set.features) layers.Add((config.z_features, set.features, "features"));
            if (set.eyes)     layers.Add((config.z_eyes, set.eyes, "eyes"));
            if (set.eyebrows)    layers.Add((config.z_eyebrows, set.eyebrows, "eyebrows"));
            if (set.mouth)    layers.Add((config.z_mouth, set.mouth, "mouth"));

            // Beard (chỉ adult nếu có)
            if (age == AvatarAge.Adult && set.beard)
                layers.Add((config.z_beard, set.beard, "beard"));

            // Hair front + Accessory (lớp trên cùng)
            if (set.hairFront)  layers.Add((config.z_hairFront, set.hairFront, "hairFront"));
            if (set.accessory)  layers.Add((config.z_accessory, set.accessory, "accessory"));

            // sắp xếp theo z rồi vẽ
            layers.Sort((a,b) => a.z.CompareTo(b.z));
            foreach (var (z, sp, name) in layers)
            {
                var go = new GameObject(name, typeof(RectTransform), typeof(Image));
                go.transform.SetParent(layerRoot, false);
                var rt = (RectTransform)go.transform;
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = size;

                var img = go.GetComponent<Image>();
                img.sprite = sp;
                img.preserveAspect = true;
                img.color = Color.white;

                _spawned.Add(go);
            }
        }

        // gọi thử khi bật object trong Editor
        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false) Redraw();
#endif
        }
    }
}