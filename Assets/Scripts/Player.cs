using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class Player : MonoBehaviour
{
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _jumpForce;
    [SerializeField] private LayerMask _platformLayer;

    [SerializeField] private float _velocity;
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider2D;
    private Animator _animator;

    private void Awake()
    {
        _velocity = 0;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.D))
            {
                MoveRight();
            }

            if (Input.GetKey(KeyCode.A))
            {
                MoveLeft();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            if (_velocity != 0)
            {
                _velocity = Mathf.MoveTowards(_velocity, 0, _acceleration * Time.deltaTime);
            }
        }

        if (_velocity != 0)
        {
            transform.Translate(Vector2.right * _velocity * Time.deltaTime);
        }
    }

    private void MoveRight()
    {
        string runRightTrigger = "Movement Right";

        _animator.SetTrigger(runRightTrigger);

        _velocity = Mathf.MoveTowards(_velocity, _maxVelocity, _acceleration * Time.deltaTime);
    }

    private void MoveLeft()
    {
        _velocity = Mathf.MoveTowards(_velocity, _maxVelocity * -1, _acceleration * Time.deltaTime);
    }

    private void Jump()
    {
        if (CheckIsOnGround())
        {
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool CheckIsOnGround()
    {
        float offsetY = 0.1f;
        float boxSizeXModifier = 0.6f;
        RaycastHit2D castForPlatform = Physics2D.BoxCast(_boxCollider2D.bounds.center, new Vector2(_boxCollider2D.bounds.size.x * boxSizeXModifier, _boxCollider2D.bounds.size.y),
            0, Vector2.down, offsetY, _platformLayer);

        if (castForPlatform.collider != null)
        {
            return castForPlatform.collider.gameObject.TryGetComponent(out Platform platform);
        }

        return false;
    }
}
