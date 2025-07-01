using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] Character ConnectedCharacter;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (ConnectedCharacter.rb.velocity.y == 0 && other.tag == "Ground")
        {
            ConnectedCharacter.grounded = true;
            ConnectedCharacter.fastFalling = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ConnectedCharacter.grounded = false;
    }
}
