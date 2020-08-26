using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roads
{
    [ExecuteInEditMode]
    public class CarTraficAlgorithm : MonoBehaviour
    {
        public List<CarMovement> CarMovements;
        // Start is called before the first frame update
        void Start()
        {
            CarMovements = new List<CarMovement>();
            foreach (var car in GameObject.FindGameObjectsWithTag("Car"))
            {
                if (car.TryGetComponent(out CarMovement movement))
                {
                    CarMovements.Add(movement);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}