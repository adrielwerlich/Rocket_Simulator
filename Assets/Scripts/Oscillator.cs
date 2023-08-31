using UnityEngine;

public class Oscillator : MonoBehaviour
{
    private Vector3 startPosition;

    [Header("Type of oscillation")]
    public MovementType movementType = MovementType.UpDown;

    [Header("General setting for all movements")]
    public float speed = 2f;
    public int amplitude = 10;

    [Header("Rotation and Back and Forth")]
    public bool reverseDirection = false;




    [Header("Rotation Only")]
    public float radius = 1f;
    public float rotationSpeed = 90f;
    private float angle;


    [Header("Back and Forth")]
    public float distance = 10f;
    public bool movingForward = true;
    private float startPositionZ;
    private float endPositionZ;
    private float reversedEndPositionZ;


    public enum MovementType
    {
        UpDown,
        LeftRight,
        BackForth,
        RotateHorizontally,
        Rotate
    }

    void Start()
    {
        startPosition = transform.position;
        startPositionZ = transform.position.z;
        endPositionZ = startPositionZ + distance;
        reversedEndPositionZ = startPositionZ - distance;
    }

    // Update is called once per frame
    void Update()
    {
        switch (movementType)
        {
            case MovementType.UpDown:
                OscillateUpDown();
                break;
            case MovementType.LeftRight:
                OscillateLeftRight();
                break;
            case MovementType.BackForth:
                OscillateBackAndForth();
                break;
            case MovementType.Rotate:
                if (!reverseDirection)
                {
                    RotateClockwise();
                } else
                {
                    RotateCounterClockwise();
                }
                break;
            case MovementType.RotateHorizontally:
                if (reverseDirection)
                {
                    RotateCounterClockwiseHorizontally();
                }
                else
                {
                    RotateClockwiseHorizontally();
                }
                break;
                
        }
    }

    private void OscillateUpDown()
    {
        // Calculate the vertical movement
        float verticalMovement = Mathf.Sin(Time.time * speed) * 2f * amplitude;

        // Update the object's position
        transform.position = new Vector3(startPosition.x, startPosition.y + verticalMovement, startPosition.z);
    }

    private void OscillateLeftRight()
    {
        // Calculate the horizontal movement
        float horizontalMovement = Mathf.Sin(Time.time * speed) * 2f * amplitude;

        // Update the object's position
        transform.position = new Vector3(startPosition.x + horizontalMovement, startPosition.y, startPosition.z);
    }

    private void OscillateBackAndForth()
    {
        // Check if the object has reached the end position
        float direction;
        if (reverseDirection)
        {
            if (transform.position.z <= reversedEndPositionZ)
                movingForward = false;
            else if (transform.position.z >= startPositionZ)
                movingForward = true;
            direction = movingForward ? -1 : 1;
        }
        else
        {
            if (transform.position.z >= endPositionZ)
                movingForward = false;
            else if (transform.position.z <= startPositionZ)
                movingForward = true;
            direction = movingForward ? 1 : -1;
        }

        // Move the object based on the current direction
        transform.Translate(Vector3.forward* direction * speed * Time.deltaTime);
    }

    private void RotateClockwise()
    {
        angle += Time.deltaTime * rotationSpeed;
        if (angle > 360f)
        {
            angle -= 360f;
        }

        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * amplitude * radius;
        float y = Mathf.Cos(angle * Mathf.Deg2Rad) * amplitude * radius;

        transform.position = startPosition + new Vector3(x, y, 0);
    }

    void RotateCounterClockwise() {
        angle -= Time.deltaTime* rotationSpeed;
        if (angle< 0f)
        {
            angle += 360f;
        }

        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * amplitude * radius;
        float y = Mathf.Cos(angle * Mathf.Deg2Rad) * amplitude * radius;

        if (reverseDirection)
        {
            x *= -1f;
            y *= -1f;
        }

        transform.position = startPosition + new Vector3(x, y, 0);    
    }

    private void RotateClockwiseHorizontally()
    {
        angle += Time.deltaTime * rotationSpeed;
        if (angle > 360f)
        {
            angle -= 360f;
        }

        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * amplitude * radius;
        float z = Mathf.Cos(angle * Mathf.Deg2Rad) * amplitude * radius;

        transform.position = startPosition + new Vector3(x, 0, z);
    }

    void RotateCounterClockwiseHorizontally()
    {
        angle -= Time.deltaTime * rotationSpeed;
        if (angle < 0f)
        {
            angle += 360f;
        }

        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * amplitude * radius;
        float z = Mathf.Cos(angle * Mathf.Deg2Rad) * amplitude * radius;

        if (reverseDirection)
        {
            x *= -1f;
            z *= -1f;
        }

        transform.position = startPosition + new Vector3(x, 0, z);
    }
}
