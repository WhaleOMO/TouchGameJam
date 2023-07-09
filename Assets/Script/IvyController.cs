using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject initialObject;
    public GameObject ivyObject;

    public void ActivateIvy()
    {
        initialObject.SetActive(false);
        ivyObject.SetActive(true);
    }
}
