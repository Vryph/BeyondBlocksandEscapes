using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace BBE
{
    public class RaycastDataRetrieval : MonoBehaviour
    {

        [SerializeField] private Rigidbody2D _body;

        [Header("Ceiling Edge Detection Parameters")]
        [SerializeField, Range(0.1f, 2f)] private float _xOffsetCeiling = 0.5f;
        [SerializeField, Range(0.1f, 2f)] private float _yOffsetCeiling = 0.3f;
        [SerializeField, Range(0.05f, 0.2f)] private float _raysOffsetCeiling = 0.1f;
        [SerializeField, Range(0.05f, 2f)] private float _rayLenghtCeiling = 0.1f;

        [Header("Dash Edge Detection Parameters")]
        [SerializeField, Range(-2f, 2f)] private float _xOffsetDash = 0.5f;
        [SerializeField, Range(-2f, 2f)] private float _yOffsetDash = 0.3f;
        [SerializeField, Range(0.05f, 0.2f)] private float _raysOffsetDash = 0.1f;
        [SerializeField, Range(0.05f, 2f)] private float _rayLenghtDash = 0.1f;


        public bool DashEdgeDetected { get; private set; }
        public bool CeilingEdgeDetected { get; private set; }
        public float CeilingDistance {  get; private set; }
        public float DashDistance { get; private set; }



        private bool _ceilingEdgeHit, _ceilingLimitHit, _dashEdgeHit, _dashLimitHit;
        Vector3 _edgeCastOriginCeiling, _limitCastOriginCeiling, _lengthCastOriginCeiling, _edgeCastOriginDash, _limitCastOriginDash, _lengthCastOriginDash;
        int _mask, _raycastDirection;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
        }


        public void CeilingEdgeDetection()
        {
            _mask = ~LayerMask.GetMask("Player");

            _edgeCastOriginCeiling = new Vector3(transform.position.x - _xOffsetCeiling * _raycastDirection, transform.position.y + _yOffsetCeiling, transform.position.z);
            _limitCastOriginCeiling = new Vector3(transform.position.x - (_xOffsetCeiling - _raysOffsetCeiling) * _raycastDirection, transform.position.y + _yOffsetCeiling, transform.position.z);
            _lengthCastOriginCeiling = new Vector3(_limitCastOriginCeiling.x, _limitCastOriginCeiling.y + _rayLenghtCeiling, transform.position.z);

            if (Physics2D.Raycast(_edgeCastOriginCeiling, Vector2.up, _rayLenghtCeiling, _mask))
            {
                _ceilingEdgeHit = true;
            }
            else _ceilingEdgeHit = false;

            if (Physics2D.Raycast(_limitCastOriginCeiling, Vector2.up, _rayLenghtCeiling, _mask))
            {
                _ceilingLimitHit = true;
            }
            else _ceilingLimitHit = false;

            CeilingEdgeDetected = _ceilingEdgeHit && !_ceilingLimitHit;

            if (CeilingEdgeDetected)
            {
                Vector3 direction = _raycastDirection == 1 ? Vector3.right : Vector3.left;
                RaycastHit2D _hit = Physics2D.Raycast(_lengthCastOriginCeiling, -direction, 1, _mask);
                CeilingDistance = Mathf.Abs(_edgeCastOriginCeiling.x - _hit.point.x);
                //Debug.Log($"{Distance} {_hit.point}");
            }
        }

        public void DashEdgeDetection()
        {
            _mask = ~LayerMask.GetMask("Player");

            Vector3 direction = _raycastDirection == 1 ? Vector3.right : Vector3.left;

            _edgeCastOriginDash = new Vector3(transform.position.x + _xOffsetDash * _raycastDirection, transform.position.y + _yOffsetDash, transform.position.z);
            _limitCastOriginDash = new Vector3(_edgeCastOriginDash.x, transform.position.y + (_yOffsetDash + _raysOffsetDash), transform.position.z);
            _lengthCastOriginDash = new Vector3(_edgeCastOriginDash.x + _rayLenghtDash * _raycastDirection, _limitCastOriginDash.y, transform.position.z);

            if (Physics2D.Raycast(_edgeCastOriginDash, direction, _rayLenghtDash, _mask))
            {
                _dashEdgeHit = true;
            }
            else _dashEdgeHit = false;

            if (Physics2D.Raycast(_limitCastOriginDash, direction, _rayLenghtDash, _mask))
            {
                _dashLimitHit = true;
            }
            else _dashLimitHit = false;

            DashEdgeDetected = _dashEdgeHit && !_dashLimitHit;

            if (DashEdgeDetected)
            {
               RaycastHit2D _hit = Physics2D.Raycast(_lengthCastOriginDash, Vector3.down, 0.5f, _mask);
               DashDistance = Mathf.Abs(_edgeCastOriginDash.y - _hit.point.y);
               //Debug.Log($"{DashDistance} {_hit.point}");
            }
        }


        public void SetDirection()
        {
            _raycastDirection = _body.velocity.x < 0 ? -1 : 1;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _ceilingEdgeHit ? Color.red : Color.green;
            Gizmos.DrawLine(_edgeCastOriginCeiling, new Vector3(_edgeCastOriginCeiling.x, _edgeCastOriginCeiling.y + _rayLenghtCeiling, transform.position.z));

            Gizmos.color = _ceilingLimitHit ? Color.red : Color.green;
            Gizmos.DrawLine(_limitCastOriginCeiling, new Vector3(_limitCastOriginCeiling.x, _limitCastOriginCeiling.y + _rayLenghtCeiling, transform.position.z));

            Gizmos.color = _dashEdgeHit ? Color.red : Color.green;
            Gizmos.DrawLine(_edgeCastOriginDash, new Vector3(_edgeCastOriginDash.x + _rayLenghtDash * _raycastDirection, _edgeCastOriginDash.y, transform.position.z));

            Gizmos.color = _dashLimitHit ? Color.red : Color.green;
            Gizmos.DrawLine(_limitCastOriginDash, new Vector3(_limitCastOriginDash.x + _rayLenghtDash * _raycastDirection, _limitCastOriginDash.y, transform.position.z));

            Gizmos.color = CeilingEdgeDetected ? Color.red : Color.green;
            Gizmos.DrawLine(_lengthCastOriginCeiling, new Vector3(_lengthCastOriginCeiling.x + (1 * -_raycastDirection), _lengthCastOriginCeiling.y, transform.position.z));

            Gizmos.color = DashEdgeDetected ? Color.red : Color.green;
            Gizmos.DrawLine(_lengthCastOriginDash, new Vector3(_lengthCastOriginDash.x, _lengthCastOriginDash.y - 0.5f, transform.position.z));
        }
        
    }
}
