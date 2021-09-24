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
    [SerializeField]private int tableType = 0; // 0 -> normal, 1 -> cortar, 2 -> freir
    private bool occupy = false;
    private Collider colOnTable = null;
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
        // Coger objeto
        if (!occupy && !pm.holding && collider.gameObject.CompareTag("Pickable")) {
            collider.transform.position = zone.position;
            collider.transform.rotation = zone.rotation;
            collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            occupy = true;
            colOnTable = collider;
        }  
    }
    public void Action() { 
        if (tableType > 0 && occupy && !crafting) {
            // Carne -> Carne Cortada Cruda
            if      (tableType == 1 && colOnTable.name.StartsWith("f_carne")) {
                StartCoroutine(Craft(colOnTable, 2));
            }
            // Carne Cortada -> Carne Cortada Frita
            else if (tableType == 2 && colOnTable.name.StartsWith("f_c_carne")) {
                StartCoroutine(Craft(colOnTable, 3));
            }
            // Carne -> Carne Frita
            else if (tableType == 2 && colOnTable.name.StartsWith("f_carne")) {
                StartCoroutine(Craft(colOnTable, 1));
            }
            // Carne Frita -> Carne Cortada Frita
            else if (tableType == 1 && colOnTable.name.StartsWith("f_f_carne")) {
                StartCoroutine(Craft(colOnTable, 3));
            }
            
            // Pan -> Pan Cortado
            else if (tableType == 1 && colOnTable.name.StartsWith("f_pan")) {
                StartCoroutine(Craft(colOnTable, 11));
            }

            // Queso -> Queso Cortado
            else if (tableType == 1 && colOnTable.name.StartsWith("f_queso")) {
                StartCoroutine(Craft(colOnTable, 7));
            }
        }
    }
    // --------- CRAFTEO ----------
    IEnumerator Craft(Collider col, int craft) {
        crafting = true;
        col.gameObject.tag = "Untagged";
        col.gameObject.layer = 6;
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
    }
}

