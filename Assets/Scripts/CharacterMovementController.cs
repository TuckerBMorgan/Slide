using UnityEngine;
using System.Collections.Generic;

public class CharacterMovementController : MonoBehaviour{
    
    private MovementState currentMovementState;

    public static float Epsilon = 0.003f;

	// Use this for initialization
	void Start () {
        currentMovementState = new Idle(this);
	}
	
	// Update is called once per frame
	void Update () {
        currentMovementState.Update();
	}

    public class MovementState
    {
        protected CharacterMovementController CharacterMovementController;

        public MovementState(CharacterMovementController characterMovementController)
        {
            CharacterMovementController = characterMovementController;
        }

        public virtual void Update()
        {
            
        }

    }

    public class Idle : MovementState
    {
        public Idle(CharacterMovementController characterMovementController) :
            base(characterMovementController)
        {
            
        }

        public void MoveToPoint(List<Tile> movementPath)
        {
            CharacterMovementController.currentMovementState = new OnTheMove(CharacterMovementController, movementPath);
        }

    }

    public class OnTheMove : MovementState
    {
        private readonly List<Tile> _path;
        private Tile _currentTargetTile;
        private Vector3 _moveDirection;
        private float _distance;

        public OnTheMove(CharacterMovementController characterController, List<Tile> path) :
            base(characterController)
        {
            _path = path;
        }

        public override void Update()
        {
            if (_path.Count > 0)
            {
                if (_currentTargetTile == null)
                {
                    _currentTargetTile = _path[0];
                    _path.RemoveAt(0);
                    if (_currentTargetTile != null)
                    {
                        _moveDirection = _currentTargetTile.transform.position - CharacterMovementController.transform.position;
                        _moveDirection.y = 0;
                        CharacterMovementController.transform.position += _moveDirection.normalized/20;
                    }
                }
                else
                {
                    CharacterMovementController.transform.position += _moveDirection.normalized/20;
                    _distance = Mathf.Abs(Vector3.SqrMagnitude(CharacterMovementController.transform.position - _currentTargetTile.transform.position));
                    if (_distance <= Epsilon)
                    {
                        CharacterMovementController.transform.position = new Vector3(_currentTargetTile.X, 0, _currentTargetTile.Y);
                        _currentTargetTile = null;
                    }
                }
            }
            base.Update();
        }
    }


}
