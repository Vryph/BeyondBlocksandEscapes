using UnityEngine;

namespace BBE
{
    public class Checkpoint : MonoBehaviour
    {
        public bool HasTriggered = false;
        public int Id { private get; set; }

        [SerializeField] private Checkpoint _checkpointPair;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType().ToString() == "UnityEngine.BoxCollider2D")
            {
                switch (Id)
                {
                    case 0:
                        if (!_checkpointPair.HasTriggered)
                        {
                            HasTriggered = true;
                        }
                        break;
                    case 1:
                        HasTriggered = !HasTriggered;
                        break;
                    default: break;
                }
            }
        }
    }
}