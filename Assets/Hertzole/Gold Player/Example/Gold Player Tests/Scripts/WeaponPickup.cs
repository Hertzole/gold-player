//using Hertzole.GoldPlayer.Weapons;
//using UnityEngine;

//public class WeaponPickup : MonoBehaviour
//{
//    [SerializeField]
//    private int m_WeaponIndex = 0;

//    // Update is called once per frame
//    void Update()
//    {
//        transform.Rotate(Vector3.up * 45f * Time.deltaTime);
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            GoldPlayerWeapons weapons = other.GetComponent<GoldPlayerWeapons>();
//            if (weapons != null)
//            {
//                weapons.AddWeapon(m_WeaponIndex);
//                weapons.ChangeWeapon(weapons.MyWeaponIndexes.Count - 1);
//                gameObject.SetActive(false);
//            }
//        }
//    }
//}
