using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace BBE
{
    public class RaycastDataRetrieval : MonoBehaviour
    {

        [SerializeField] private Rigidbody2D _body;

        [Header("Ceiling Edge Detection Parameters")]
        [SerializeField, Range(0.1f, 2f)] private float _xOffset = 0.5f;
        [SerializeField, Range(0.1f, 2f)] private float _yOffset = 0.3f;
        [SerializeField, Range(0.05f, 0.2f)] private float _raysOffset = 0.1f;
        [SerializeField, Range(0.05f, 2f)] private float _rayLenght = 0.1f;

        public bool CeilingEdgeDetected { get; private set; }

        private bool _ceilingEdgeHit, _ceilingLimitHit;
        Vector3 _edgeCastOrigin, _limitCastOrigin;
        int _mask, _raycastDirection;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
        }


        public void CeilingEdgeDetection()
        {
            _mask = ~LayerMask.GetMask("Player");

            _edgeCastOrigin = new Vector3(transform.position.x - _xOffset * _raycastDirection, transform.position.y + _yOffset, transform.position.z);
            _limitCastOrigin = new Vector3(transform.position.x - (_xOffset - _raysOffset) * _raycastDirection, transform.position.y + _yOffset, transform.position.z);

            if (Physics2D.Raycast(_edgeCastOrigin, Vector2.up, _rayLenght, _mask))
            {
                _ceilingEdgeHit = true;
            }
            else _ceilingEdgeHit = false;

            if (Physics2D.Raycast(_limitCastOrigin, Vector2.up, _rayLenght, _mask))
            {
                _ceilingLimitHit = true;
            }
            else _ceilingLimitHit = false;

            CeilingEdgeDetected = _ceilingEdgeHit && !_ceilingLimitHit;

        }

        public void SetDirection()
        {
            _raycastDirection = _body.velocity.x < 0 ? -1 : 1;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _ceilingEdgeHit ? Color.red : Color.green;
            Gizmos.DrawLine(_edgeCastOrigin, new Vector3(_edgeCastOrigin.x, _edgeCastOrigin.y + _rayLenght, transform.position.z));

            Gizmos.color = _ceilingLimitHit ? Color.red : Color.green;
            Gizmos.DrawLine(_limitCastOrigin, new Vector3(_limitCastOrigin.x, _limitCastOrigin.y + _rayLenght, transform.position.z));
        }
        
    }
}
