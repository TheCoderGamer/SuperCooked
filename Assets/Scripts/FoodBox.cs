using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBox : MonoBehaviour
{
    private bool delay = false;
    string[] foodPrefabs = new string[] {
        "f__carne",
        "f_f_carne",
        "f_c_carne",
        "f_cf_carne",
        "f_pc_carne",
        "f_pcf_carne",
        "f__queso",
        "f_c_queso",
        "f_pc_queso",
        "f__pan",
        "f_pc_pan",
        "f_c_pan",
        "f_p_hambur",
        "f_p_hamburQueso"
    };
    public int food = 0;

    Transform foodParent;
    Transform holoZone;
    Vector3 holoZonePos;
    Quaternion holoZoneRot;

    void Start() {
        foodParent = GameObject.Find("/Stage/Food").transform;
        holoZone = this.transform.GetChild(0);
        holoZonePos = holoZone.position;
        holoZoneRot = holoZone.rotation;
        
        GameObject holograma = Instantiate(Resources.Load("Prefabs/food/" + foodPrefabs[food]), holoZonePos, holoZoneRot) as GameObject;
        Destroy(holograma.gameObject.GetComponent<Rigidbody>());
        Destroy(holograma.gameObject.GetComponent<Pickable>());
        holograma.gameObject.layer = 0;
        holograma.gameObject.tag = "FoodBoxHolo";
        holograma.transform.parent = holoZone;
        holograma.name = "holograma";
    }

    public GameObject Pick() {
        if (delay) { return null; }
        if (foodParent.childCount > 30) { return null; }

        GameObject gameObject = Instantiate(Resources.Load("Prefabs/food/" + foodPrefabs[food]), holoZonePos, holoZoneRot) as GameObject;
        StartCoroutine(Delay());   
        return gameObject;
    }
    IEnumerator Delay() {
        delay = true;
        yield return new WaitForSeconds(1);
        delay = false;
    }
}
