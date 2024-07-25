using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class SpriteDataBase : MonoBehaviour
{
    public static SpriteDataBase instance;

    public Sprite[] sprites;

    public Dictionary<string,Sprite> spriteDataBase = new Dictionary<string, Sprite>();

    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (sprites != null)
        {
            foreach (Sprite sprite in sprites)
            {
                if (sprite != null)
                {
                    spriteDataBase[sprite.name] = sprite;
                }
            }
        }
    }
    public Sprite GetSprite(string name)
    {
        if (spriteDataBase.TryGetValue(name, out Sprite sprite))
        {
            return sprite;
        }
        return null;
    }
}
