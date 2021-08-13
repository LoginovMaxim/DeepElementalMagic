using System;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
	[SerializeField] private float _speedMovement;
    [SerializeField] private float _mumpForce;
	[Range(0, .3f)] [SerializeField] private float _movementSmoothing;
	[SerializeField] private bool _airControl = false;
	[SerializeField] private LayerMask _whatIsGround;
	[SerializeField] private Transform _groundCheck;

	const float _groundedRadius = .2f;
	private bool _grounded;
	private Rigidbody2D _rigidbody2D;
	private bool _facingRight = true;
	private Vector3 _velocity = Vector3.zero;

	private void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		var movement = Input.GetAxis("Horizontal") * _speedMovement * _movementSmoothing;

		var isJump = Input.GetKeyDown(KeyCode.Space);
		
		Move(movement, isJump);
	}

	private void FixedUpdate()
	{
		var wasGrounded = _grounded;
		_grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		var colliders = Physics2D.OverlapCircleAll(_groundCheck.position, _groundedRadius, _whatIsGround);
		for (var i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				_grounded = true;
			}
		}
	}

	public void Move(float move, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if (_grounded || _airControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			_rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity, _movementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !_facingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && _facingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (_grounded && jump)
		{
			// Add a vertical force to the player.
			_grounded = false;
			_rigidbody2D.AddForce(new Vector2(0f, _mumpForce));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		_facingRight = !_facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
