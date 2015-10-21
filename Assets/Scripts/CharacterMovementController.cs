using UnityEngine;
using System.Collections.Generic;

public class CharacterMovementController : MonoBehaviour{
    
    private MovementState currentMovementState;
    public string MoveState;

    public static float Epsilon = 0.003f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    if (currentMovementState == null)
	    {
	        currentMovementState = new Idle(this);
	    }
        currentMovementState.Update();
	    MoveState = currentMovementState.ToString().Replace("CharacterMovementController", "");
	}

    public void SetMoveTargetAndGo(Tile target, System.Action action)
    {
        currentMovementState = new OnTheMove(this, target, action);
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
        public System.Action act;


        public OnTheMove(CharacterMovementController characterController, List<Tile> path) :
            base(characterController)
        {
            _path = path;
        }

        public OnTheMove(CharacterMovementController characterMovementController, Tile start, System.Action action)
            : base(characterMovementController)
        {
            _path = new List<Tile>() {start};
            act = action;
        }

        public override void Update()
        {
            if (_path.Count > 0 || _currentTargetTile != null)
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
                        CharacterMovementController.GetComponent<SlideCharacter>().SetTile(_currentTargetTile);
                        _currentTargetTile = null;
                        if (_path.Count == 0)
                        {
                            CharacterMovementController.currentMovementState = new Idle(CharacterMovementController);
                            if (act != null)
                            {
                                act();
                            }
                        }

                    }
                }
            }
            base.Update();
        }
    }


}
