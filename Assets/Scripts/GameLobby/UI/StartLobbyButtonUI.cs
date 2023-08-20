using LobbyRelaySample.UI;
using System;
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
            Manager.UIChangeMenuState(GameState.JoinMenu);
        }
    }
}
