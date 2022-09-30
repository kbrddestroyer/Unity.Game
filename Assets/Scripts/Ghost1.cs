using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost1 : Anomaly
{
    [SerializeField] public GameObject[] players;
    [SerializeField, Range(0f, 100f)] private float distanceTreshold;
    [SerializeField, Range(0, 100)] private int huntDuration;
    [SerializeField, Range(0, 100)] private int huntCooldown;
    [SerializeField, Range(0, 100)] private int eventCooldown;
    [SerializeField, Range(0, 100)] private int checkThreshold;

    private float average_mind = 0.0f;

    void CheckPlayer(GameObject player)
    {
        if (Vector3.Distance(transform.position, player.transform.position) < distanceTreshold)
        {
            Player player_ = player.GetComponent<Player>();
            player_.mind -= Random.Range(0.01f, 0.05f);
            if (player_.mind < 0) player_.mind = 0;
        }
    }

    IEnumerator CheckPlayers()
    {
        while (true)
        {
            average_mind = 0.0f;
            for (int i = 0; i < players.Length; i++)
            {
                CheckPlayer(players[i]);
                average_mind += players[i].GetComponent<Player>().mind;
            }
            average_mind /= players.Length;
            Debug.Log("Average Status: " + average_mind * 100 + "%");
            if (average_mind < attackThreshold && !hunting && can_hunt)
            {
                int dice = Random.Range(1, 3);
                if (dice == 2) StartCoroutine(hunt());
            }
            if (average_mind < eventThreshold && !inEvent && can_perform_event)
            {
                int dice = Random.Range(1, 3);
                if (dice == 2) ghostevent();
            }
            yield return new WaitForSeconds(checkThreshold);
        }
    }

    IEnumerator HuntThreshold()
    {
        hunting = true;
        yield return new WaitForSeconds(huntDuration);
        hunting = false;
    }

    IEnumerator HuntCooldown()
    {
        can_hunt = false;
        yield return new WaitForSeconds(huntCooldown);
        can_hunt = true;
    }

    IEnumerator EventCooldown()
    {
        can_perform_event = false;
        yield return new WaitForSeconds(eventCooldown);
        can_perform_event = true;
    }

    public override IEnumerator hunt()
    {
       
        Debug.Log("Hunt starts right now >:)");
        GetComponent<Rigidbody>().isKinematic = false;
        collider.enabled = true;
        StartCoroutine(HuntThreshold());
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        float min_distance = Vector3.Distance(transform.position, players[0].transform.position);
        Vector3 closest = players[0].transform.position;
        for (int i = 1; i < players.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, players[i].transform.position);
            if (dist < min_distance)
            {
                min_distance = dist;
                closest = players[i].transform.position;
            }
        }

        RaycastHit hit;
        Ray ray = new Ray(transform.position, (closest - transform.position));
        if (Physics.Raycast(ray, out hit))
        {
            Player closest_player = hit.transform.gameObject.GetComponent<Player>();
            Debug.Log(hit.transform.name);
            if (closest_player)
            {
                Debug.DrawLine(transform.position, closest_player.transform.position, Color.green, 10f, false);
            }
        }

        foreach (GameObject door in doors)
        {
            Debug.DrawLine(transform.position, door.transform.position, Color.red, 2f, false);
        }

        while (hunting)
        {
            yield return new WaitForEndOfFrame();
        }
        while (hunting)
        {

            yield return new WaitForEndOfFrame();
        }
        Debug.Log("You're alive by now!");
        collider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(HuntCooldown());
    }

    public override void ghostevent()
    {
        Debug.Log("Boo");
        Light[] lights = GameObject.FindObjectsOfType<Light>();
        foreach (Light light in lights) light.enabled = false;
        StartCoroutine(EventCooldown());
    }

    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        collider.enabled = false;
        players = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(CheckPlayers());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            can_hunt = true;
            StartCoroutine(hunt());
        }
    }
}
