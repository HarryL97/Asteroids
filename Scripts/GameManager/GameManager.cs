using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum gameState {
    Start, Playing, GameOver, HighScore
}

//KNOWNISSUE -sometimes the round doesn't end
//KNOWNISSUE - UI iteams appear off screen
//KNOWNISSUE - Full screen mode creats weirs colour boarders
public class GameManager : Singleton<GameManager>
{

    [SerializeField]
    private UnityEngine.UI.Text currentScoreTxt;
    [SerializeField]
    private UnityEngine.UI.Text highScoreTxt;
    [SerializeField]
    private UnityEngine.UI.Text menu;
    [SerializeField]
    private UnityEngine.UI.RawImage[] livesImg;


    // TODO - camelCase these vars;
    [SerializeField]
    private Asteroid[] Asteroid;
    [SerializeField]
    private InvsBoundary[] OppositeBoundary;
    [SerializeField]
    private Player player;
    [SerializeField]
    private UFO[] ufo;
    [SerializeField]
    private GameObject[] powerUps;

    // TODO - camelCase these vars;
    private GameObject LifeImgHolder;
    private gameState currentState = gameState.Start;
    private int roundNo = 0;
    private int currentScore = 0;
    private int highScore = 0;
    private int lives = 3;
    private int livesScore =0;
    private int ufoCounter = 0;
    private int powerCounter =0;

    // TODO - Better implementation of this spagetti tracker
    public int UfoCounter {
        get {
            return ufoCounter;
        }
        set {
            ufoCounter = value;
        }
    }

    public int PowerCounter {
        get {
            return powerCounter;
        }
        set {
            powerCounter = value;
        }
    }

    void Start()
    {
        menu.gameObject.SetActive(false);
        if(PlayerPrefs.HasKey("High Score")) {
            highScore = PlayerPrefs.GetInt("High Score");
            highScoreTxt.text = highScore.ToString();
        }
        else {
            highScoreTxt.text = "00";
        }

        currentScoreTxt.text = "00";
        ShowMenu();
    }

    void Update()
    {
        if((currentState == gameState.Start || currentState ==  gameState.GameOver) && Input.GetKeyDown(KeyCode.Space)) {
            StartGame();
        }
    }
    
    // TODO - better implementation of these simmilar methods
    public Vector3 GetBoundaryPosition(int BoundaryNumber) {
        return OppositeBoundary[BoundaryNumber].transform.position;
    }
    
    public float RandomXLocation() { 
        return Random.Range(OppositeBoundary[2].transform.position.x,OppositeBoundary[3].transform.position.x); 
    }

    public float RandomYLocation() {
        return Random.Range(OppositeBoundary[1].transform.position.y,OppositeBoundary[0].transform.position.y);
    }

    // TODO - Break this down into smaller methods
    public void AddScore(int value) {
        currentScore += value;
        currentScoreTxt.text = currentScore.ToString();
        if((currentScore - livesScore) > 10000){
            lives += 1;
            LifeImgHolder.SetActive(true); //dirty 
        }
        IsRoundOver();
    }
    // TODO - Break this down into smaller methods
    public void SubLives() {
        lives -= 1;
        livesImg[lives].gameObject.SetActive(false);
        if(lives <= 0) {
            currentState = gameState.GameOver;
            UpdateHighScore();
            ShowMenu();
        }
        else {
            Respawn();
        }

        LifeImgHolder = livesImg[lives].gameObject;
    }

    private void CreateAsteroid()
    {
        Asteroid newAsteroid = Instantiate(Asteroid[Random.Range(0,Asteroid.Length)]);
        GameObject FindPlayer = GameObject.FindWithTag("Player");
        Vector3 candidatePosition = FindPlayer.transform.position;

        while(Vector3.Distance(candidatePosition,FindPlayer.transform.position) < 3 ) {

            candidatePosition = new Vector3(RandomXLocation(),RandomXLocation(),0);
        }
        
        newAsteroid.transform.position = candidatePosition;
        newAsteroid.Move();
    }

    private void ShowMenu() {
        switch(currentState)
        {
            case gameState.Start :
                menu.text = "1 Coin 1 Play\nPress Space";
                // TODO - Add audio
                break;
            case gameState.GameOver :
                menu.text = "Game Over\nPlay Again?";
                break;
            case gameState.HighScore :
                menu.text = "HighScore!\nEnter Initials";
                break;
        }

        menu.gameObject.SetActive(true);
    }

    private void StartGame() {
        currentState = gameState.Playing;
        menu.gameObject.SetActive(false);
        currentScore = 0;
        roundNo = 0;
        currentScoreTxt.text = "00";   
        DeleteAll();
        ResetLives();

        Instantiate(player);
        StartCoroutine(UFOSpawner());
        StartCoroutine(PowerUpSpawner());
        NextRound();
    }

    private void ResetLives() {
        lives = 3;
        livesImg[0].gameObject.SetActive(true);
        livesImg[1].gameObject.SetActive(true);
        livesImg[2].gameObject.SetActive(true);
        //TODO - Add more RawImg lives to the UI canvas
        //livesImg[3].gameObject.SetActive(false); 
        //livesImg[4].gameObject.SetActive(false);
    }

    // TODO turn Respawn into general sheild/invincibility method 
    private void Respawn() {
        Player respawnPlayer = Instantiate(player);
        respawnPlayer.TurnOffCollider();
        StartCoroutine(InvincibilityTimer(respawnPlayer));
    }

    IEnumerator InvincibilityTimer(Player player) {
        yield return new WaitForSeconds(2);
        player.TurnOnCollider();
    }

    private void NextRound() {
        for(int i = 0; i < 1+roundNo*2; i++) {
            CreateAsteroid();
        }
    }

    private void IsRoundOver() {
        //TODO - For somereason the length is 1 when objects are null
        if(GameObject.FindGameObjectsWithTag("Asteroid").Length <= 1) {
            print("roundOver");
            roundNo += 1;
            NextRound();
        }
    }

    private void UpdateHighScore() {

        highScore = currentScore;
        highScoreTxt.text = highScore.ToString();
        PlayerPrefs.SetInt("High Score", highScore);
    }

    private void DeleteAll(){
        // TODO - Better implemenation, Make list to track objects
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("Asteroid")) {
            Destroy(o);
        }
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("UFO")) {
            Destroy(o);
        }
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("Player")) {
            Destroy(o);
        }

        StopAllCoroutines();
    }
    // TODO - Manage the UFO and Power up methods better
    IEnumerator UFOSpawner() {
        
        if(Random.Range(0,6) > 1 && ufoCounter == 0){
            UFOController();
            ufoCounter = 1;
        }
        yield return new WaitForSeconds(5);
        StartCoroutine(UFOSpawner());
    }

    private void UFOController() {
        UFO newUFO = Instantiate(ufo[Random.Range(0,2)]);
        int selectXBoundary = Random.Range(2,4);
        newUFO.transform.position = new Vector3(OppositeBoundary[selectXBoundary].transform.position.x,RandomYLocation(),0);
        
        if(selectXBoundary == 2) {
            newUFO.Move(-(Mathf.PI/4),(Mathf.PI/4));
        }
        else if(selectXBoundary ==3) {
            newUFO.Move((3*Mathf.PI/4),(5*Mathf.PI/4));
        }
    }

    IEnumerator PowerUpSpawner() {
        
        if(Random.Range(0,6) > 3 && powerCounter == 0){
            PowerupController();
            powerCounter = 1;
        }
        yield return new WaitForSeconds(5);
        StartCoroutine(PowerUpSpawner());
    }
    // TODO - Add lightning and star power up sprites 
    private void PowerupController() {
        GameObject PowerUp = Instantiate(powerUps[Random.Range(0,0)]);
        PowerUp.transform.position = new Vector3(RandomXLocation(),RandomYLocation(),0);
        StartCoroutine(PowerUpTimer(PowerUp));
    }

    IEnumerator PowerUpTimer(GameObject powerUp) {
        yield return new WaitForSeconds(10);
        Destroy(powerUp);
        powerCounter = 0;
    }

}
