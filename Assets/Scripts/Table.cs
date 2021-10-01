// ----------- IMPORTANTE ------------
// Tiene que tener un collider trigger
// y que la zona donde se teletrasporta,
// siga tocando el trigger.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour {
    Transform zone;
    Transform zone2;
    PlayerManager pm;
    public bool disable = false;
    [SerializeField]private int tableType = 0; // 0 -> normal, 1 -> cortar, 2 -> freir
    private bool occupy = false;
    public Collider colOnTable = null;
    private Collider sarOnTable = null;
    private bool crafting = false;
    private bool conSarten = false;
    private bool pickCrafted = false;

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
        if (tableType == 2) { zone2 = transform.GetChild(1); }
        pm = FindObjectOfType<PlayerManager>();
        foodContainer = GameObject.Find("/Stage/Food").transform;
    }

    private void OnTriggerStay(Collider collider) {
        if (disable) { return; }

        if(pickCrafted && !pm.holding) {
            PickUp(collider);
            colOnTable = collider;
            occupy = true;
            pickCrafted = false;
        }

        // Coger objeto
        if ((!occupy && !pm.holding && colOnTable != collider && (collider.name.StartsWith("f_")) | (tableType == 0 && collider.name.StartsWith("u_plato")))) {
            PickUp(collider);
            colOnTable = collider;
            occupy = true;
        }
        // Coger sarten como objeto
        if (tableType == 0 && !occupy && !pm.holding && colOnTable != collider && collider.name.StartsWith("u_sarten")) {
            PickUp(collider);
            colOnTable = collider;
            occupy = true;
        }
        // Coger sarten
        if (tableType == 2 && !pm.holding && sarOnTable != collider && collider.name.StartsWith("u_sarten")) {
            PickUp(collider);
            sarOnTable = collider;
            conSarten = true;
        }
        // Unir plato
        if(occupy && tableType == 0 && !pm.holding && collider.name.StartsWith("f_")) {
            Plato plato = colOnTable.GetComponent<Plato>();
            if(plato != null) {
                plato.CraftLogic(collider);
            }
        }
        if (occupy && !pm.holding && collider.name.StartsWith("u_plato") && colOnTable.name.StartsWith("f_")) {
            Plato plato = collider.GetComponent<Plato>();
            if (plato != null) {
                pickCrafted = plato.CraftLogic(colOnTable);
            }
        }


        if (occupy | conSarten) {
            if (colOnTable != null) { colOnTable.tag = "OnTable"; }
            if (sarOnTable != null) { sarOnTable.tag = "OnTable"; }
        }
    }
    private void PickUp (Collider col) {
        if (conSarten) {
            col.transform.position = zone2.position;
            col.transform.rotation = zone2.rotation;
        } else {
            col.transform.position = zone.position;
            col.transform.rotation = zone.rotation;
        }
        col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        col.gameObject.tag = "OnTable";
        col.GetComponent<Pickable>().table = this.gameObject;
    }
    public GameObject Drop() {
        if (!disable) {
            if (!conSarten) {
                sarOnTable = null;
                conSarten = false;
                occupy = false;
                colOnTable.GetComponent<Pickable>().table = null;
                GameObject _conOnTable = colOnTable.gameObject;
                colOnTable = null;
                return _conOnTable;
            }
            if (conSarten && occupy) {
                conSarten = true;
                occupy = false;
                colOnTable.GetComponent<Pickable>().table = null;
                GameObject _conOnTable = colOnTable.gameObject;
                colOnTable = null;
                return _conOnTable;
            }
            if (conSarten && !occupy) {
                colOnTable = null;
                conSarten = false;
                sarOnTable.GetComponent<Pickable>().table = null;
                GameObject _sarOnTable = sarOnTable.gameObject;
                sarOnTable = null;
                return _sarOnTable;
            }
            else { return null; }
        }
        else { return null; }
    }

    // --------- CRAFTEO ----------
    public void Action() {
        if (disable) { return; }
        if (tableType > 0 && occupy && !crafting) {
            // Carne -> Carne Cortada Cruda
            craftLogic("f_carne", 2, 1);

            // Carne Cortada -> Carne Cortada Frita
            craftLogic("f_c_carne", 3, 2);

            // Carne -> Carne Frita
            craftLogic("f_carne", 1, 2);

            // Carne Frita -> Carne Cortada Frita
            craftLogic("f_f_carne", 3, 1);

            // Pan -> Pan Cortado
            craftLogic("f_pan", 11, 1);

            // Queso -> Queso Cortado
            craftLogic("f_queso", 7, 1);
        }
    }
    private void craftLogic(string IN, int OUT, int tableTypeRequired) {
        if (tableType == tableTypeRequired && colOnTable.name.StartsWith(IN)) {
            if(tableTypeRequired == 2 && !conSarten) {
                // Si es una vitroceramica y no tiene sarten no hacer nada
                return;
            }
            crafting = true;
            StartCoroutine(Craft(colOnTable, OUT));
        }
    }
    IEnumerator Craft(Collider col, int craft) {
        col.gameObject.tag = "Untagged";
        col.gameObject.layer = 6;
        yield return new WaitForSeconds(1);
        Vector3 pos = col.transform.position;
        Quaternion rot = col.transform.rotation;
        Destroy(col.gameObject);
        GameObject _clone = Instantiate(Resources.Load("Prefabs/food/" + foodPrefabs[craft]), pos, rot) as GameObject;
        colOnTable = _clone.GetComponent<Collider>();
        crafting = false;
        pm.crafting = false;
        //colOnTable.transform.parent = foodContainer;
        PickUp(colOnTable);
    }
    
    private void OnTriggerExit(Collider col) {
        if (col == colOnTable) {
            occupy = false;
            colOnTable = null;
        }
        if (col == sarOnTable) {
            conSarten = false;
            sarOnTable = null;
        }
    }
}

