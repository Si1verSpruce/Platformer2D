using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _velocity;
    [SerializeField] private Transform _path;

    private Transform[] _points;
    private int _targetPoint;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private float _lastPositionX;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _points = new Transform[_path.childCount];

        for (int i = 0; i < _points.Length; i++)
            _points[i] = _path.GetChild(i);
    }

    private void Update()
    {
        Transform target = _points[_targetPoint];
        transform.position = new Vector2(Mathf.MoveTowards(transform.position.x, target.position.x, _velocity * Time.deltaTime), transform.position.y);

        if (transform.position.x == target.position.x)
            SetNextTargetPoint();

        float direction = transform.position.x - _lastPositionX;

        FlipInMovementDirection(direction);

        _lastPositionX = transform.position.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != null)
        {
            GameObject enemy = collision.gameObject;

            if (collision.gameObject.TryGetComponent<Player>(out Player player))
                Destroy(enemy);
        }
    }

    private void SetNextTargetPoint()
    {
        if (_targetPoint == _points.Length - 1)
            _targetPoint = 0;
        else
            _targetPoint++;
    }

    private void FlipInMovementDirection(float direction)
    {
        if (direction > 0)
        {
            if (_spriteRenderer.flipX == true)
                _spriteRenderer.flipX = false;
        }
        else if (direction < 0)
            if (_spriteRenderer.flipX == false)
                _spriteRenderer.flipX = true;
    }
}
