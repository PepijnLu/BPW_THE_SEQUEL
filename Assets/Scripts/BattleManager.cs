using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public Transform playerCardSlot, enemyCardSlot;
    public bool battle;
    public List<GameObject> battleOpponents;
    public List<Stats> needToFinish;
    public Image playerSword, enemySword;
    GameObject[] cards;
    public bool attackMade, parryable, parried, triedToParry;
    bool playerAttack, enemyAttack;
    public GameObject playerAttackScreen, attackButton, parryButton;
    public TextMeshProUGUI attackText;
    public float distanceBetween;
    public bool takingDamage;

    void Awake()
    {
        instance = this;
        GameObject[] deactivates = GameObject.FindGameObjectsWithTag("Deactivate");
        foreach (GameObject deactivate in deactivates)
        {
            deactivate.SetActive(false);
        }
        battleOpponents = new List<GameObject>(){};
        needToFinish = new List<Stats>(){};
    }

    void Start()
    {
        
    }

    public void StartTakeDamageRoutine(int damage, GameObject enemy)
    {
        StartCoroutine(TakeDamage(GameManager.instance.playerStats, damage, enemy.GetComponent<Stats>()));
    }

    public void Battle(GameObject player, GameObject enemy, bool resetAudio, bool playerIniate)
    {
        if (resetAudio)
        {
            StartCoroutine(AudioManager.instance.AudioFadeIn(AudioManager.instance.audioSources["battleTheme"], 0.2f));
            AudioManager.instance.PlaySound(AudioManager.instance.audioSources["battleTheme"]);
            StartCoroutine(AudioManager.instance.AudioFadeOut(AudioManager.instance.audioSources["gameTheme"], 0.5f));
        }
        
        Stats enemyInListStats = enemy.GetComponent<Stats>();
        enemyInListStats.inBattle = true;
        GameManager.instance.stopped = true;

        if (enemyInListStats.inList == false)
        {
            battleOpponents.Add(enemy);
            enemyInListStats.inList = true;
            if (playerIniate)
            {
                enemyInListStats.attackingFirst = false;
            }
            else
            {
                enemyInListStats.attackingFirst = true;
            }
        }
        if ( (battleOpponents.Count > 0) && (battle == false) )
        {
            battle = true;

            foreach(GameObject card in ProcGen.instance.cards)
            {
                card.SetActive(false);
            }

            Stats playerStats = player.GetComponent<Stats>();
            Stats enemyStats = battleOpponents[0].GetComponent<Stats>();

            playerStats.card.SetActive(true);
            enemyStats.card.SetActive(true);

            ///UpdateUI(playerStats, enemyStats, playerDamage, enemyDamage);
            if (enemyStats.attackingFirst)
            {
                StartCoroutine(AttackInBattle(enemyStats, playerStats));
            }
            else
            {
                StartCoroutine(AttackInBattle(playerStats, enemyStats));
            }
            //StartCoroutine(DisableCards(playerStats, enemyStats, resetAudio, playerIniate));
        }
    }

    public IEnumerator TakeDamage(Stats defender, int damage, Stats attacker)
    {
        bool inQueue = false;
        while (takingDamage)
        {
            inQueue = true;
            yield return null;
        }
        takingDamage = true;
        Debug.Log("Take Damage pt 1");
        GameManager.instance.stopped = true;
        defender.card.SetActive(true);
        yield return new WaitForSeconds(1f);
        Debug.Log("Take Damage pt 2");
        defender.health -= damage;
        defender.hpText.text = defender.health.ToString();

        if (defender.health <= 0)
        {
            playerAttackScreen.SetActive(false);
            defender.health = 0;
            defender.hpText.text = defender.health.ToString();
            yield return new WaitForSeconds(1);
            if (defender.gameObject.tag == "Player")
            {
                SceneManager.LoadScene("GameOver");
            }
            defender.gameObject.SetActive(false);
            battleOpponents.Remove(defender.gameObject);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1);
        defender.card.SetActive(false);
        GameManager.instance.stopped = false;
        StartCoroutine(Movement.instance.EndMove(attacker));
        if (!inQueue)
        {
            foreach(Stats stats in needToFinish)
            {
                Movement.instance.EndMove(stats);
            }
            needToFinish.Clear();
        }
        takingDamage = false;
    }

    public IEnumerator AttackInBattle(Stats attacker, Stats defender)
    {
        playerAttackScreen.SetActive(true);
        Debug.Log("Attack: " + attacker + "Defemd: " + defender);
        parryable = false;
        if (attacker.gameObject.tag == "Player")
        {
            attackButton.SetActive(true);
            while (!attackMade)
            {
                yield return null;
                Debug.Log("while");
            }
            attackMade = false;
        }
        else
        {
            parryButton.SetActive(true);
        }
        int damage = attacker.damage;
        int missOrCrit = Random.Range(1, 11);
        attacker.sword.transform.position = attacker.cardSlot.transform.position;
        attacker.sword.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Vector2 startPos = attacker.sword.transform.position;
        while (elapsedTime < 1f)
        {
            float t = elapsedTime / 1f;
            attacker.sword.transform.position = Vector2.Lerp(startPos, defender.cardSlot.transform.position, t);
            distanceBetween = (defender.cardSlot.transform.position - attacker.sword.transform.position).magnitude;
            Debug.Log("distance between = " + distanceBetween);
            if (distanceBetween < 30f)
            {
                parryable = true;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        parryable = false;
        parryButton.SetActive(false);
        if (parried)
        {
            damage = 0;
            attackText.gameObject.SetActive(true);
            attackText.text = "Parried!";
            yield return new WaitForSeconds(1.5f);
            attackText.gameObject.SetActive(false);
        }
        else if (triedToParry)
        {
            damage *= 2;
            attackText.gameObject.SetActive(true);
            attackText.text = "Parry Failed!";
            yield return new WaitForSeconds(1.5f);
            attackText.gameObject.SetActive(false);
        }
        triedToParry = false;
        attacker.sword.gameObject.SetActive(false);
        if ((missOrCrit == 1) && (!parried))
        {
            attackText.text = "Miss";
            attackText.gameObject.SetActive(true);
            damage = 0; 
            yield return new WaitForSeconds(1);
            attackText.gameObject.SetActive(false);
        }
        else if (missOrCrit == 10)
        {
            attackText.text = "Critical Hit!";
            attackText.gameObject.SetActive(true);
            damage *= 2;
            yield return new WaitForSeconds(2);
            attackText.gameObject.SetActive(false);
        }
        parried = false;
        defender.health -= damage;
        defender.hpText.text = defender.health.ToString();
        if (defender.health <= 0)
        {
            playerAttackScreen.SetActive(false);
            defender.health = 0;
            defender.hpText.text = defender.health.ToString();
            StartCoroutine(AudioManager.instance.AudioFadeOut(AudioManager.instance.audioSources["battleTheme"], 1.5f));
            yield return new WaitForSeconds(1);
            if (defender.gameObject.tag == "Player")
            {
                SceneManager.LoadScene("GameOver");
            }
            defender.card.SetActive(false);
            defender.gameObject.SetActive(false);
            battleOpponents.Remove(defender.gameObject);
            battle = false;
            yield return new WaitForSeconds(0.2f);
            if(battleOpponents.Count > 0)
            {
                Battle(attacker.gameObject, battleOpponents[0], false, false);
            }
            else
            {
                GameManager.instance.stopped = false;
                // enemyStats.inBattle = false;
                // enemyStats.inList = false;
                StartCoroutine(AudioManager.instance.AudioFadeOut(AudioManager.instance.audioSources["battleTheme"], 0.5f));
                StartCoroutine(AudioManager.instance.AudioFadeIn(AudioManager.instance.audioSources["gameTheme"], 0.5f));
                attacker.card.SetActive(false);
            }

           StartCoroutine(Movement.instance.EndMove(attacker));
        }
        else if (defender.gameObject.tag == "Enemy")
        {
            StartCoroutine(AttackInBattle(defender, attacker));
        }
        else if (attacker.gameObject.tag == "Enemy")
        {
            StartCoroutine(AttackInBattle(defender, attacker));
        }
    }
}
