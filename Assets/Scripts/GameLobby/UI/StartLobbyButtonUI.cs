using LobbyRelaySample.UI;
using System;
using System.Collections;
using UnityEngine;

namespace LobbyRelaySample
{
    /// <summary>
    /// Main menu start button.
    /// </summary>
    public class StartLobbyButtonUI : UIPanelBase
    {
        [SerializeField] Animator ellipseAnimator;
        public void ToJoinMenu()
        {
            ellipseAnimator.enabled = true;
            ellipseAnimator.Play("EllipseMove");
            StartCoroutine(WaitBeforeStateChange());
        }

        IEnumerator WaitBeforeStateChange()
        {
            yield return new WaitForSeconds(0.3f);
			Manager.UIChangeMenuState(GameState.JoinMenu);
		}
    }
}
