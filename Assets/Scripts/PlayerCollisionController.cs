using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CMF;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
// using socket.io;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class PlayerCollisionController : MonoBehaviour
{
    // endpoint to get current action, from Python
    public static string actionUri = "http://localhost:5000/pose";
    public static string imageUri = "http://localhost:5000/image";

    public static IPEndPoint socketUri = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI scoreTextOver;
    public TextMeshProUGUI highScoreTextOver;

    public SpawnManager spawnManager;
    public ParticleSystem coinCollectParticle;
    public ParticleSystem enemyHitParticle;
    public GameObject gameOverMenu;

    private bool isdead = false;
    private bool middleLaneTriggered = true;
    private float scores = 0;
    private float coins = 0;
    private string SavePath = "Assets/HighScores.json";
    private int numPerRanking = 5;

    // using namespace CMF
    private AdvancedWalkerController awc;
    private AudioControl audioControl;
    private string action = "idle";
    private string stand_crunch = "stand";
    private string lane = "middle";
    private bool httpError = true; // status of server is working or not

    // get pose task handler
    private IEnumerator getPoseRequest;
    private IEnumerator getImageRequest;


    public Image displayImage;

    [Serializable]
    public class PoseResponse
    {
        public string code;
        public string action;
        public string stand_crunch;
        public string lane;
    }


    private Socket sock;
    private static ManualResetEvent connectDone =
        new ManualResetEvent(false);

    static string decode_message(byte[] raw){
        string result = "";
        for(int i = 0; i<raw.Length && raw[i] != 1; ++i) {
            result += (char) raw[i];
        }
        return result;
    }
    // Start is called before the first frame update
    void Start()
    {
        awc = GetComponent<AdvancedWalkerController>();
        audioControl = GetComponent<AudioControl>();
        StartCoroutine("GetImage");
        // connect
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        sock.Blocking = false;
        sock.ReceiveTimeout = 1; // 100 ms for receiving
        // connect async
        sock.BeginConnect(socketUri, new AsyncCallback(ConnectCallback), sock);
        connectDone.WaitOne(2000);
        // now start corroutine
        StartCoroutine("GetPose");
        // cannot start corroutine in another thread, which prevent corroutine to work with unity main thread

    }

    private void ConnectCallback(IAsyncResult ar) {
        // after connecting done call to start corroutine
        // Retrieve the socket from the state object.
        sock = (Socket) ar.AsyncState;
        // Complete the connection phase
        sock.EndConnect(ar);
        connectDone.Set();
    }

    void FixedUpdate()
    {
        scoreText.text = scores.ToString() + " m";
        coinsText.text = coins.ToString() + " x";
    }
    string currentLane = "m";
    string nextLane = "m";
    bool keyLRPressed = false;

    // Update is called once per frame
    private void SwitchLane()
    {
        if (lane == "unknown") {
            keyLRPressed = false;
        }

        if (lane == "right")
        {
            if (currentLane != "r")
            {
                awc.ChangeAction("right");
                keyLRPressed = true;
                if (currentLane == "m" && transform.position.x >= 3)
                {
                    awc.ChangeAction("idle");
                    currentLane = "r";
                    middleLaneTriggered = false;
                    keyLRPressed = false;
                }
                else if (currentLane == "l" && middleLaneTriggered)
                {
                    awc.ChangeAction("idle");
                    currentLane = "m";
                    keyLRPressed = false;
                }
            }
        }

        if (lane == "left")
        {
            if (currentLane != "l")
            {
                awc.ChangeAction("left");
                keyLRPressed = true;
                if (currentLane == "m" && transform.position.x <= -3)
                {
                    awc.ChangeAction("idle");
                    currentLane = "l";
                    middleLaneTriggered = false;
                    keyLRPressed = false;
                }
                else if (currentLane == "r" && middleLaneTriggered)
                {
                    awc.ChangeAction("idle");
                    currentLane = "m";
                    keyLRPressed = false;
                }
            }
        }

        if (lane == "middle")
        {
            if (currentLane != "m")
            {
                if (currentLane == "l")
                {
                    awc.ChangeAction("right");
                    keyLRPressed = true;
                    if (middleLaneTriggered)
                    {
                        awc.ChangeAction("idle");
                        currentLane = "m";
                        keyLRPressed = false;
                    }
                }
                if (currentLane == "r")
                {
                   awc.ChangeAction("left");
                   keyLRPressed = true;
                    if (middleLaneTriggered)
                    {
                        awc.ChangeAction("idle");
                        currentLane = "m";
                        keyLRPressed = false;
                    }
                }
            }
        }
    }
    private void SwitchLane_Keyboard(){
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane != "r")
            {
                awc.ChangeAction("right");
                nextLane = "r";
                keyLRPressed = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane != "l")
            {
                awc.ChangeAction("left");
                nextLane = "l";
                keyLRPressed = true;
            }
        }

        if (keyLRPressed)
        {
            if (nextLane == "r")
            {
                if (currentLane == "l" && middleLaneTriggered)
                {
                    awc.ChangeAction("idle");
                    currentLane = "m";
                    keyLRPressed = false;
                }
                else if (currentLane == "m" && transform.position.x >= 3)
                {
                    awc.ChangeAction("idle");
                    currentLane = "r";
                    keyLRPressed = false;
                    middleLaneTriggered = false;
                }
            }
            else if (nextLane == "l")
            {
                if (currentLane == "r" && middleLaneTriggered)
                {
                    awc.ChangeAction("idle");
                    currentLane = "m";
                    keyLRPressed = false;
                }
                else if (currentLane == "m" && transform.position.x <= -3)
                {
                    awc.ChangeAction("idle");
                    currentLane = "l";
                    keyLRPressed = false;
                    middleLaneTriggered = false;
                }
            }
        }
    }

    void Update()
    {
        awc.Update_STT_Server(httpError);

        if (httpError == false)
        { // server is working !!!
            if (!isdead)
            {
                SwitchLane();
                if (!keyLRPressed)
                {
                    if (stand_crunch == "crunch") {
                        awc.ChangeAction("slide");
                        scores += 2;
                    }
                    else if (action == "walking")
                    {
                        awc.ChangeAction("walk");
                        scores += 1;
                    }
                    else if (action == "running")
                    {
                        awc.ChangeAction("run");
                        scores += 2;
                    }
                    else if (action == "idle" || action == "unknown" || action == "") {
                        awc.ChangeAction("idle");
                    }
                }
            }
        }
        else
        { // using keyboard controller
            if (!isdead)
            {
                scores += 1;
                if (Input.GetKeyDown(KeyCode.Space)) awc.ChangeAction("jump");
                else if (Input.GetKeyDown(KeyCode.DownArrow)) awc.ChangeAction("slide");
                else
                {
                    SwitchLane_Keyboard();
                    if (!keyLRPressed)
                    {
                        awc.ChangeAction("run");
                    }
                }
            }
        }
    }

    IEnumerator GetPose() {
        while (true) {

            // read data if available
            if (sock.Available > 0) {
                Debug.Log("Data available: " + sock.Available);
                // reset buffer
                var buffer = new byte[128];

                // receive data from socket
                var readCount = sock.Receive(buffer);

                if (readCount == 0) {
                    // stop the corroutine
                    yield return new WaitForSeconds(0.01f);
                }

                // decode it
                var message = decode_message(buffer);

                try {
                    // parse to get action from http response
                    var text = message;
                    var response = JsonUtility.FromJson<PoseResponse>(text);
                    this.action = response.action;
                    this.stand_crunch = response.stand_crunch;
                    this.lane = response.lane;
                    this.httpError = false;
                    Debug.Log("New action updated: " + action);
                }
                // if error
                catch (Exception e) {
                    this.httpError = true;
                    Debug.LogError("Error happened: " + e.ToString() + ". Message received:" + message);
                }
                // return to caller
                // Debug.Log(message);
                // yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator GetImage()
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                if (webRequest.isHttpError)
                {
                    httpError = true;
                    // Debug.Log("HTTP ERROR: " + webRequest.error);
                    continue;
                }

                httpError = false;

                try
                {
                    displayImage.enabled = true;
                    // parse to get action from http response
                    var base64Url = webRequest.downloadHandler.text;
                    byte[] imageBytes = Convert.FromBase64String(base64Url);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    Rect rect = new Rect(0.0f, 0.0f, tex.width, tex.height);
                    Vector2 pivot = new Vector2(0.5f, 0.5f);
                    float pixelsPerUnit = 128f;
                    Sprite sprite = Sprite.Create(tex, rect, pivot, pixelsPerUnit);
                    displayImage.overrideSprite = sprite;
                }
                // if error
                catch (Exception e)
                {
                    Debug.LogError("Error happened: " + e.ToString());
                }

                // whatever happens, wait for 0.05 second
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SpawnTrigger")
        {
            spawnManager.SpawnTriggerEntered();
        }

        if (other.tag == "coin")
        {
            Instantiate(coinCollectParticle, other.transform.position + new Vector3(0, 0.49f, 0), other.transform.rotation);
            Destroy(other.gameObject);
            audioControl.PlayCoinSound();
            ++coins;
            scores += 10;
        }

        if (other.tag == "MiddleLane"){
            middleLaneTriggered = true;
        }
    }

    private EntryTable GetSavedHighScores()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("HighScores.json does not exist.");
            return new EntryTable();
        }
        using (StreamReader stream = new StreamReader(SavePath))
        {
            string json = stream.ReadToEnd();
            return JsonUtility.FromJson<EntryTable>(json);
        }
    }

    private void SaveScores(EntryTable entryTable)
    {
        using (StreamWriter stream = new StreamWriter(SavePath))
        {
            string json = JsonUtility.ToJson(entryTable, true);
            stream.Write(json);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hurdle" && !isdead)
        {
            audioControl.PlayDeathSound();
            isdead = true;
            EntryTable entries = GetSavedHighScores();
            entries.highScores.Add((int)scores);
            entries.highScores.Sort((a, b) => b.CompareTo(a));
            if (entries.highScores.Count > numPerRanking)
            {
                entries.highScores.RemoveRange(numPerRanking, entries.highScores.Count - numPerRanking);
            }

            SaveScores(entries);
            scoreTextOver.text = scores.ToString();
            highScoreTextOver.text = Mathf.Max(scores, entries.highScores[0]).ToString();
            awc.ChangeAction("death");
        }
    }

    public void RestartGame()
    {
        // Application.LoadLevel(Application.loadedLevelName);
        gameOverMenu.SetActive(true);
    }

    void OnApplicationQuit()
    {
        if (sock != null) {
            sock.Close();
            sock = null;
        }
    }
}
