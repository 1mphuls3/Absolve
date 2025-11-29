using UnityEngine;
using UnityEngine.SceneManagement;

public class EchoManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] int maxEquipped;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public bool EquipEcho(EchoAbillity echo)
    {
        if(player.activeEchoes.Contains(echo) || player.activeEchoes.Count >= maxEquipped)
        {
            return false;
        }
        player.activeEchoIcons[player.activeEchoes.Count].sprite = echo.icon;
        player.activeEchoes.Add(echo);
        echo.OnEquip(player);
        return true;
    }

    public void UnequipEcho(EchoAbillity echo)
    {
        player.activeEchoes.Remove(echo);
        echo.OnUnequip(player);
    }
}
