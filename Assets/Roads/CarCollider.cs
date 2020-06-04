using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    [RequireComponent(typeof(BoxCollider))]
    public class CarCollider : MonoBehaviour
    {
        public CarMovementInfo Info;
        public BoxCollider Trigger;
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

        bool ValidCollider(Collider collider)
        {
            if (collider.TryGetComponent(out CarCollider c))
            {
                return c.Car != Car;
            }
            return true;
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
            Debug.Log("Trigger entered");
            if (ValidCollider(other)) OnTriggerEntered(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Trigger exitted");
            if (ValidCollider(other))  OnTriggerExitted(other);
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    Debug.Log("Trigger stay");
        //    OnTriggerStayed(other);
        //}
    }
}
