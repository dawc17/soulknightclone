using UnityEngine;
using Tenko;

public class PlayerAnimator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //References
    private Animator am;
    private PlayerLocomotionManager pm;
    private SpriteRenderer sr;
    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerLocomotionManager>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.moveDir.x != 0 || pm.moveDir.y != 0)
        {
            am.SetBool("Move", true);
            
            SpriteDirectionChecker();
        }
        else 
        {
            am.SetBool("Move", false);
        }
    }
    
    void SpriteDirectionChecker()
    {
        if (pm.LastHorizontalVector < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }
}
