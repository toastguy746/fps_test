using UnityEngine;

// 더미 총알 클래스: 충돌 시 데미지를 주고 일정 시간이 지나면 삭제됨
public class dummyBullet : MonoBehaviour
{
    public float damage = 30f;   // 총알이 줄 데미지 양
    public float lifetime = 5f;  // 총알이 존재하는 최대 시간(초)

    void Start()
    {
        // 일정 시간이 지나면 총알 오브젝트 자동 삭제
        Destroy(gameObject, lifetime);
    }

    // 총알이 다른 오브젝트와 충돌했을 때 호출
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트 또는 부모 오브젝트에서 ObjectHealth 컴포넌트 찾기
        ObjectHealth health = collision.collider.GetComponentInParent<ObjectHealth>();
        
        // ObjectHealth가 있다면 데미지 적용
        if (health != null)
        {
            health.TakeDamage((int)damage);
        }

        // 충돌 후 즉시 총알 삭제
        Destroy(gameObject);
    }
}
