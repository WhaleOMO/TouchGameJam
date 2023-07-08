using System;
using UnityEngine;
using UnityEngine.Events;

struct RayCastOrigins
{
	public Vector2 topLeft, topRight;
	public Vector2 bottomLeft, bottomRight;
}

[RequireComponent(typeof(BoxCollider2D))]
public class MovementController : MonoBehaviour
{
	private const float skinWidth = 0.015f;

	public LayerMask obstacleMask;
	public LayerMask crouchableMask;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	private float horizontalRaySpacing;
	private float verticalRaySpacing;
	private BoxCollider2D collider;
	private RayCastOrigins rayCastOrigins;
	private bool isFacingRight = true;  // For determining which way the player is currently facing.
	
	private void Start()
	{
		collider = GetComponent<BoxCollider2D>();
		CalculateRaySpacing();
	}

	public void Move(Vector3 velocity)
	{
		UpdateRaycastsOrigins();
		if (velocity.x != 0)
		{
			HorizontalCollision(ref velocity);
		}

		if (velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}
		
		transform.Translate(velocity);
	}

	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;
		Vector2 rayOrigin = (directionY == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.topLeft;
		
		for (int i = 0; i < verticalRayCount; i++)
		{
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, obstacleMask);
			
			if (hit)
			{
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = Mathf.Min(hit.distance,rayLength);
			}
			
			Debug.DrawRay(rayOrigin, Vector3.up * (directionY * rayLength), Color.red);
		}
	}
	
	void HorizontalCollision(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;
		Vector2 rayOrigin = (directionX == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight;
		
		for (int i = 0; i < horizontalRayCount; i++)
		{
			rayOrigin += Vector2.up * (verticalRaySpacing * i);
			
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, obstacleMask);
			
			if (hit)
			{
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = Mathf.Min(hit.distance,rayLength);
			}
			
			Debug.DrawRay(rayOrigin, Vector3.right * (directionX * rayLength), Color.yellow);
		}
	}

	void UpdateRaycastsOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		rayCastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		rayCastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		rayCastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		rayCastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);
		
		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		isFacingRight = !isFacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
