using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Platformer
{
    public class CarrotPowerUp : MonoBehaviour
    {

        void OnTriggerEnter2D(Collider2D other)
        {

            PlayerController player = other.transform.GetComponent<PlayerController>();


            if (other.gameObject.CompareTag("Player"))
            {
                player.SpeedUpEnabled();
                Destroy(this.gameObject);
            }
        }
    }
}