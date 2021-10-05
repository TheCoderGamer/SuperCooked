using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plato : MonoBehaviour {
    // Variables
    PlayerManager pm;
    Collider food;
    public bool crafting = false;
    public bool platoExtra2 = false;
    [SerializeField]private string thisType = "u_plato";
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

    public bool CraftLogic(Collider _food) {
        food = _food;
        string name = food.name;
        
        // ---- Unir a plato vacio ----
        if(thisType == "u_plato") {
            // Carne cortada
            if(name.StartsWith(foodPrefabs[2])) { Craft(4, false); }
            // Carne cortada cocinada
            else if (name.StartsWith(foodPrefabs[3])) { Craft(5, false); }
            // Pan cortado
            else if (name.StartsWith(foodPrefabs[11])) { Craft(10, false); }
            // Queso cortado
            else if (name.StartsWith(foodPrefabs[7])) { Craft(8, false); }
        }
        // ---- Unir 1er Ingrediente en plato ----
        // Carne cortada cocinada plato
        else if (thisType == foodPrefabs[5]) {
            // Pan cortado
            if (name.StartsWith(foodPrefabs[10])) { Craft(12, true); }
            else if (name.StartsWith(foodPrefabs[11])) { Craft(12, false); }
        } 
        // Pan cortado plato
        else if (thisType == foodPrefabs[10]) {
            // Carne cortado cocinada
            if (name.StartsWith(foodPrefabs[5])) { Craft(12, true); }
            else if (name.StartsWith(foodPrefabs[3])) { Craft(12, false); }
        } 
        // Queso cortado plato
        else if (thisType == foodPrefabs[8]) {
            return false;
        }
        // Hamburguesa
        else if (thisType == foodPrefabs[12]) {
            // Queso cortado
            if (name.StartsWith(foodPrefabs[8])) { Craft(13, true); }
            else if (name.StartsWith(foodPrefabs[7])) { Craft(13, false); }
        }
        else { crafting = false; return false; }
        return true;
    }
    private void Craft(int result, bool platoExtra) {
        Plato plato = food.GetComponent<Plato>();
        if(plato != null) {
            plato.crafting = true;        
        }
        Pickable pk = this.GetComponent<Pickable>();
        GameObject _gameObject = Instantiate(Resources.Load("Prefabs/food/" + foodPrefabs[result]), transform.position, transform.rotation) as GameObject;
        if(pk != null) {
            if(pk.table != null) {
                _gameObject.GetComponent<Pickable>().table = pk.table;
                pk.table.GetComponent<Table>().colOnTable = _gameObject.GetComponent<Collider>();
            }
        }
        // Plato Extra
        if (platoExtra) {
            if (platoExtra2) {
                Instantiate(Resources.Load("Prefabs/utils/u_plato"), this.transform.position, this.transform.rotation);
            }
            else {
                Instantiate(Resources.Load("Prefabs/utils/u_plato"), food.transform.position, food.transform.rotation);
            }
            platoExtra2 = false;
        }
        // Destruir ingredientes
        Destroy(food.gameObject);
        Destroy(this.gameObject);
    }

    private void Start() {
        pm = FindObjectOfType<PlayerManager>();
    }

    private void OnTriggerStay(Collider col) {
        if (col.gameObject.name.StartsWith("f_") && !pm.holding && !crafting) {
            //crafting = true;
            CraftLogic(col);
        }
    }
}
