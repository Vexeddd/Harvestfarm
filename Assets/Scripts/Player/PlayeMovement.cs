using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayeMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 30f;
    public bool canMove = true;
    private bool isRun = false;
    private Vector3 targetVelocity;

    private Vector3 moveDir;
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 checkPos;
    private Quaternion targetRotation;

    private Rigidbody _rb;
    private PersonCamera personCamera;
    private Animator _animator;

    public bool isGrounded;
    public float jumpForce = 30f;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = this.AddComponent<Rigidbody>();
        }

        personCamera = Camera.main.GetComponent<PersonCamera>();

        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            capsuleCollider = this.AddComponent<CapsuleCollider>();
        }
        _animator = GetComponentInChildren<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputContextManager.Instance.Current != InputContext.GamePlay)
        {
            return;
        }

        MoveInputUpdate();
        JumpUpdate();
    }

    void FixedUpdate()
    {
        if (InputContextManager.Instance.Current != InputContext.GamePlay)
        {
            return;
        }

        ApplyMovement();
        ApplyRotation();
        CheckGround();
    }


    private void MoveInputUpdate()
    {
        if (!canMove)
        {
            return;
        }
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        cameraForward = personCamera.GetCameraFoward();
        cameraRight = Quaternion.Euler(0f, 90f, 0f) * cameraForward;

        moveDir = (h * cameraRight + v * cameraForward).normalized;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
        targetVelocity = moveDir * (isRun ? sprintSpeed : moveSpeed);

        _animator.SetFloat("speed", (isRun ? (moveDir.magnitude) : (moveDir.magnitude / 2)));
    }

    private void JumpUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            _animator.SetTrigger("jump");

            //²¥·ÅÒôÐ§
            MusicManager.Instance.PlayOneShot(MusicType.Jump);
        }
    }

    private void ApplyMovement()
    {
        if (!canMove) return;
        _rb.velocity = new Vector3(targetVelocity.x, _rb.velocity.y, targetVelocity.z);
    }


    private void ApplyRotation()
    {
        if (moveDir.magnitude > 0.1f)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }



    public float checkOffset = 0.15f;
    private CapsuleCollider capsuleCollider;

    private void CheckGround()
    {
        checkPos = transform.position + Vector3.up * checkOffset;
        isGrounded = Physics.CheckSphere(checkPos, capsuleCollider.radius * 0.9f, LayerMask.GetMask("Ground", "Farmland"));
        _animator.SetBool("isGrounded", isGrounded);

    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(checkPos, capsuleCollider.radius * 0.9f);
    }

}
