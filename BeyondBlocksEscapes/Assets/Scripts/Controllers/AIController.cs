using UnityEngine;

namespace BBE
{
    [CreateAssetMenu(fileName = "AIController", menuName = "InputController/AIController")]
    public class AIController : InputController
    {
        public override bool RetrieveJumpInput()
        {
            return true;
        }

        public override float RetrieveMoveInput()
        {
            return 1f;
        }

        public override bool RetrieveDashInput()
        {
            return false;
        }
    }
}
