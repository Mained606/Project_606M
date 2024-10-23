using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Project_606M
{  
    public class PlayerMovement : MonoBehaviour
    {
        #region Variables
        private CharacterController characterController;
        private PlayerInput playerInput;
        private Animator animator;
        private Vector2 input;
        private Vector3 velocity;

        private PlayerState currentState;
        private PlayerState previousState;

        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;

        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;
        #endregion

        void Start()
        {
            //초기화
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            animator = GetComponent<Animator>();
            currentState = PlayerState.Player_Idle;
        }

        void FixedUpdate()
        {
            Move();
            ApplyGravity();
        }

        public void SetState(PlayerState newState)
        {
            if (currentState == newState)
            return;

            // 이전 상태 저장
            previousState = currentState;
            // 현재 상태 저장
            currentState = newState;
            // 애니메이션 상태 변경
            animator.SetInteger("PlayerState", (int)newState);
        }

        //이동 함수
        void Move()
        {
            //Vector2 입력을 Vector3로 변환하여 방향 벡터 생성
            Vector3 dir = new Vector3(input.x, 0f, input.y);
            
            //상태 변경 조건 확인
            if (dir == Vector3.zero)
            {
                SetState(PlayerState.Player_Idle);
            }
            else
            {
                SetState(PlayerState.Player_Walk);
            }
            
            //이동 방향으로 회전
            if (dir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }

            //이동
            characterController.Move(dir * Time.fixedDeltaTime * walkSpeed);
        }

        //중력 적용 함수
        void ApplyGravity()
        {
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Ensure the player sticks to the ground
            }

            velocity.y += gravity * Time.fixedDeltaTime;
            characterController.Move(velocity * Time.fixedDeltaTime);
        }

        //이동 입력 이벤트 함수
        public void OnMove(InputAction.CallbackContext context)
        {
            input = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            Debug.Log("Attack");    
            if (context.started)
            {
                SetState(PlayerState.Player_Attack);
                // animator.SetTrigger("AttackTrigger");
            }
        }

        //점프 입력 이벤트 함수
        public void OnJump(InputAction.CallbackContext context)
        {
            Debug.Log("Jump");
            if (context.performed)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                SetState(PlayerState.Player_Jump);
            }
        }
    }
    
}
