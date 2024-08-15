﻿using Meta.XR.MRUtilityKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
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


        private GameObject _debugCube2;
        private GameObject _mush;

        [SerializeField]
        private float cooldown = 0.5f;

        private float lastSpawn;



        void Start()
        {
            Debug.Log("SceneSpawn.Start");

            //_debugCube2 = GameObject.Find("SomeCube");
            _mush = GameObject.Find("Mush");

        }



        void Update()
        {

            Debug.Log("SceneSpawn.Update");
            Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);

            MRUKRoom room = MRUK.Instance.GetCurrentRoom();
            bool hasHit = room.Raycast(ray, rayLength, LabelFilter.FromEnum(labelFilter), out RaycastHit hit, out MRUKAnchor anchor);

            if (hasHit)
            {
                Vector3 hitPoint = hit.point;

                //_debugCube2.transform.position =  hitPoint;
                _mush.transform.position = hitPoint;


                // If right trigger press.. place a cube at that position
                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) ||
                    OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
                {
                    //GameObject newCube = GameObject.Instantiate(_debugCube2);
                    //newCube.transform.position = hitPoint;

                    if (Time.time < lastSpawn + cooldown)
                        return;

                    GameObject newMush = GameObject.Instantiate(_mush);
                    newMush.transform.position = hitPoint;

                    lastSpawn = Time.time;

                };
            }



        }


    }
}
