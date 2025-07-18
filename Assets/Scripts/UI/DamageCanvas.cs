using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageCanvas : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI damageText;
        private Rigidbody2D rb2d;
    
    
    public void Setup(float damage , Vector2 direction)
    {
        damageText.text = damage.ToString();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddForce(direction*10 , ForceMode2D.Impulse);

        StartCoroutine(StartFadeOut(Random.Range(0.7f, 1.2f)));
    }



    private IEnumerator StartFadeOut(float time)
    {
        
        yield return new WaitForSeconds(time);

        StartCoroutine(Fade(damageText, 0, time));
        
        
        yield return new WaitForSeconds(time);
        
        Destroy(gameObject);
        
        
        
    }
    
    
    IEnumerator Fade(TextMeshProUGUI mat, float targetAlpha , float fadeSpeed)
    {
        while(mat.color.a != targetAlpha)
        {
            var newAlpha = Mathf.MoveTowards(mat.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, newAlpha);
            yield return null;
        }
    }
    
    
    
}
