using Meta.XR.MRUtilityKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using static Meta.XR.MRUtilityKit.MRUKAnchor;

namespace Assets.Scripts
{
    public class SceneSpawn : MonoBehaviour
    {
        public Transform rayStartPoint;
        public float rayLength = 5;
        public MRUKAnchor.SceneLabels labelFilter;
        //public MRUKAnchor


        private GameObject _debugCube;
        private GameObject _debugCube2;
        //private OVRCameraRig _cameraRig;
        //public OVRRayHelper RayHelper;
        //public OVRInputModule InputModule;
        //public OVRRaycaster Raycaster;
        //public OVRGazePointer GazePointer;


        void Start()
        {
            Debug.Log("SceneSpawn.Start");


            //_debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //_debugCube.name = "SceneDebugger_Cube";
            //_debugCube.GetComponent<Renderer>().material.color = Color.yellow;
            //_debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //_debugCube.GetComponent<Collider>().enabled = true;
            //_debugCube.SetActive(true);

            _debugCube2 = GameObject.Find("SomeCube");

            //_debugCube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //_debugCube2.name = "SceneDebugger_Cube2";
            //_debugCube2.GetComponent<Renderer>().material.color = Color.red;
            //_debugCube2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //_debugCube2.transform.position = new Vector3(0,0,0);
            //_debugCube2.GetComponent<Collider>().enabled = true;
            //_debugCube2.SetActive(true);

            //_cameraRig = FindObjectOfType<OVRCameraRig>();

            //SetupInteractionDependencies();
        }

        //private void SetupInteractionDependencies()
        //{
        //    if (!_cameraRig)
        //        return;

        //    GazePointer.rayTransform = _cameraRig.centerEyeAnchor;
        //    InputModule.rayTransform = _cameraRig.rightControllerAnchor;
        //    Raycaster.pointer = _cameraRig.rightControllerAnchor.gameObject;
        //    if (_cameraRig.GetComponentsInChildren<OVRRayHelper>(false).Length > 0)
        //        return;
        //    var rightControllerHelper =
        //        _cameraRig.rightControllerAnchor.GetComponentInChildren<OVRControllerHelper>();
        //    if (rightControllerHelper)
        //    {
        //        rightControllerHelper.RayHelper =
        //            Instantiate(RayHelper, Vector3.zero, Quaternion.identity, rightControllerHelper.transform);
        //        rightControllerHelper.RayHelper.gameObject.SetActive(true);
        //    }

        //    var leftControllerHelper =
        //        _cameraRig.leftControllerAnchor.GetComponentInChildren<OVRControllerHelper>();
        //    if (leftControllerHelper)
        //    {
        //        leftControllerHelper.RayHelper =
        //            Instantiate(RayHelper, Vector3.zero, Quaternion.identity, leftControllerHelper.transform);
        //        leftControllerHelper.RayHelper.gameObject.SetActive(true);
        //    }

        //    var hands = _cameraRig.GetComponentsInChildren<OVRHand>();
        //    foreach (var hand in hands)
        //    {
        //        hand.RayHelper =
        //            Instantiate(RayHelper, Vector3.zero, Quaternion.identity, _cameraRig.trackingSpace);
        //        hand.RayHelper.gameObject.SetActive(true);
        //    }
        //}

        void Update()
        {

            Debug.Log("SceneSpawn.Update");
            Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);

            MRUKRoom room = MRUK.Instance.GetCurrentRoom();
            bool hasHit = room.Raycast(ray, rayLength, LabelFilter.FromEnum(labelFilter), out RaycastHit hit, out MRUKAnchor anchor);

            if(hasHit)
            {
                Vector3 hitPoint = hit.point;
                //Vector3 hitNormal = hit.normal;

                _debugCube2.transform.position =  hitPoint;
                //_debugCube.transform.rotation = hitNormal.q;
            }


            //var ray = GetControllerRay();
            //MRUKAnchor sceneAnchor = null;
            //var positioningMethod = MRUK.PositioningMethod.DEFAULT;
            ////if (positioningMethodDropdown)
            ////    positioningMethod = (MRUK.PositioningMethod)positioningMethodDropdown.value;
            //var bestPose = MRUK.Instance?.GetCurrentRoom()?.GetBestPoseFromRaycast(ray, Mathf.Infinity,
            //    new LabelFilter(), out sceneAnchor, positioningMethod);
            //if (bestPose.HasValue && sceneAnchor && _debugCube)
            //{
            //    _debugCube.transform.position = bestPose.Value.position;
            //    _debugCube.transform.rotation = bestPose.Value.rotation;
            //    _debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //    //SetLogsText("\n[{0}]\nAnchor: {1}\nPose Position: {2}\nPose Rotation: {3}",
            //    //    nameof(GetBestPoseFromRaycastDebugger),
            //    //    sceneAnchor.name,
            //    //    bestPose.Value.position,
            //    //    bestPose.Value.rotation
            //    //);
            //}
        }

        //private Ray GetControllerRay()
        //{
        //    Vector3 rayOrigin;
        //    Vector3 rayDirection;
        //    if (OVRInput.activeControllerType == OVRInput.Controller.Touch
        //        || OVRInput.activeControllerType == OVRInput.Controller.RTouch)
        //    {
        //        rayOrigin = _cameraRig.rightHandOnControllerAnchor.position;
        //        rayDirection = _cameraRig.rightHandOnControllerAnchor.forward;
        //    }
        //    else if (OVRInput.activeControllerType == OVRInput.Controller.LTouch)
        //    {
        //        rayOrigin = _cameraRig.leftHandOnControllerAnchor.position;
        //        rayDirection = _cameraRig.leftHandOnControllerAnchor.forward;
        //    }
        //    else // hands
        //    {
        //        var rightHand = _cameraRig.rightHandAnchor.GetComponentInChildren<OVRHand>();
        //        // can be null if running in Editor with Meta Linq app and the headset is put off
        //        if (rightHand != null)
        //        {
        //            rayOrigin = rightHand.PointerPose.position;
        //            rayDirection = rightHand.PointerPose.forward;
        //        }
        //        else
        //        {
        //            rayOrigin = _cameraRig.centerEyeAnchor.position;
        //            rayDirection = _cameraRig.centerEyeAnchor.forward;
        //        }
        //    }

        //    return new Ray(rayOrigin, rayDirection);
        //}
    }
}
