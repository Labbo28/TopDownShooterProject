using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    //let camera follow target
    public class CameraFollowPlayer : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float lerpSpeed = 2.0f;

        private Vector3 _offset;

        private Vector3 _targetPos;

        private void Start()
        {
            if (target == null) return;

            _offset = transform.position - target.position;
        }

        private void Update()
        {
            if (target == null) return;

            _targetPos = target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, _targetPos, lerpSpeed * Time.deltaTime);
          }

    }
