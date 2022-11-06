using Demonics.Sounds;
using FixMath.NET;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask _wallLayerMask = default;
    [SerializeField] private LayerMask _playerLayerMask = default;
    private Player _player;
    private Audio _audio;
    private FixVector2 _velocity;
    private Coroutine _knockbackCoroutine;
    private int _knockbackFrame;
    private int _knockbackDuration;

    private readonly Fix64 _cornerLimit = (Fix64)10.46;
    public DemonicsPhysics Physics { get; private set; }
    public bool HasJumped { get; set; }
    public bool HasDoubleJumped { get; set; }
    public bool HasAirDashed { get; set; }
    public Fix64 MovementSpeed { get; set; }
    public Vector2 MovementInput { get; set; }
    public bool IsGrounded { get; set; } = true;
    public bool IsInCorner { get; private set; }
    public bool IsInHitstop { get; private set; }

    void Awake()
    {
        _player = GetComponent<Player>();
        Physics = GetComponent<DemonicsPhysics>();
        _audio = GetComponent<Audio>();
    }

    void Start()
    {
        MovementSpeed = _player.playerStats.SpeedWalk;
    }

    void FixedUpdate()
    {
        CheckKnockback();
        CheckCorner();
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        if (Physics.Position.y == DemonicsPhysics.GROUND_POINT)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    public void TravelDistance(Vector2 travelDistance)
    {
        Physics.Velocity = new FixVector2((Fix64)travelDistance.x, (Fix64)travelDistance.y);
    }

    public void CheckForPlayer()
    {
        float space = 0.685f;
        for (int i = 0; i < 3; i++)
        {
            Debug.DrawRay(new Vector2(transform.position.x + space, transform.position.y), Vector2.down, Color.green);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + space, transform.position.y), Vector2.down, 0.6f, _playerLayerMask);
            if (hit.collider != null)
            {
                if (hit.collider.transform.root != transform.root)
                {
                    if (hit.collider.transform.root.TryGetComponent(out Player player))
                    {
                        float pushboxSizeX = hit.collider.GetComponent<BoxCollider2D>().size.x;
                    }
                }
            }
            space -= 0.685f;
        }
    }

    public void AddForce(float moveHorizontally)
    {
        int direction = 0;
        if (moveHorizontally == 1)
        {
            direction = (int)transform.localScale.x * -1;
        }
    }

    public void KnockbackNow(Vector2 knockbackDirection, Vector2 knockbackForce, int knockbackDuration)
    {
        _startPosition = Physics.Position;
        _endPosition = new FixVector2(Physics.Position.x + (Fix64)knockbackDirection.x * (Fix64)knockbackForce.x, Physics.Position.y + (Fix64)knockbackDirection.y * (Fix64)knockbackForce.y); ;
        _knockbackDuration = knockbackDuration;
    }

    public void Knockback(Vector2 knockbackDirection, Vector2 knockbackForce, int knockbackDuration)
    {
        _player.hitstopEvent.AddListener(() =>
        {
            _startPosition = Physics.Position;
            _endPosition = new FixVector2(Physics.Position.x + (Fix64)knockbackDirection.x * (Fix64)knockbackForce.x, Physics.Position.y + (Fix64)knockbackDirection.y * (Fix64)knockbackForce.y); ;
            _knockbackDuration = knockbackDuration;
        });
    }

    public void StopKnockback()
    {
        if (_knockbackCoroutine != null)
        {
            StopCoroutine(_knockbackCoroutine);
        }
    }
    FixVector2 _startPosition;
    FixVector2 _endPosition;
    private void CheckKnockback()
    {
        if (_knockbackDuration > 0)
        {
            _knockbackFrame++;
            Fix64 ratio = (Fix64)_knockbackFrame / (Fix64)_knockbackDuration;
            Physics.SetPositionWithRender(new FixVector2(_startPosition.x * ((Fix64)1 - ratio) + _endPosition.x * ratio, _startPosition.y * ((Fix64)1 - ratio) + _endPosition.y * ratio));
            if (_knockbackFrame == _knockbackDuration)
            {
                Physics.Velocity = FixVector2.Zero;
                Physics.Position = _endPosition;
                _knockbackDuration = 0;
                _knockbackFrame = 0;
            }
        }
    }

    private void CheckCorner()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(-transform.localScale.x, 0.0f), 1.0f, _wallLayerMask);
        if (Physics.Position.x >= Fix64.Abs(_cornerLimit))
        {
            IsInCorner = true;
        }
        else
        {
            IsInCorner = false;
        }
    }

    public Vector2 OnWall()
    {
        if (!IsInHitstop)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(-transform.localScale.x, 0.0f), 2.0f, _wallLayerMask);
            if (hit.collider != null)
            {
                return new Vector2(hit.point.x - (0.2f * -transform.localScale.x), transform.localPosition.y);
            }
            else
            {
                return Vector2.zero;
            }
        }
        return Vector2.zero;
    }

    public void ResetGravity()
    {
    }

    public void ZeroGravity()
    {
    }

    public void ResetToWalkSpeed()
    {
        if (MovementSpeed == _player.playerStats.SpeedRun)
        {
            _audio.Sound("Run").Stop();
            MovementSpeed = _player.playerStats.SpeedWalk;
        }
    }

    public void EnterHitstop()
    {
        if (!IsInHitstop)
        {
            IsInHitstop = true;
            _velocity = Physics.Velocity;
            Physics.SetFreeze(true);
        }
    }

    public void ExitHitstop()
    {
        if (IsInHitstop)
        {
            IsInHitstop = false;
            Physics.SetFreeze(false);
            Physics.Velocity = _velocity;
        }
    }

    public void SetRigidbodyKinematic(bool state)
    {

    }
}
