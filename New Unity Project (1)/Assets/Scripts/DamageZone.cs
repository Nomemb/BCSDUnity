using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    // OnTriggerEnter2D : 해당 영역에 들어갈 때만 데미지
    // OnTriggerStay2D : 영역 안에 머무르는 동안에도 매 프레임마다 데미지
    private void OnTriggerStay2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeHealth(-1);
        }
    }
}
