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

        FlipInMovementDirection();
    }

    private void SetNextTargetPoint()
    {
        if (_targetPoint == _points.Length - 1)
            _targetPoint = 0;
        else
            _targetPoint++;
    }

    private void FlipInMovementDirection()
    {
        if (_velocity > 0)
            _spriteRenderer.flipX = false;
        else if (_velocity < 0)
            _spriteRenderer.flipX = true;
    }
}
