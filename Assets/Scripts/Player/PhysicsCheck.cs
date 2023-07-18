using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    public float checkRadius;

    Vector2 bottomOffset = new Vector2(0,0);
    Vector2 rightOffset = new Vector2(0.4f, 0.5f);
    Vector2 leftOffset = new Vector2(-0.4f, 0.5f);
    public bool IsGround()
    {
        //Check ground
        var check = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRadius);
        return check != null && (check.CompareTag("Ground") || check.CompareTag("Headtile"));
    }

    public bool IsBlockFacing(float dir)
    {

        Vector2 left = new Vector2(-0.4f, 1f);
        Vector2 right = new Vector2(0.4f, 1f);

        if (dir > 0) return CheckBlockAtDirection(right);
        if (dir < 0) return CheckBlockAtDirection(left);
        return false;
    }

    public bool CheckBlockAtDirection(Vector2 direction)
    {
        Vector2 size = new Vector2(0.2f, 1.7f);
        var check = Physics2D.OverlapCapsule((Vector2)transform.position + direction, size, CapsuleDirection2D.Vertical, 0);
        return check != null && (check.CompareTag("Ground") || check.CompareTag("Headtile"));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
        Gizmos.color = Color.yellow;
        // left yellow
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.color = Color.blue;
        // right blue
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);

    }
}
