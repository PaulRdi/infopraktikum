using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectiblox.View
{
    public class PickTileInteraction : StateMachineBehaviour
    {
        CursorController cursor;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            cursor = animator.GetComponentInChildren<CursorController>(true);
            cursor.gameObject.SetActive(true);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            cursor.UpdateCursorPosition();
            if (Input.GetMouseButtonDown(0))
            {
                cursor.Click();
                animator.SetBool("PickTile", false);
            }

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}