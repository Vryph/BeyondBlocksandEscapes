using UnityEngine;

namespace BBE
{
    public abstract class InputController : ScriptableObject
    {
        public abstract float RetrieveMoveInput();
        public abstract bool RetrieveJumpInput();

        public abstract bool RetrieveDashInput();
    }
}
