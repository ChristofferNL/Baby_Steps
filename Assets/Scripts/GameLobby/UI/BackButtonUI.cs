using UnityEngine;

namespace LobbyRelaySample.UI
{
    /// <summary>
    /// For navigating the main menu.
    /// </summary>
    public class BackButtonUI : UIPanelBase
    {
        [SerializeField] Animator ellipseAnimator;
        public void ToJoinMenu()
        {
            Manager.UIChangeMenuState(GameState.JoinMenu);
        }

        public void ToMenu()
        {
            Manager.UIChangeMenuState(GameState.Menu);
            ellipseAnimator.Play("EllipseMoveBack");
        }
    }
}
