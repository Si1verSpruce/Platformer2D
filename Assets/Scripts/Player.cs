using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    left = -1,
    right = 1
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class Player : MonoBehaviour
{
    private const string AnimatorIsIdleName = "Is Idle";
    private const string AnimatorIsMoveRightName = "Is Move Right";
    private const string AnimatorIsMoveLeftName = "Is Move Left";

    [SerializeField] private float _acceleration;
    [SerializeField] private float _inAirAcceleration;
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _jumpForce;
    [SerializeField] private LayerMask _platformLayer;

    private float _velocity;
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private List<string> _stateNames = new List<string>();
    private bool _isOnGround;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _stateNames.Add(AnimatorIsIdleName);
        _stateNames.Add(AnimatorIsMoveRightName);
        _stateNames.Add(AnimatorIsMoveLeftName);
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            _isOnGround = CheckIsOnGround();

            if (Input.GetKey(KeyCode.D))
                Move(Direction.right, AnimatorIsMoveRightName);

            if (Input.GetKey(KeyCode.A))
                Move(Direction.left, AnimatorIsMoveRightName);

            if (Input.GetKeyDown(KeyCode.Space))
                Jump();
        }
        else
            StopMovement();

        if (_velocity != 0)
            transform.Translate(Vector2.right * _velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
            if (collision.gameObject.TryGetComponent<Coin>(out Coin coin))
                Destroy(collision.gameObject);
    }

    private void Move(Direction direction, string stateName)
    {
        FlipInMovementDirection(direction);

        if (_isOnGround)
        {
            _velocity = Mathf.MoveTowards(_velocity, _maxVelocity * (int)direction, _acceleration * Time.deltaTime);
            SwitchAnimation(stateName);
        }
        else
        {
            _velocity = Mathf.MoveTowards(_velocity, _maxVelocity * (int)direction, _inAirAcceleration * Time.deltaTime);
            SwitchAnimation(AnimatorIsIdleName);
        }
    }

    private void Jump()
    {
        if (_isOnGround)
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private bool CheckIsOnGround()
    {
        float offsetY = 0.1f;
        float boxSizeXModifier = 0.6f;
        RaycastHit2D castForPlatform = Physics2D.BoxCast(_boxCollider2D.bounds.center, new Vector2(_boxCollider2D.bounds.size.x * boxSizeXModifier, _boxCollider2D.bounds.size.y),
            0, Vector2.down, offsetY, _platformLayer);

        if (castForPlatform.collider != null)
            return castForPlatform.collider.gameObject.TryGetComponent(out Platform platform);

        return false;
    }

    private void StopMovement()
    {
        SwitchAnimation(AnimatorIsIdleName);

        if (_velocity != 0)
            _velocity = Mathf.MoveTowards(_velocity, 0, _acceleration * Time.deltaTime);
    }

    private void SwitchAnimation(string newStateName)
    {
        if (_animator.GetBool(newStateName) != true)
        {
            _animator.SetBool(newStateName, true);

            foreach (string stateName in _stateNames)
                if (stateName != newStateName)
                    _animator.SetBool(stateName, false);
        }
    }

    private void FlipInMovementDirection(Direction direction)
    {
        if (direction == Direction.right)
        {
            if (_spriteRenderer.flipX == true)
                _spriteRenderer.flipX = false;
        }
        else if (direction == Direction.left)
            if (_spriteRenderer.flipX == false)
                _spriteRenderer.flipX = true;
    }
}
