using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBox : MonoBehaviour
{
    string[] foodPrefabs = new string[] {
        "f_carne",
        "f_f_carne",
        "f_c_carne",
        "f_cf_carne",
        "f_pc_carne",
        "f_pcf_carne",
        "f_queso",
        "f_c_queso",
        "f_pc_queso",
        "f_pan",
        "f_pc_pan",
        "f_c_pan",
        "f_p_hambur",
        "f_p_hamburQueso"
    };
    public int food = 0;
    
    Transform holoZone;
    Vector3 holoZonePos;
    Quaternion holoZoneRot;

    void Start() {
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
        GameObject gameObject = Instantiate(Resources.Load("Prefabs/food/" + foodPrefabs[food]), holoZonePos, holoZoneRot) as GameObject;
        return gameObject;
    }
}
