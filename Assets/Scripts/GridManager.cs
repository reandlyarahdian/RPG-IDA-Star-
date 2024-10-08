using UnityEditor;
using UnityEngine;

namespace GridSystem
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private float _nodeSize = 1f;
        [SerializeField] private LayerMask _wallLayerMask;
        [SerializeField] private bool _drawGridOnStart;
        
        private Transform _pointA;
        private Transform _pointB;
        
        public static GridManager Instance { get; private set; }
        public Grid Grid { get; private set; }

        private Vector3 _worldOrigin = Vector3.zero;
        private Vector3 _positionA;
        private Vector3 _positionB;

        private void Awake()
        {
            // singleton
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

            _pointA = transform.GetChild(0);
            _pointB = transform.GetChild(1);
        }

        private void Start()
        { 
            CreateGrid();
            LockArea(); // avoid editing visual area by mistake once the grid is created
        }

        #region GRID INITIALIZATION
        private void CreateGrid()
        {
            Vector2Int gridSize = Vector2Int.zero;
            gridSize += GetNumberOfCellsFromWorldOrigin(_pointA.position);
            gridSize += GetNumberOfCellsFromWorldOrigin(_pointB.position);
            
            Vector3 originPosition = GetGridOriginPosition();
            
            Grid = new Grid(gridSize.x, gridSize.y, _nodeSize, originPosition, _wallLayerMask);
            if (_drawGridOnStart)
                Grid.DrawGrid();
        }

        private void LockArea()
        {
            _positionA = _pointA.position;
            _positionB = _pointB.position;
            Destroy(_pointA.gameObject);
            Destroy(_pointB.gameObject);
        }
        
        private Vector2Int GetNumberOfCellsFromWorldOrigin(Vector3 position)
        {
            Vector2Int cells = new Vector2Int(
                Mathf.FloorToInt(Mathf.Abs(position.x - _worldOrigin.x) / _nodeSize),
                Mathf.FloorToInt(Mathf.Abs(position.y - _worldOrigin.y) / _nodeSize));
            
            return cells;
        }

        private Vector3 GetGridOriginPosition()
        {
            if (!_pointA || !_pointB) return Vector3.zero;
            
            // gets the left bottom corner of the grid, matching the nodes size
            
            Vector3 bottomLeftPosition = new Vector3(
                Mathf.Min(_pointA.position.x, _pointB.position.x),
                Mathf.Min(_pointA.position.y, _pointB.position.y),
                Mathf.Min(_pointA.position.z, _pointB.position.z));
            Vector2Int cells = GetNumberOfCellsFromWorldOrigin(bottomLeftPosition);
            Vector3 originPosition = new Vector3(
                cells.x * _nodeSize * Mathf.Sign(bottomLeftPosition.x),
                cells.y * _nodeSize * Mathf.Sign(bottomLeftPosition.y));
            
            return originPosition;
        }
        #endregion
        
        #region GIZMOS
        private void OnDrawGizmos()
        {
            if (IsSelectedOrChild())
            {
                Vector3 posA = Vector3.zero;
                Vector3 posB = Vector3.zero;
                if (Application.isPlaying)
                {
                    // once the grid is created, take the positions of the points
                    // avoid the user to change by mistake the positions, changing visually the grid area
                    posA = _positionA;
                    posB = _positionB;
                }
                else if (_pointA && _pointB)
                {
                    // update the positions during edit mode
                    posA = _pointA.position;
                    posB = _pointB.position;
                }
                
                Vector3 center = GetAreaCenter();
                Vector3 size = new Vector3(
                    Mathf.Abs(posA.x - posB.x),
                    Mathf.Abs(posA.y - posB.y),
                    0f);
                
                Gizmos.color = new Color(0, 1, 0, 0.2f);
                Gizmos.DrawCube(center, size);
                
                // area corners dots
                if (!Application.isPlaying)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(posA, 0.15f);
                    Gizmos.DrawSphere(posB, 0.15f);
                }
            }
        }

        private bool IsSelectedOrChild()
        {
            if (!Selection.activeGameObject)
                return false;

            if (Selection.activeGameObject == gameObject)
                return true;

            if (Selection.activeGameObject.transform.IsChildOf(gameObject.transform))
                return true;

            return false;
        }

        private Vector3 GetAreaCenter()
        {
            Vector3 posA = Vector3.zero;
            Vector3 posB = Vector3.zero;
            if (Application.isPlaying)
            {
                // once the grid is created, take the positions of the points
                // avoid the user to change by mistake the positions, changing visually the grid area
                posA = _positionA;
                posB = _positionB;
            }
            else if (_pointA && _pointB)
            {
                // update the positions during edit mode
                posA = _pointA.position;
                posB = _pointB.position;
            }
            
            Vector3 center = Vector3.zero;
            center.x = posA.x > posB.x ? (posA.x - posB.x) / 2 + posB.x : (posB.x - posA.x) / 2 + posA.x;
            center.y = posA.y > posB.y ? (posA.y - posB.y) / 2 + posB.y : (posB.y - posA.y) / 2 + posA.y;
            
            return center;
        }
        #endregion
    }
}