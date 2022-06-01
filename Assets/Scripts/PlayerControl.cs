using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rb;
    private Renderer _renderer;

    private Transform ball;
    private bool moveTheBall;

    private Vector3 touchPosition;

    private Vector3 startMousePos, startBallPos;
    [Range(0, 1f)] public float maxSpeed;
    [Range(0, 1f)] public float camSpeed;
    [Range(0, 100f)] public float pathSpeed;
    private float defaultSpeed;

    public Transform path;

    private Camera mainCam;
    private float velocity, camVelocity;

    private Collider _collider;

    public ParticleSystem CollisionEffect;
    public ParticleSystem landing;
    public ParticleSystem Air;
    public ParticleSystem trail;

    // Start is called before the first frame update
    void Start()
    {


        ball = transform;
        mainCam = Camera.main;

        rb = GetComponent<Rigidbody>();
        defaultSpeed = pathSpeed;
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
        PlayerPrefs.SetInt("score", 0);

        if (!PlayerPrefs.HasKey("hiScore"))
        {
            PlayerPrefs.SetInt("hiScore", 0);
        }
        MenuManager.MenuManagerInstance.menuElement[4].GetComponent<Text>().text = "High score: " + PlayerPrefs.GetInt("hiScore");
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && MenuManager.MenuManagerInstance.GameState)
        {

            moveTheBall = true;

            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            Plane newPlane = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);



            if (newPlane.Raycast(ray, out var distance))
            {
                ;
                startBallPos = ball.position;
            }



            Vector2 dragDistanceUnscaled = touch.deltaPosition;
            Vector2 dragDistance = new Vector2(dragDistanceUnscaled.x / Screen.width,
            dragDistanceUnscaled.y / Screen.height);
            Debug.Log(dragDistance.x * 100);

            float desiredPosition = dragDistance.x * 100 + startBallPos.x;

            desiredPosition = Mathf.Clamp(desiredPosition, -2.7f, 2.7f);

            ball.position = new Vector3(Mathf.SmoothDamp(ball.position.x, desiredPosition, ref velocity, maxSpeed),
            ball.position.y, ball.position.z);

        }



        if (MenuManager.MenuManagerInstance.GameState)
        {
            var pathNewPos = path.position;
            path.position = new Vector3(pathNewPos.x, pathNewPos.y, Mathf.MoveTowards(pathNewPos.z, -1000f, pathSpeed * Time.deltaTime));
        }
    }
    private void LateUpdate()
    {
        var CameraNewPos = mainCam.transform.position;

        mainCam.transform.position = new Vector3(Mathf.SmoothDamp(CameraNewPos.x, ball.transform.position.x, ref camVelocity, camSpeed),
        CameraNewPos.y, CameraNewPos.z);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "obstacles")
        {


            var newParticle1 = Instantiate(CollisionEffect, transform.position, Quaternion.identity);
            newParticle1.GetComponent<Renderer>().material = _renderer.material;
            gameObject.SetActive(false);
            MenuManager.MenuManagerInstance.menuElement[2].GetComponent<Text>().text = "You Lose";

            GameObject.FindWithTag("air").GetComponent<ParticleSystem>().Stop();
            MenuManager.MenuManagerInstance.GameState = false;
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<RectTransform>().transform.Translate(new Vector3(0, 1, 0) * -800);
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<Text>().text = "Score: " + PlayerPrefs.GetInt("score").ToString();

            MenuManager.MenuManagerInstance.menuElement[0].SetActive(true);
            MenuManager.MenuManagerInstance.menuElement[3].SetActive(true);


            if (PlayerPrefs.GetInt("hiScore") < PlayerPrefs.GetInt("score"))
            {
                PlayerPrefs.SetInt("hiScore", PlayerPrefs.GetInt("score"));
                MenuManager.MenuManagerInstance.menuElement[4].GetComponent<Text>().text = "High score: " + PlayerPrefs.GetInt("hiScore");
                PlayerPrefs.Save();

            }

            MenuManager.MenuManagerInstance.menuElement[3].GetComponent<Text>().text = "Play Again";

        }
        if (other.tag == "colorBall")
        {

            other.gameObject.SetActive(false);
            var newParticle = Instantiate(CollisionEffect, transform.position, Quaternion.identity);
            _renderer.material = newParticle.GetComponent<Renderer>().material = trail.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;

            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 10);
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<Text>().text = PlayerPrefs.GetInt("score").ToString();



        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "path")
        {
            rb.isKinematic = _collider.isTrigger = false;
            rb.velocity = new Vector3(0f, 7f, 0f);
            pathSpeed = pathSpeed * 1.2f;

            var AirMain = Air.main;
            AirMain.simulationSpeed = 10;


        }

    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.collider.tag == "path")
        {
            rb.isKinematic = _collider.isTrigger = true;

            var AirMain = Air.main;
            AirMain.simulationSpeed = 6;

            landing.transform.position = other.contacts[0].point;
            landing.Play();


        }

        if (other.collider.tag == "finish")
        {


            pathSpeed = 0;
            MenuManager.MenuManagerInstance.menuElement[2].GetComponent<Text>().text = "You Win!";

            GameObject.FindWithTag("air").GetComponent<ParticleSystem>().Stop();
            MenuManager.MenuManagerInstance.GameState = false;
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<RectTransform>().transform.Translate(new Vector3(0, 1, 0) * -800);
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<Text>().text = "Score: " + PlayerPrefs.GetInt("score").ToString();

            MenuManager.MenuManagerInstance.menuElement[0].SetActive(true);
            MenuManager.MenuManagerInstance.menuElement[3].SetActive(true);
            MenuManager.MenuManagerInstance.menuElement[3].GetComponent<Text>().text = "Play Again";

            trail.Stop();

            if (PlayerPrefs.GetInt("hiScore") < PlayerPrefs.GetInt("score"))
            {
                PlayerPrefs.SetInt("hiScore", PlayerPrefs.GetInt("score"));
                MenuManager.MenuManagerInstance.menuElement[4].GetComponent<Text>().text = "High score: " + PlayerPrefs.GetInt("hiScore");
                PlayerPrefs.Save();

            }

            landing.transform.position = other.contacts[0].point;
            landing.Play();


        }

    }


}
