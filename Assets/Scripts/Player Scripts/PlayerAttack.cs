using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private WeaponManager weapon_Manager;
    private Animator zoomCameraAnim;
    private Camera mainCam;
    private GameObject crosshair;
    private bool is_Aiming;

    [SerializeField] private GameObject arrow_Prefab, spear_Prefab;
    [SerializeField] private Transform arrow_Bow_StartPosition;
    public float fireRate = 15f;
    private float nextTimeToFire;
    public float damage = 20f;

    void Awake()
    {
        weapon_Manager = GetComponent<WeaponManager>();
        zoomCameraAnim = transform.Find(Tags.LOOK_ROOT)
            .transform.Find(Tags.ZOOM_CAMERA).GetComponent<Animator>();
        crosshair = GameObject.FindWithTag(Tags.CROSSHAIR);
        mainCam = Camera.main;
    }

    // 🔫 Mobil butona bağlanacak
    public void OnShootButton()
    {
        var weapon = weapon_Manager.GetCurrentSelectedWeapon();

        if (weapon.fireType == WeaponFireType.MULTIPLE)
        {
            if (Time.time > nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                weapon.ShootAnimation();
                BulletFired();
            }
        }
        else
        {
            // axe
            if (weapon.tag == Tags.AXE_TAG)
            {
                weapon.ShootAnimation();
            }
            // bullet
            else if (weapon.bulletType == WeaponBulletType.BULLET)
            {
                weapon.ShootAnimation();
                BulletFired();
            }
            // arrow / spear
            else if (is_Aiming)
            {
                weapon.ShootAnimation();
                if (weapon.bulletType == WeaponBulletType.ARROW)
                    ThrowArrowOrSpear(true);
                else if (weapon.bulletType == WeaponBulletType.SPEAR)
                    ThrowArrowOrSpear(false);
            }
        }
    }

    // 🎯 Mobil butona bağlanacak
    public void OnAimButtonDown()
    {
        var weapon = weapon_Manager.GetCurrentSelectedWeapon();

        if (weapon.weapon_Aim == WeaponAim.AIM)
        {
            zoomCameraAnim.Play(AnimationTags.ZOOM_IN_ANIM);
            crosshair.SetActive(false);
        }
        else if (weapon.weapon_Aim == WeaponAim.SELF_AIM)
        {
            weapon.Aim(true);
            is_Aiming = true;
        }
    }

    public void OnAimButtonUp()
    {
        var weapon = weapon_Manager.GetCurrentSelectedWeapon();

        if (weapon.weapon_Aim == WeaponAim.AIM)
        {
            zoomCameraAnim.Play(AnimationTags.ZOOM_OUT_ANIM);
            crosshair.SetActive(true);
        }
        else if (weapon.weapon_Aim == WeaponAim.SELF_AIM)
        {
            weapon.Aim(false);
            is_Aiming = false;
        }
    }

    void ThrowArrowOrSpear(bool throwArrow)
    {
        GameObject obj = Instantiate(throwArrow ? arrow_Prefab : spear_Prefab);
        obj.transform.position = arrow_Bow_StartPosition.position;
        obj.GetComponent<ArrowBowScript>().Launch(mainCam);
    }

    void BulletFired()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit))
        {
            if (hit.transform.CompareTag(Tags.ENEMY_TAG))
            {
                hit.transform.GetComponent<HealthScript>().ApplyDamage(damage);
            }
        }
    }
}
