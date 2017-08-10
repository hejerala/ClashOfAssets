using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour {

    public float speed = 20.0f;

    public void Init(BaseObject target, int damage) {
        transform.LookAt(target.transform);
        Tweener moveTween = transform.DOMove(target.transform.position, speed);
        //Sequence moveTween = transform.DOJump(target.transform.position, 5.0f, 1, speed);
        //we can easily make a tween speed based (instead of time based) by using this method
        //Now regardless of the distance, the arrow will move with the same speed
        moveTween.SetSpeedBased(true);
        //We give it a linear ease (means the object has the same speed throughout the tween)
        moveTween.SetEase(Ease.Linear);
        //We declare the callback of the tween INSIDE the init method
        //This system can be used for any callback in c#
        //We are basically creating a method inside a method
        moveTween.OnComplete( () => {
            //When the tween has completed we do damage to the building
            if (target != null)
                target.OnHit(damage);
            //Destroy(gameObject);
            PoolManager.instance.Despawn(gameObject);
        } );
    }

}
