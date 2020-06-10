using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public enum CarSituation
    {
        CarIsThere = 0x00001,
        CarWillChangeLaneLeft = 0x00010,
        CarWillChangeLaneRight = 0x00100,
        Trigger = 0x01000,
        TriggerTooClose = 0x10000,
        SlowDown = 0x00111,
    }

    [RequireComponent(typeof(BoxCollider))]
    public class CarCollider : MonoBehaviour
    {
        public BoxCollider Trigger;
        public CarSituation Situation;
        public bool NeedToSlowDown => Active && (Situation | CarSituation.SlowDown) == CarSituation.SlowDown;
        public bool Active
        {
            get => Trigger.enabled;
            set => Trigger.enabled = value;
        }
        protected virtual void OnStart()
        {
            if (Trigger == null) Trigger = GetComponent<BoxCollider>();
            Trigger.isTrigger = true;
            if (Car == null) Car = GetComponentInParent<CarMovement>();

        }

        void Start()
        {
            OnStart();
        }

        public Action<Collider> TriggerEntered;
        public Action<Collider> TriggerExitted;
        public Action<Collider> TriggerStayed;
        public CarMovement Car;

        public float Speed => Car.speed;
        public Vector3 WorldFront => Trigger.bounds.center + new Vector3(Trigger.bounds.extents.x, 0, 0);
        public Vector3 LocalFront => transform.worldToLocalMatrix.MultiplyPoint(WorldFront);

        public Vector3 WorldBack => Trigger.bounds.center - new Vector3(Trigger.bounds.extents.x, 0, 0);
        public Vector3 LocalBack => transform.worldToLocalMatrix.MultiplyPoint(WorldBack);

        public float LocalLength => Vector3.Distance(LocalBack, LocalFront);
        public float WorldLength => Vector3.Distance(WorldBack, WorldFront);

        bool ValidCollider(Collider collider)
        {
            if (collider.TryGetComponent(out CarCollider c))
            {
                return c.Car != Car;
            }
            return true;
        }

        public float GetBackToBackPercent(CarCollider collider)
        {
            return GetBackToBackDistance(collider) / LocalLength;
        }

        public float GetBackToBackDistance(CarCollider collider)
        {
            var localOtherPosition = transform.worldToLocalMatrix.MultiplyPoint(collider.WorldBack);
            return localOtherPosition.x - LocalBack.x;
        }

        protected void OnTriggerEntered(Collider collider)
        {
            Action<Collider> handler = TriggerEntered;
            handler?.Invoke(collider);
        }

        protected void OnTriggerExitted(Collider collider)
        {
            Action<Collider> handler = TriggerExitted;
            handler?.Invoke(collider);
        }

        protected void OnTriggerStayed(Collider collider)
        {
            Action<Collider> handler = TriggerStayed;
            handler?.Invoke(collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            /*if (ValidCollider(other))*/ OnTriggerEntered(other);
        }

        private void OnTriggerExit(Collider other)
        {
            /*if (ValidCollider(other))*/ OnTriggerExitted(other);
        }

        private void OnTriggerStay(Collider other)
        {
            /*if (ValidCollider(other))*/ OnTriggerStayed(other);
        }
    }
}
