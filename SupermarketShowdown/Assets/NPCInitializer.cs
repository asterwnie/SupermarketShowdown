using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInitializer : MonoBehaviour
{
    public Grid grid;
    public int numberToSpawn = 2;
    [HideInInspector] public bool canInit = false; // delay to allow everything else to initialize
    public GameObject npcPrefab;
    public GameObject npcContainer;
    public List<GameObject> npcs = new List<GameObject>();
    public GameObject[] groceries;

    bool gameIsInitted = false;

    int numGroceries;
    int numGroceriesPerNPC = 2;

    // Start is called before the first frame update
    void Start()
    {
        groceries = GameObject.FindGameObjectsWithTag("StoreItem");
        numGroceries = groceries.Length;
        numGroceriesPerNPC = Mathf.FloorToInt(numGroceries / numberToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameIsInitted && GameManager.instance.isFinishedInit)
        {
            gameIsInitted = true;
            canInit = true;
        }

        if(canInit)
        {
            canInit = false; // don't init twice
            StartCoroutine(StaggerNPCSpawns());
        }
    }

    IEnumerator StaggerNPCSpawns()
    {
        // distribute items amongst NPCs
        for (int i = 0; i < numberToSpawn; i ++)
        {

            GameObject npc = Instantiate(npcPrefab, npcContainer.transform);
            Vector3 pos = new Vector3(Random.value * 140 - 70, 0, Random.value * 140 - 70);
            while (!(grid.NodeFromWorldPoint(pos).walkable))
            {
                pos = new Vector3(Random.value * 140 - 70, 0, Random.value * 140 - 70);
            }

            npc.transform.position = pos;

            npcs.Add(npc);
            GenericNPCAIStateMachine npcBrain = npc.GetComponent<GenericNPCAIStateMachine>();
            npcBrain.groceriesToCollect = numGroceriesPerNPC;

            yield return new WaitForSeconds(2f);
        }
    }
}
