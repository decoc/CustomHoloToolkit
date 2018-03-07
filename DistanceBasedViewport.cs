using UnityEngine;

namespace HoloToolkit.CustomUtility {
    /// <summary>
    /// 対象との距離ベースのViewScale調整スクリプト
    /// </summary>
    public class DistanceBasedViewport : MonoBehaviour {

        [SerializeField]
        [Tooltip("TargetOverrideを指定すると、Rayによる距離計算は行いません。")]
        private GameObject targetOverride;
        
        [SerializeField]
        [Tooltip("Rayの最長飛距離です")]
        private float maxDistance = 10f;

        [SerializeField]
        [Tooltip("Rayの最短飛距離です")]
        private float minDistance = 0.1f;

        [SerializeField]
        [Tooltip("ViewportScaleの最小値です")]
        private float minViewportSize = 0.5f;

        [SerializeField]
        public float CurrentScale { get; private set; }

        private void OnEnable() {
            CurrentScale = 1.0f;
        }

        private void OnDisable() {
#if UNITY_2017_2_OR_NEWER
            UnityEngine.XR.XRSettings.renderViewportScale = 1.0f;
#else
            UnityEngine.VR.VRSettings.renderViewportScale = 1.0f;
#endif
        }

        private Camera cam;

        // Use this for initialization
        void Start() {
            cam = Camera.main;
        }

        protected void OnPreCull() {

            if(targetOverride != null) {
                ConfigureTransfomOverrideViewScale();
            }
            else {
                ConfigureRayViewScare();
            }

            Debug.LogFormat("ViewScale {0}", CurrentScale);

#if UNITY_2017_2_OR_NEWER
            UnityEngine.XR.XRSettings.renderViewportScale = CurrentScale;
#else
            UnityEngine.VR.VRSettings.renderViewportScale = CurrentScale;
#endif
        }

        /// <summary>
        /// Targetを指定している際のViewportScaleの計算です
        /// </summary>
        private void ConfigureTransfomOverrideViewScale() {
            var viewPos = cam.WorldToViewportPoint(targetOverride.transform.position);
            if( 0 < viewPos.x && viewPos.x < 1 &&
                0 < viewPos.y && viewPos.y < 1 ) {
                var distance = Vector3.Distance(cam.transform.position, transform.position);
            }
            else {
                CurrentScale = 1.0f;
            }
        }

        /// <summary>
        /// RayによるViewScaleの計算です
        /// </summary>
        private void ConfigureRayViewScare() {
            var ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance)) {
                var distance = Vector3.Distance(cam.transform.position, hit.point);
                var clampDistance = Mathf.Clamp(distance, minDistance, maxDistance);
                var lerpVal = Mathf.InverseLerp(minDistance, maxDistance, clampDistance);
                CurrentScale = Mathf.Lerp(minViewportSize, 1.0f, lerpVal);
            }
            else {
                CurrentScale = 1.0f;
            }
        }
    }

}
