using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public Player owningPlayer;

    public void ReportWeaponAction(string hitType)
    {
        var eventData = new WeaponEvent(
            weaponName: GetType().Name,
            isFreezed: false,
            isOpponentTaunted: owningPlayer.opponent.isTaunted,
            eventType: hitType
        );

        GameReport.Instance.PostDataToFirebase("weaponEvent", eventData);
    }
}
