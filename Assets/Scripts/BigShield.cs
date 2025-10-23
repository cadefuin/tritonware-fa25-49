using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BigShield : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShieldTimer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();

            enemyScript.shieldHP = 3;
        }



    }
    
    public IEnumerator ShieldTimer()
    {
        //SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        Renderer objectRenderer = GetComponent<Renderer>();
        for (int i = 0; i < 10; i++)
        {
            objectRenderer.material.color = new Color(1, 1, 1, 1 - (i * 0.1f));
            gameObject.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }
}
