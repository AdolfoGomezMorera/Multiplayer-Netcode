using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public override void OnNetworkSpawn()
        {
            Move();

            if (IsOwner)
            {

                // Buscamos los elementos en la escena
                Camera camara = Camera.main;
                TMP_Text textoEstrellas = GameObject.Find("TextoEstrellas")?.GetComponent<TMP_Text>();
                TMP_Text textoVictoria = GameObject.Find("TextoVictoria")?.GetComponent<TMP_Text>();
                TMP_Text textoTiempo = GameObject.Find("TextoTiempo")?.GetComponent<TMP_Text>();

                // Le pasamos las referencias al PrimerScript
                PrimerScript ps = GetComponent<PrimerScript>();
                if (ps != null)
                {
                    ps.AsignarReferencias(camara, textoEstrellas, textoVictoria, textoTiempo);
                }
            }
        }

        public void Move()
        {
            if (IsServer)
            {
                SubmitPositionRequestServerRpc();
            }
            else
            {
                SubmitPositionRequestOwnerRpc();
            }
        }

        [Rpc(SendTo.Server)]
        void SubmitPositionRequestServerRpc(RpcParams rpcParams = default)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }

        [Rpc(SendTo.Owner)]
        void SubmitPositionRequestOwnerRpc(RpcParams rpcParams = default)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update()
        {
            if (IsOwner)
            {
                Vector3 input = Vector3.zero;

                if (Keyboard.current != null)
                {
                    if (Keyboard.current.wKey.isPressed) input += Vector3.forward;
                    if (Keyboard.current.sKey.isPressed) input += Vector3.back;
                    if (Keyboard.current.aKey.isPressed) input += Vector3.left;
                    if (Keyboard.current.dKey.isPressed) input += Vector3.right;
                }

                if (input != Vector3.zero)
                {
                    transform.position += input.normalized * 3f * Time.deltaTime;
                }
            }
        }
    }
}
