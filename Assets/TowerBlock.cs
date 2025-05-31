using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TowerBlock : MonoBehaviour
{
    [HideInInspector]
    public BoxCollider2D m_Collider;
    [HideInInspector]
    public BlockType m_BlockType;
    public SpriteRenderer m_BlockSprite;

    [Header("Checking collision")]
    public LayerMask CollisionMask;
    ContactFilter2D ColliderFilter;
    List<Collider2D> Collisions;

    [System.Serializable]
    public enum BlockType {
        None = -1,
        Armor,
        Bomb,
        Gun,
        Laser
    }

    public Color ArmorColor = Color.gray;
    public Color BombColor = Color.red;
    public Color GunColor = Color.blue;
    public Color LaserColor = Color.yellow;

    private void Awake()
    {
        m_Collider = GetComponent<BoxCollider2D>();
        ColliderFilter = new ContactFilter2D();
        ColliderFilter.SetLayerMask(CollisionMask);
        Collisions = new List<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Check collisions");
            IsOverlappingSomething();
        }

    }

    public void Init(BlockType type)
    {
        m_BlockType = type;
        switch (type)
        {
            case BlockType.Armor:
                m_BlockSprite.color = ArmorColor;
                break;
            case BlockType.Bomb:
                m_BlockSprite.color = BombColor;
                break;
            case BlockType.Gun:
                m_BlockSprite.color = GunColor;
                break;
            case BlockType.Laser:
                m_BlockSprite.color = LaserColor;
                break;
        }
    }

    public bool IsOverlappingSomething()
    {
        m_Collider.Overlap(ColliderFilter, Collisions);

        if (Collisions.Count > 0)
        {
            //Debug.Log("Block " + gameObject.name + " would be overlapping " + Collisions[0].gameObject.name);
            return true;
        }
        return false;
    }
}
