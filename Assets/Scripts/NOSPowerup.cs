using UnityEngine;

public class NOSPowerup : MonoBehaviour
{
    public int chargesToAdd = 1;        // number of charges added
    public bool autoActivate = true;    // if true, activates NOS immediately after pickup

    public bool playEffect_OnPickup = false; // whether to play an effect when picked up
    public GameObject pickupEffect; // prefab for the pickup effect
    public bool offNOS = false;
    public bool objectToOff = false;
    public GameObject[] objectToDisable; // object to disable when picked up
    private RCC_CarControllerV3 carinside;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("NOSPowerup: OnTriggerEnter with " + other.transform.root.name +
        //other.transform.root.GetComponent<RCC_CarControllerV3>());

        // Check if the other object is the player's car

        RCC_CarControllerV3 car = other.transform.root.GetComponent<RCC_CarControllerV3>();
        if (car != null && car.useNOS)
        {
            carinside = car;
            car.AddNOSCharges(chargesToAdd);
            if (autoActivate)
                car.ActivateNOS();
            if(playEffect_OnPickup)
            {
                if (pickupEffect != null)
                {
                    pickupEffect.SetActive(true);
                }
            }
            if(offNOS)
            {
                Invoke(nameof(OffNOS), 3f); // delay to allow effect to play
            }
            Invoke(nameof(DisableObjects), 1f); // delay to allow effect to play

            //gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }

    public void OffNOS()
    {
        carinside.ForceStopNOS();
    }

    public void DisableObjects()
    {
        if (objectToOff)
        {
            foreach (GameObject obj in objectToDisable)
            {
                obj.SetActive(false);
            }
        }
    }
}