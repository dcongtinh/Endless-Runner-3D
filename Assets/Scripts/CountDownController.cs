using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDownController : MonoBehaviour
{
    public int countDownTime;
    public TextMeshProUGUI countDownDisplay;
    private GameObject player;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(CountDownToStart());
        player = GameObject.Find("Player");
    }

    IEnumerator CountDownToStart(){
        while (countDownTime > 0){
            countDownDisplay.text = countDownTime.ToString();
            yield return new WaitForSeconds(1f);
            --countDownTime;
        }
        countDownDisplay.text = "GO!";
        player.GetComponent<PlayerCollisionController>().enabled = true;
        GameObject.Find("BlurBg").SetActive(false);
        yield return new WaitForSeconds(1f);
        countDownDisplay.gameObject.SetActive(false);
    }
}
