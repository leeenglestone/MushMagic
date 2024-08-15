using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneSpawn : MonoBehaviour
    {
        public Transform rayStartPoint;
        public float rayLength = 6;
        public MRUKAnchor.SceneLabels labelFilter;
        private GameObject _debugCube;
        private float lastSpawn;

        [SerializeField]
        private float cooldown = 0.5f;

        void Start()
        {
            _debugCube = GameObject.Find("SomeCube");
        }

        void Update()
        {
            Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);

            MRUKRoom room = MRUK.Instance.GetCurrentRoom();
            bool hasHit = room.Raycast(ray, rayLength, LabelFilter.FromEnum(labelFilter), out RaycastHit hit, out MRUKAnchor anchor);

            if (hasHit)
            {
                Vector3 hitPoint = hit.point;

                _debugCube.transform.position = hitPoint;

                // If right trigger press.. place a cube at that position
                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) ||
                    OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
                {
                    if (Time.time < lastSpawn + cooldown)
                        return;

                    GameObject newCube = GameObject.Instantiate(_debugCube);
                    newCube.transform.position = hitPoint;

                    lastSpawn = Time.time;
                };
            }
        }
    }
}
