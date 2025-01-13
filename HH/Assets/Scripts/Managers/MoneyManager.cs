using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static float money = 0;
    public static float moneyMultiplier = 1;

    [Tooltip("This will be used to calculate how many coin gameobjects will be generated")]
    public float moneyPerCoin;


    public float coinGenerationCooldown;
    public GameObject coinGO;
    public Transform coinSpawnPos;
    public List<GameObject> droppedCoins;

    public AudioSource coinFall;

    public void AddMoney(float moneyToAdd)
    {
        money += moneyToAdd * moneyMultiplier;

        StartCoroutine(DropCoinsInJar((Mathf.RoundToInt(money / moneyPerCoin))));
    }
    [ContextMenu("DevToolAddMoney")]
    private void DevToolAddMoney()
    {
        money += 20;
        StartCoroutine(DropCoinsInJar((Mathf.RoundToInt(money / moneyPerCoin))));
    }
    [ContextMenu("DevtoolRemoveMoney")]
    private void DevToolRemoveMoney()
    {
        money -= 10;
        StartCoroutine(DropCoinsInJar((Mathf.RoundToInt(money / moneyPerCoin))));
    }

    public IEnumerator DropCoinsInJar(int coinsInJar)
    {
        int coinsToAdd = coinsInJar - droppedCoins.Count;

        if (coinsToAdd > 0)
        {
            for(int i = 0; i < coinsToAdd; i++)
            {
                droppedCoins.Add(Instantiate(coinGO, coinSpawnPos.position, Quaternion.identity, coinSpawnPos));
                coinFall.Play();
                yield return new WaitForSeconds(coinGenerationCooldown);
            }
        }

        //Coins to add is negative here, so its more like coinsToRemove
        else if(coinsToAdd < 0)
        {
            coinsToAdd = Mathf.Abs(coinsToAdd);

            for (int i = 0; i < coinsToAdd; i++)
            {
                int random = Random.Range(0, droppedCoins.Count);
                if (droppedCoins[i] != null) ;
                Destroy(droppedCoins[random]);
                droppedCoins.RemoveAt(random);

                yield return new WaitForSeconds(coinGenerationCooldown);
            }
        }
        yield return null;
    }
}
