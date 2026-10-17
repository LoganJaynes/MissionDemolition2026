using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    idle,
    playing,
    levelEnd,
    gameOver
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;
    [Header("Inscribed")]
    public Text uitLevel;
    public Text uitShots;
    public Text uitScore;
    public Vector3 castlePos;
    public GameObject[] castles;
    [Tooltip("Max shots for each level, easy to hard. Same order as castles.")]
    public int[] shotLimits = { 6, 5, 5, 4 };
    public GameObject gameOverPanel;
    public Text uitGameOverTitle;
    public Text uitGameOverScore;
    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public int shotsLeft;
    public int score;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";
    float lastShotTime;
    void Start()
    {
        S = this;
        levelMax = castles.Length;
        gameOverPanel.SetActive(false);
        ResetGame();
    }

    void ResetGame()
    {
        level = 0;
        shotsTaken = 0;
        score = 0;
        StartLevel();
    }

    void StartLevel()
    {
        if (castle != null)
        {
            Destroy(castle);
        }
        Projectile.DESTROY_PROJECTILES();
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        Goal.goalMet = false;

        shotsLeft = (level < shotLimits.Length) ? shotLimits[level] : 5;

        UpdateGUI();
        mode = GameMode.playing;
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots left: " + shotsLeft;
        uitScore.text = "Score: " + score;
    }

    void Update()
    {
        if (mode == GameMode.gameOver) return;
        UpdateGUI();

        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;
            score += 100 + 50 * shotsLeft;
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);
            Invoke("NextLevel", 2f);
            return;
        }

        // Out of shots: lose once the projectile settles, or after 8 seconds regardless
        if ((mode == GameMode.playing) && shotsLeft <= 0)
        {
            bool settled = !Projectile.ANY_AWAKE();
            bool timedOut = Time.time - lastShotTime > 8f;
            if (settled || timedOut)
            {
                mode = GameMode.levelEnd;
                Debug.Log("Out of shots, ending game");
                Invoke("LoseGame", 1.5f);
            }
        }
    }


    void NextLevel()
    {
        level++;
        if (level >= levelMax)
        {
            GameOver(true);
            return;
        }
        StartLevel();
    }

    void LoseGame()
    {
        GameOver(false);
    }

    void GameOver(bool won)
    {
        mode = GameMode.gameOver;
        Debug.Log("GameOver called, won = " + won);
        gameOverPanel.SetActive(true);   // show the panel FIRST
        if (uitGameOverTitle != null) uitGameOverTitle.text = won ? "You Win!" : "Game Over";
        if (uitGameOverScore != null) uitGameOverScore.text = "Final score: " + score + "\nShots taken: " + shotsTaken;
    }

    // Hook this to the Play Again button's OnClick
    public void PlayAgain()
    {
        CancelInvoke();
        gameOverPanel.SetActive(false);
        ResetGame();
    }

    static public bool CAN_SHOOT()
    {
        return S.mode == GameMode.playing && S.shotsLeft > 0;
    }

    static public void SHOT_FIRED()
    {
        S.shotsTaken++;
        S.shotsLeft--;
        S.lastShotTime = Time.time;
    }

    static public GameObject GET_CASTLE()
    {
        return S.castle;
    }
}