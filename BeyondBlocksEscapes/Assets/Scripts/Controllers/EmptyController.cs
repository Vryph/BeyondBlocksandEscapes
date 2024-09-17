using UnityEngine;

namespace BBE {
    [CreateAssetMenu(fileName = "EmptyController", menuName = "InputController/EmptyController")]
    public class EmptyController : InputController
    {
        public override bool RetrieveJumpInput()
        {
            return false;
        }


        public override float RetrieveMoveInput()
        {
            return 0f;
        }
    }
}
