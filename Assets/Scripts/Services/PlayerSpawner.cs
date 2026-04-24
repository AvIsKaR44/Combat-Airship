using UnityEngine;


public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private Player m_PlayerPrefab;
    [SerializeField] private MovementController m_MovementControllerPrefab;
    [SerializeField] private VirtualGamePad m_VirtualGamePadPrefab;

    [SerializeField] private Transform m_SpawnPoint;

    public Player Spawn()
    {
        VirtualGamePad virtualGamePad = Instantiate(m_VirtualGamePadPrefab);
        MovementController movementController = Instantiate(m_MovementControllerPrefab);
        movementController.Construct(virtualGamePad);

        Player player = Instantiate(m_PlayerPrefab);

        player.Construct(movementController, m_SpawnPoint);

        return player;
    }
}


