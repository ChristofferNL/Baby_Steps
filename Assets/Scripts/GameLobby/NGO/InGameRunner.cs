using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace LobbyRelaySample.ngo
{
    /// <summary>
    /// Once the NetworkManager has been spawned, we need something to manage the game state and setup other in-game objects
    /// that is itself a networked object, to track things like network connect events.
    /// </summary>
    public class InGameRunner : NetworkBehaviour
    {
        private NetworkedDataStore m_dataStore = default;

        public Action onGameBeginning;
        Action m_onConnectionVerified, m_onGameEnd;
        private int m_expectedPlayerCount; // Used by the host, but we can't call the RPC until the network connection completes.
        private float m_timeout = 10;
        private bool m_hasConnected = false;
        private PlayerData m_localUserData; // This has an ID that's not necessarily the OwnerClientId, since all clients will see all spawned objects regardless of ownership.

        public static InGameRunner Instance
        {
            get
            {
                if (s_Instance!) return s_Instance;
                return s_Instance = FindObjectOfType<InGameRunner>();
            }
        }

        static InGameRunner s_Instance;

        public void Initialize(Action onConnectionVerified, int expectedPlayerCount, Action onGameBegin,
            Action onGameEnd,
            LocalPlayer localUser)
        {
            m_onConnectionVerified = onConnectionVerified;
            m_expectedPlayerCount = expectedPlayerCount;
            onGameBeginning = onGameBegin;
            m_onGameEnd = onGameEnd;
            m_localUserData = new PlayerData(localUser.DisplayName.Value, 0);
        }

        public override void OnNetworkSpawn()
        {
            if (IsHost)
                FinishInitialize();
            m_localUserData = new PlayerData(m_localUserData.name, NetworkManager.Singleton.LocalClientId);
            VerifyConnection_ServerRpc(m_localUserData.id);
        }

        public override void OnNetworkDespawn()
        {
            m_onGameEnd(); // As a backup to ensure in-game objects get cleaned up, if this is disconnected unexpectedly.
        }

        private void FinishInitialize()
        {

        }

        /// <summary>
        /// To verify the connection, invoke a server RPC call that then invokes a client RPC call. After this, the actual setup occurs.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void VerifyConnection_ServerRpc(ulong clientId)
        {
            VerifyConnection_ClientRpc(clientId);

            // While we could start pooling symbol objects now, incoming clients would be flooded with the Spawn calls.
            // This could lead to dropped packets such that the InGameRunner's Spawn call fails to occur, so we'll wait until all players join.
            // (Besides, we will need to display instructions, which has downtime during which symbol objects can be spawned.)
        }

        [ClientRpc]
        private void VerifyConnection_ClientRpc(ulong clientId)
        {
            if (clientId == m_localUserData.id)
                VerifyConnectionConfirm_ServerRpc(m_localUserData);
        }

        /// <summary>
        /// Once the connection is confirmed, spawn a player cursor and check if all players have connected.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void VerifyConnectionConfirm_ServerRpc(PlayerData clientData)
        {
            // Note that the client will not receive the cursor object reference, so the cursor must handle initializing itself.
            //m_dataStore.AddPlayer(clientData.id, clientData.name);
            // The game will begin at this point, or else there's a timeout for booting any unconnected players.
            bool areAllPlayersConnected = NetworkManager.Singleton.ConnectedClients.Count >= m_expectedPlayerCount;
            VerifyConnectionConfirm_ClientRpc(clientData.id, areAllPlayersConnected);
        }

        [ClientRpc]
        private void VerifyConnectionConfirm_ClientRpc(ulong clientId, bool canBeginGame)
        {
            if (clientId == m_localUserData.id)
            {
                m_onConnectionVerified?.Invoke();
                m_hasConnected = true;
            }

            if (canBeginGame && m_hasConnected)
            {
                m_timeout = -1;
                BeginGame();
            }
        }

        /// <summary>
        /// The game will begin either when all players have connected successfully or after a timeout.
        /// </summary>
        void BeginGame()
        {
            GameManager.Instance.BeginGame();
            onGameBeginning?.Invoke();
        }

        /// <summary>
        /// Called while on the host to determine if incoming input has scored or not.
        /// </summary>
        //public void OnPlayerInput(ulong playerId, SymbolObject selectedSymbol)
        //{
        //    if (selectedSymbol.Clicked)
        //        return;

        //    if (m_sequenceSelector.ConfirmSymbolCorrect(playerId, selectedSymbol.SymbolIndex))
        //    {
        //        selectedSymbol.ClickedSequence_ServerRpc(playerId);
        //        m_scorer.ScoreSuccess(playerId);
        //        OnSymbolDeactivated();
        //    }
        //    else
        //        m_scorer.ScoreFailure(playerId);
        //}

        /// <summary>
        /// The server determines when the game should end. Once it does, it needs to inform the clients to clean up their networked objects first,
        /// since disconnecting before that happens will prevent them from doing so (since they can't receive despawn events from the disconnected server).
        /// </summary>
        //[ClientRpc]
        //private void WaitForEndingSequence_ClientRpc()
        //{
        //    m_scorer.OnGameEnd();
        //    m_introOutroRunner.DoOutro(EndGame);
        //}

        private void EndGame()
        {
            if (IsHost)
                StartCoroutine(EndGame_ClientsFirst());
        }

        private IEnumerator EndGame_ClientsFirst()
        {
            EndGame_ClientRpc();
            yield return null;
            SendLocalEndGameSignal();
        }

        [ClientRpc]
        private void EndGame_ClientRpc()
        {
            if (IsHost)
                return;
            SendLocalEndGameSignal();
        }

        private void SendLocalEndGameSignal()
        {
            m_onGameEnd();
        }
    }
}