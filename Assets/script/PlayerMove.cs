using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public GameManager gameManager;
    public GameObject attack;
    public float maxSpeed;
    public float jumpPower;
    SpriteRenderer spriteRenderer;
    Animator anim;
    void Awake() {

      rigid = GetComponent<Rigidbody2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      anim = GetComponent<Animator>();
        attack = transform.GetChild(0).gameObject;
    }
    void Update() {

    //Jump�� �Էµǰ� �������� �ƴҰ�� rigid�� ���������� jumpPower��ŭ AddForce��
    if (Input.GetButtonDown("Jump") && !anim.GetBool("jumping")){
      rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
      anim.SetBool("jumping", true);
    }
      
    //����������� ���� �� ��� �÷��̾��� �ӵ��� ����
    if(Input.GetButtonUp("Horizontal")) {
      rigid.velocity = new Vector2(rigid.velocity.normalized.x* 0.5f, rigid.velocity.y);
    }

    //���� �������� ������ ��� �÷��̾ �ش������ �ٶ󺸵��� ���� 
    if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

    if(Input.GetButtonDown("Horizontal"))
        {
            if (spriteRenderer.flipX)
            {
                attack.transform.position = rigid.transform.position + new Vector3(-0.5f, 0, 0);
            }
            else
            {
                attack.transform.position = rigid.transform.position + new Vector3(0.5f, 0, 0);
            }
        }


        //�̵��ӵ��� 0.3���Ϸ� ������ ��� �̵� �ִϸ��̼� ����
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
      anim.SetBool("waking", false);
      else
      anim.SetBool("waking", true);
    }
    
    void FixedUpdate()
    {
        //����������� �Է� �� ��� ������Ʈ�� �ӵ���ŭ ������Ʈ�� ���� ����
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //���� ���� ������Ʈ�� �̵��ӵ��� �ִ�ӵ� �̻��� ��� ������Ʈ�� �ӵ��� �ִ�ӵ��� ������
        if (rigid.velocity.x > maxSpeed)
          rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed*(-1))
          rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

      //�÷��̾ �����ߴ��� �ٴ������� �Ǻ�
      //������Ʈ�� y �ӵ��� ������ ��� �Ʒ��������� Raycast�� ������ "Platform"�� ����
      if(rigid.velocity.y < 0) {
      Debug.DrawRay(rigid.position, Vector3.down, new Color(0,1,0));
      RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
      //������ ������ü���� �Ÿ��� 0.7������ ��� "jumping"�� false��
      if(rayHit.collider != null){
        if(rayHit.distance < 0.7f){
          anim.SetBool("jumping", false);
        }
      }
      }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //���� ��ü�� �ε����� ��� �ε��� ������Ʈ�� �±װ� "��"�̸� OnDamaged�Լ� ȣ��
        if (collision.gameObject.tag == "Enemy")
        {
            OnDamaged(collision.transform.position);
        }

        //�÷��̾ �������� �԰� �� �ݴ�������� �з������� (�������� �Դ��߿��� �� �̻��� �������� ���� �ʵ��� ��)
        void OnDamaged(Vector2 targetPos)
        {
            gameObject.layer = 11;
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

            anim.SetTrigger("doDamaged");

            Invoke("OffDamaged", 2);
        }

    }
        //�÷��̾ �������� �����԰� �ٽ� ���� ���·� �ǵ��ư�
        void OffDamaged()
        {
            gameObject.layer = 10;
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            //������ ������ ������ ������ ��
            if (collision.gameObject.tag == "Item")
            {
                bool isBronze = collision.gameObject.name.Contains("Bronze");
                bool isGold = collision.gameObject.name.Contains("Gold");
                bool isSilver = collision.gameObject.name.Contains("Silver");

                if (isBronze)
                    gameManager.stagepoint += 10;
                else if (isSilver)
                    gameManager.stagepoint += 20;
                else if (isGold)
                    gameManager.stagepoint += 30;

                collision.gameObject.SetActive(false);
            }
            //������ �����ϸ� �������������� �̵���
            else if (collision.gameObject.tag == "Finish")
            {
                gameManager.NextStage();
            }
        }
}