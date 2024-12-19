using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Obstacles
{
    [Serializable]
    public struct CirclePivotUnit
    {
        public float Speed;
        public float Radius;
        public float Phase;
        public Vector3 Center;
        public Transform Pivot;
    }

    public class LaserObstacle : BaseObstacle
    {
        [Space(10)]
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private LineRenderer laserRay;
        [SerializeField] private LayerMask playerLayer;

        [Header("Movement")]
        [SerializeField] private CirclePivotUnit pivotAUnit;
        [SerializeField] private CirclePivotUnit pivotBUnit;

        private float _timer = 0;

        public override void OnAwake()
        {
            base.OnAwake();
            SetLaserActive(true);
        }

        public override void DamageCharacter(Collision collision) { }

        public override void ExitDamage(Collision collision) { }

        public override void ObstacleAction()
        {
            LaserMovement();
            UpdateLaserRay();
            DamagePlayer();
        }

        private void LaserMovement()
        {
            _timer += Time.deltaTime;
            MoveCirclePivot(pivotAUnit);
            MoveCirclePivot(pivotBUnit);
            LaserRotation();
        }

        private void LaserRotation()
        {
            Quaternion lookRotation = Quaternion.Euler(0, 90, 0);
            Vector3 offset = pivotBUnit.Pivot.position - pivotAUnit.Pivot.position;

            Quaternion aRotation = Quaternion.LookRotation(lookRotation * offset);
            Quaternion bRotation = Quaternion.LookRotation(lookRotation * -offset);

            pivotAUnit.Pivot.rotation = aRotation;
            pivotBUnit.Pivot.rotation = bRotation;
        }

        private void MoveCirclePivot(CirclePivotUnit circlePivot)
        {
            float x = circlePivot.Radius * Mathf.Cos(_timer * circlePivot.Speed + Mathf.Deg2Rad * circlePivot.Phase);
            float z = circlePivot.Radius * Mathf.Sin(_timer * circlePivot.Speed + Mathf.Deg2Rad * circlePivot.Phase);
            Vector3 position = new Vector3(x, 0, z) + circlePivot.Center + transform.position;
            circlePivot.Pivot.position = position;
        }

        private void UpdateLaserRay()
        {
            laserRay.SetPosition(0, pointA.position);
            laserRay.SetPosition(1, pointB.position);
        }

        private void DamagePlayer()
        {
            RaycastHit playerHit;
            if (Physics.Linecast(pointA.position, pointB.position, out playerHit, playerLayer))
            {
                if (playerHit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeHealthDamage(new HealthDamage
                    {
                        DamageAmount = 1,
                        DamageType = DamageType.Energy
                    });
                }
            }
        }

        private void SetLaserActive(bool active)
        {
            laserRay.gameObject.SetActive(active);
            pointA.gameObject.SetActive(active);
            pointB.gameObject.SetActive(active);
        }
    }
}
