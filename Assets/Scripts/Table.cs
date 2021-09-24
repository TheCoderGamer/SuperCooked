// ----------- IMPORTANTE ------------
// Tiene que tener un collider trigger
// y que la zona donde se teletrasporta,
// siga tocando el trigger.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour {
    Transform zone;
    PlayerManager pm;
    public bool disable = false;
    public int tableType = 0; // 0 -> normal, 1 -> cortar, 2 -> freir
    private bool occupy = false;
    private Collider colOnTable = null;
    private bool action1;
    bool isPlayer = false;
    bool crafting = false;

    Transform foodContainer;

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

    private void Start() {
        zone = transform.GetChild(0);
        pm = FindObjectOfType<PlayerManager>();
        foodContainer = GameObject.Find("/Stage/Food").transform;
    }

    private void OnTriggerStay(Collider collider) {
        if (disable) { return; }
        if (!occupy && !pm.holding && collider.gameObject.CompareTag("Pickable")) {
            collider.transform.position = zone.position;
            collider.transform.rotation = zone.rotation;
            collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            occupy = true;
            colOnTable = collider;
        }
        
        // Crafteo
        action1 = Input.GetButton("Action1");
        if (collider.gameObject.CompareTag("Player")) {
            isPlayer = true;
        }
        if (tableType > 0 && occupy && isPlayer && action1 && !crafting) {
            // Carne -> Carne Cortada Cruda
            if (tableType == 1 && collider.name.Equals("f_carne")) {
                StartCoroutine(Craft(collider, 2));
            }
            // Carne Cortada -> Carne Cortada Frita
            else if (tableType == 2 && collider.name.Equals("f_c_carne")) {
                StartCoroutine(Craft(collider, 3));
            }
            // Carne -> Carne Frita
            else if (tableType == 2 && collider.name.Equals("f_carne")) {
                StartCoroutine(Craft(collider, 1));
            }
            // Carne Frita -> Carne Cortada Frita
            else if (tableType == 1 && collider.name.Equals("f_f_carne")) {
                StartCoroutine(Craft(collider, 3));
            }
            
            // Pan -> Pan Cortado
            else if (tableType == 1 && collider.name.Equals("f_pan")) {
                StartCoroutine(Craft(collider, 11));
            }

            // Queso -> Queso Cortado
            else if (tableType == 1 && collider.name.Equals("f_queso")) {
                StartCoroutine(Craft(collider, 7));
            }
        }
    }
    // --------- CRAFTEO ----------
    IEnumerator Craft(Collider col, int craft) {
        col.gameObject.tag = "Untagged";
        col.gameObject.layer = 6;
        crafting = true;
        yield return new WaitForSeconds(1);
        Vector3 pos = col.transform.position;
        Quaternion rot = col.transform.rotation;
        Destroy(col.gameObject);
        GameObject _gameObject = Instantiate(Resources.Load("Prefabs/food/" + foodPrefabs[craft]), pos, rot) as GameObject;
        EndCrafting(_gameObject);
    }

    private void EndCrafting(GameObject gameObject) {
        crafting = false;
        gameObject.transform.position = zone.position;
        gameObject.transform.rotation = zone.rotation;
        gameObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        occupy = true;
        colOnTable = gameObject.GetComponent<Collider>();
        gameObject.transform.parent = foodContainer;
    }
    private void OnTriggerExit(Collider collider) {
        if (disable || colOnTable == null) { return; }
        if (occupy && collider == colOnTable) { // Problema, se ejecuta al menos una vez aunq ya este suelto
            occupy = false;
            colOnTable = null;
        }
        if (collider.gameObject.CompareTag("Player")) {
            isPlayer = false;
        }
    }

}

