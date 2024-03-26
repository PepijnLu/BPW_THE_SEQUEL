using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public int buttonInt;
    // Start is called before the first frame update
    public void WhenClicked()
    {
        switch(buttonInt)
        {
            //ATTACK
            case 1:
                BattleManager.instance.attackMade = true;
                gameObject.SetActive(false);
                break;
            case 2:
                if (BattleManager.instance.parryable)
                {
                    BattleManager.instance.parried = true;
                }
                BattleManager.instance.triedToParry = true;
                Debug.Log("tried to parry at: " + BattleManager.instance.distanceBetween);
                gameObject.SetActive(false);
                break;
        }
    }

}
