using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Windows.Speech;
using System.Linq;

public class Player : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField, Range(0f, 10f)] private float speed;
    [SerializeField, Range(0f, 10f)] private float run_speed;
    [SerializeField, Range(0f, 10f)] private float msens;
    [SerializeField, Range(0f, 10f)] private float targetDistance;
    [SerializeField, Range(0f, 10f)] private float translateSpeed;
    [SerializeField, Range(0f, 2f)] private float crouchHeight;
    [SerializeField] private new Light light;
    [SerializeField] private AudioClip  footsteps;

    //---   PUBLIC VARIABLES    ---//

    public Transform targetPoint;
    public Transform headPosition;
    public float mind = 1.0f;

    //---   PRIVATE VARIABLES   ---//
    private float normalHeight;
    private const float seconds = 0.5f;
    private const float run_seconds = 0.4f;
    private AudioSource audioSource;
    private Vector3     rotation;
    private Animator    animator;
    private bool        running = false;
    private bool        crouching = false;
    private Camera      mainCamera;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    private float mentalHealth = 1.0f;
    private AudioClip audioClip;
    public float getMentalHealth() { return mentalHealth; }

    IEnumerator Footsteps()
    {
        audioSource.clip = footsteps;
        audioSource.Play();
        yield return new WaitForSeconds((running) ? run_seconds : seconds);
        audioSource.Stop();
    }

    IEnumerator MindChecker()
    {
        while (true)
        {
            Debug.Log("Current statis: " + mind * 100 + "%");
            yield return new WaitForSeconds(1);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rotation = new Vector3(0, 0, 0);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        normalHeight = mainCamera.transform.localPosition.y;
        keywords.Add("give us a sign", () =>
        {
            light.color = new Color(0.75f, 0.25f, 0.0f);
        });
        keywords.Add("fuck you leatherman", () =>
        {
            light.color = new Color(1.0f, 1.0f, 1.0f);
        });
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

        StartCoroutine(MindChecker());
    }
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    IEnumerator TranslateCameraPosition(float yPos)
    {
        int direction = (int) (mainCamera.transform.position.y - yPos) / (int) Mathf.Abs(mainCamera.transform.position.y - yPos);
        while ((direction < 0 && mainCamera.transform.position.y < yPos)
            || (direction > 0 && mainCamera.transform.position.y > yPos))
        {
            mainCamera.transform.position = new Vector3(
                mainCamera.transform.position.x,
                direction * translateSpeed * Time.deltaTime,
                mainCamera.transform.position.z
                );
            yield return new WaitForEndOfFrame();
        }
    }

    void Update()
    { 
        float new_x = Input.GetAxis("Mouse X") * msens;
        float new_y = Input.GetAxis("Mouse Y") * msens;

        rotation.x -= new_y;
        rotation.y += new_x;
        rotation.x = Mathf.Clamp(rotation.x, -89f, 89f);

        transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        Camera.main.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            running = true;
            crouching = false;
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, normalHeight, mainCamera.transform.localPosition.z);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) running = false;
        if (Input.GetKeyDown(KeyCode.T)) light.enabled = !light.enabled;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouching = !crouching;
            if (crouching) mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, crouchHeight, mainCamera.transform.localPosition.z);
            else mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, normalHeight, mainCamera.transform.localPosition.z);
        }

        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * ((running) ? run_speed : speed) * Time.deltaTime);
        animator.SetFloat("speed", Input.GetAxis("Vertical") * ((running) ? run_speed : speed));
        animator.SetFloat("side_speed", Input.GetAxis("Horizontal") * ((running) ? run_speed : speed));

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (!audioSource.isPlaying)
                StartCoroutine(Footsteps());
        }

        Ray newPositionRay = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector2( Screen.width / 2, Screen.height / 2));
        Vector3 newPosition = newPositionRay.origin + newPositionRay.direction * targetDistance;
        targetPoint.position = newPosition;
    }
}
