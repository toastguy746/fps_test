using UnityEngine;

public class DummyShooter : MonoBehaviour
{
    public GameObject bulletPrefab;     // 빨간 구 프리팹
    public Transform firePoint;         // 총알 나가는 위치
    public float fireRate = 1.5f;       // 초당 발사 횟수
    public float bulletSpeed = 20f;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;              // 중력 끔 (포물선 없애기)
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}
